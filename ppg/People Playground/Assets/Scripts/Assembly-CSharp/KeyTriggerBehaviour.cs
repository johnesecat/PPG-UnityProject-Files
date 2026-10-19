using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyTriggerBehaviour : MonoBehaviour
{
	public KeyCode KeyCode;

	public bool DoubleTrigger;

	public string Description;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public Gradient LightGradient;

	[SkipSerialisation]
	public SpriteRenderer ActiveLight;

	[SkipSerialisation]
	public TextMeshPro TextMesh;

	[SkipSerialisation]
	public TextMeshPro DescriptionGizmo;

	private bool shouldSendContinuous;

	[HideInInspector]
	public float SignalHeat;

	public float SignalCooldownSpeed = 1f;

	private void Awake()
	{
		ContinuousActivationBehaviour.Instance.OnContinuousUpdate.AddListener(OnContinuousUpdate);
	}

	private void Start()
	{
		PhysicalBehaviour.ContextMenuOptions.Buttons.Add(new ContextMenuButton("changeKeyTrigger", "Change key", "Edit trigger key", delegate
		{
			DialogBox dialog = null;
			dialog = DialogBoxManager.KeyEntry("Trigger key\nClick to edit", KeyCode, new DialogButton("Apply", true, setKeyFromDialog), new DialogButton("Cancel", true));
			void setKeyFromDialog()
			{
				KeyCode = dialog.InputKey;
				UpdateDisplay();
			}
		}));
		PhysicalBehaviour.ContextMenuOptions.Buttons.Add(new ContextMenuButton("changeKeyTriggerDescription", "Set description", "Set key trigger description", delegate
		{
			Utils.OpenTextInputDialog(Description, this, delegate(KeyTriggerBehaviour keyTrigger, string value)
			{
				keyTrigger.Description = value;
				UpdateDisplay();
			}, $"Set description\n\"Press {KeyCode} to...\"", "move forward, toggle lights, etc.");
		}));
		List<ContextMenuButton> buttons = PhysicalBehaviour.ContextMenuOptions.Buttons;
		ContextMenuButton item = new ContextMenuButton("toggleKeyTriggerBehaviour", () => (!DoubleTrigger) ? "Double trigger" : "Single trigger", "Toggle trigger behaviour. Double trigger behaviour ensures one extra trigger when the key is released, in addition to it being pressed.", delegate
		{
			DoubleTrigger = !DoubleTrigger;
		})
		{
			LabelWhenMultipleAreSelected = "Toggle key trigger behaviour"
		};
		buttons.Add(item);
		UpdateDisplay();
	}

	private void Update()
	{
		shouldSendContinuous = false;
		if (!DialogBox.IsAnyDialogboxOpen && !Global.ActiveUiBlock && !TriggerEditorBehaviour.IsBeingEdited)
		{
			if (Input.GetKeyDown(KeyCode))
			{
				SendSignal();
			}
			shouldSendContinuous = Input.GetKey(KeyCode);
			if (DoubleTrigger && Input.GetKeyUp(KeyCode))
			{
				SendSignal();
			}
			SignalHeat = Mathf.Clamp01(SignalHeat - Time.deltaTime * SignalCooldownSpeed);
		}
	}

	private void OnWillRenderObject()
	{
		ActiveLight.color = LightGradient.Evaluate(SignalHeat);
		if (DescriptionGizmo.gameObject.activeInHierarchy)
		{
			DescriptionGizmo.transform.localScale = Vector3.one;
		}
	}

	private void SendSignal()
	{
		SignalHeat = 1f;
		if (base.enabled)
		{
			ushort[] allChannels = ActivationPropagation.AllChannels;
			foreach (ushort channel in allChannels)
			{
				SendMessage("IsolatedActivation", new ActivationPropagation(base.transform.root, channel), SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void OnEnable()
	{
		UpdateDisplay();
		TextMesh.enabled = true;
		ActiveLight.enabled = true;
	}

	private void OnDisable()
	{
		TextMesh.enabled = false;
		ActiveLight.enabled = false;
	}

	public void UpdateDisplay()
	{
		TextMesh.text = KeyCode.ToString();
		DescriptionGizmo.text = ((KeyCode == KeyCode.None) ? string.Empty : (string.IsNullOrWhiteSpace(Description) ? $"Press {KeyCode}" : $"Press {KeyCode} to {Description}"));
		DescriptionGizmo.transform.rotation = Quaternion.identity;
	}

	private void OnContinuousUpdate(float dt)
	{
		if (shouldSendContinuous && base.enabled)
		{
			SignalHeat = Mathf.Max(0.5f, SignalHeat);
			Utils.SendAllChannelContinuousIsolatedActivation(this);
		}
	}
}

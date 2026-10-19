using UnityEngine;

public class ButtonBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public SliderJoint2D slider;

	[SkipSerialisation]
	public SpriteRenderer light;

	[SkipSerialisation]
	public PhysicalBehaviour phys;

	[HideInInspector]
	public float strength;

	public float Threshold = 0.1f;

	private bool isPressed;

	public bool TriggerOnExit;

	private void Awake()
	{
		ContinuousActivationBehaviour.Instance.OnContinuousUpdate.AddListener(OnContinuousUpdate);
	}

	private void OnContinuousUpdate(float dt)
	{
		if (isPressed)
		{
			Utils.SendAllChannelContinuousIsolatedActivation(this);
		}
	}

	private void Start()
	{
		phys.ContextMenuOptions.Buttons.Add(new ContextMenuButton("toggleTriggerBehaviour", () => (!TriggerOnExit) ? "Double trigger" : "Single trigger", "Toggle trigger behaviour", delegate
		{
			TriggerOnExit = !TriggerOnExit;
		}));
	}

	private void Update()
	{
		light.enabled = isPressed;
	}

	private void FixedUpdate()
	{
		bool flag = isPressed;
		isPressed = Mathf.Abs(slider.jointTranslation) < Threshold * base.transform.lossyScale.y;
		if (isPressed && !flag)
		{
			Utils.SendAllChannelIsolatedActivation(this);
		}
		if (TriggerOnExit && !isPressed && flag)
		{
			Utils.SendAllChannelIsolatedActivation(this);
		}
	}

	private void OnDisable()
	{
		light.enabled = false;
	}

	private void OnDestroy()
	{
		ContinuousActivationBehaviour.Instance.OnContinuousUpdate.RemoveListener(OnContinuousUpdate);
	}
}

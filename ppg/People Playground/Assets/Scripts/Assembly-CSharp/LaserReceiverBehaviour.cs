using System.Collections.Generic;
using UnityEngine;

public class LaserReceiverBehaviour : MonoBehaviour, Messages.ILasered
{
	public bool DoubleTrigger;

	[SkipSerialisation]
	public SpriteRenderer Light;

	private float laserTime;

	private const float threshold = 0.1f;

	private float lastLaserTime;

	private void Awake()
	{
		ContinuousActivationBehaviour.Instance.OnContinuousUpdate.AddListener(OnContinuousUpdate);
	}

	private void Start()
	{
		List<ContextMenuButton> buttons = GetComponent<PhysicalBehaviour>().ContextMenuOptions.Buttons;
		ContextMenuButton[] array = new ContextMenuButton[1];
		ContextMenuButton contextMenuButton = new ContextMenuButton("toggleGateTriggerBehaviour", () => (!DoubleTrigger) ? "Double trigger receiver" : "Single trigger receiver", "Toggle gate behaviour.", delegate
		{
			DoubleTrigger = !DoubleTrigger;
		})
		{
			LabelWhenMultipleAreSelected = "Toggle gate behaviour"
		};
		array[0] = contextMenuButton;
		buttons.AddRange(array);
	}

	private void OnContinuousUpdate(float dt)
	{
		if (laserTime > 0.1f && base.enabled)
		{
			for (int i = 0; i < ActivationPropagation.AllChannels.Length; i++)
			{
				ushort channel = ActivationPropagation.AllChannels[i];
				SendMessage("IsolatedContinuousActivation", new ActivationPropagation(base.transform.root, channel), SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void Update()
	{
		if (Global.main.Paused || Global.main.GetPausedMenu())
		{
			return;
		}
		if (DoubleTrigger && laserTime < 0.1f && lastLaserTime > 0.1f)
		{
			for (int i = 0; i < ActivationPropagation.AllChannels.Length; i++)
			{
				ushort channel = ActivationPropagation.AllChannels[i];
				SendMessage("IsolatedActivation", new ActivationPropagation(base.transform.root, channel), SendMessageOptions.DontRequireReceiver);
			}
		}
		lastLaserTime = laserTime;
		laserTime -= Time.deltaTime;
		laserTime = Mathf.Clamp01(laserTime);
		Light.enabled = laserTime > 0.1f;
	}

	private void OnDestroy()
	{
		ContinuousActivationBehaviour.Instance.OnContinuousUpdate.RemoveListener(OnContinuousUpdate);
	}

	private void OnDisable()
	{
		Light.enabled = false;
	}

	public void OnLasered(Messages.ILasered.LaserArgs args)
	{
		if (laserTime < 0.1f && base.enabled)
		{
			for (int i = 0; i < ActivationPropagation.AllChannels.Length; i++)
			{
				ushort channel = ActivationPropagation.AllChannels[i];
				SendMessage("IsolatedActivation", new ActivationPropagation(base.transform.root, channel), SendMessageOptions.DontRequireReceiver);
			}
		}
		laserTime = 0.2f;
	}
}

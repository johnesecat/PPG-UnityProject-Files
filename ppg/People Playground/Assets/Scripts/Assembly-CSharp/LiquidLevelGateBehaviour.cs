using System;
using System.Collections.Generic;
using UnityEngine;

[Obsolete]
public class LiquidLevelGateBehaviour : MonoBehaviour
{
	public enum LiquidLevelDetectorMode
	{
		GreaterThan,
		LessThan
	}

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public float CursorMaxDistance;

	[SkipSerialisation]
	public GameObject PassthroughLight;

	[SkipSerialisation]
	public SpriteRenderer Cursor;

	[SkipSerialisation]
	public AudioClip Tick;

	public float Threshold = 0.5f;

	public LiquidLevelDetectorMode Mode;

	public bool TriggerOnExit;

	private bool passingThrough;

	public bool Passingthrough => passingThrough;

	private void Awake()
	{
		ContinuousActivationBehaviour.Instance.OnContinuousUpdate.AddListener(OnContinuousUpdate);
	}

	private void Start()
	{
		List<ContextMenuButton> buttons = PhysicalBehaviour.ContextMenuOptions.Buttons;
		buttons.Add(new ContextMenuButton("toggleTriggerBehaviour", () => (!TriggerOnExit) ? "Double trigger" : "Single trigger", "Toggle trigger behaviour", delegate
		{
			TriggerOnExit = !TriggerOnExit;
		}));
		buttons.AddRange(new ContextMenuButton[1]
		{
			new ContextMenuButton("setLiquidGateThreshold", "Set threshold", "Set detector threshold", delegate
			{
				Utils.OpenFloatInputDialog(Threshold * 100f, this, delegate(LiquidLevelGateBehaviour e, float v)
				{
					e.Threshold = Mathf.Clamp01(v / 100f);
				}, "Set the detector liquid threshold", "Threshold as percentage");
			})
		});
	}

	protected void Update()
	{
		Cursor.transform.localPosition = new Vector3(0f, 0f, CursorMaxDistance * Threshold);
	}

	private void OnContinuousUpdate(float dt)
	{
		if (passingThrough)
		{
			Utils.SendAllChannelContinuousIsolatedActivation(this);
		}
	}

	private void OnDestroy()
	{
		ContinuousActivationBehaviour.Instance.OnContinuousUpdate.RemoveListener(OnContinuousUpdate);
	}

	private void FixedUpdate()
	{
		throw new NotImplementedException();
	}

	private void OnWillRenderObject()
	{
		if (base.enabled)
		{
			PassthroughLight.SetActive(passingThrough);
		}
	}
}

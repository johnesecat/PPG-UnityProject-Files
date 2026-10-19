using System.Collections.Generic;
using UnityEngine;

public class LiquidPumpBehaviour : BloodContainer, Messages.IUse
{
	[SkipSerialisation]
	public Texture2D Push;

	[SkipSerialisation]
	public Texture2D Pull;

	[SkipSerialisation]
	public Texture2D Idle;

	public PressureDirection Mode = PressureDirection.None;

	[SkipSerialisation]
	public SpriteRenderer DisplayRenderer;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	private Material material;

	public float VibrateForce;

	public float VibrateRotationSpeed;

	public override bool AllowsTransfer => false;

	public override bool AllowsOverflow => false;

	public override bool AllowPressureTransfer => true;

	public override Vector2 Limits => new Vector2(0f, 1f);

	public override PressureDirection Pressure => Mode;

	public Vector2 ForceVector
	{
		get
		{
			float f = Time.time * VibrateRotationSpeed;
			return new Vector2(Mathf.Cos(f), Mathf.Sin(f)) * VibrateForce;
		}
	}

	private void Awake()
	{
		material = DisplayRenderer.material;
	}

	private void Start()
	{
		List<ContextMenuButton> buttons = GetComponent<PhysicalBehaviour>().ContextMenuOptions.Buttons;
		buttons.Add(new ContextMenuButton(() => Pressure != PressureDirection.Pull, "setPumpToPull", "Depressurise", "Depressurise the connected container", delegate
		{
			SetMode(PressureDirection.Pull);
		}));
		buttons.Add(new ContextMenuButton(() => Pressure != PressureDirection.Push, "setPumpToPush", "Pressurise", "Pressurise the connected container", delegate
		{
			SetMode(PressureDirection.Push);
		}));
		buttons.Add(new ContextMenuButton(() => Pressure != PressureDirection.None, "setPumpToIdle", "Set to idle", "Stop affecting pressure", delegate
		{
			SetMode(PressureDirection.None);
		}));
		SetMode(Mode);
	}

	public void SetMode(PressureDirection mode)
	{
		if (base.enabled)
		{
			Mode = mode;
			UpdateDisplayTexture();
		}
	}

	private void OnEnable()
	{
		SetMode(Mode);
	}

	private void NextMode()
	{
		int mode = (int)Mode;
		mode++;
		if (mode > 2)
		{
			mode = 0;
		}
		SetMode((PressureDirection)mode);
	}

	private void PreviousMode()
	{
		int mode = (int)Mode;
		mode--;
		if (mode < 0)
		{
			mode = 2;
		}
		SetMode((PressureDirection)mode);
	}

	private void FixedUpdate()
	{
		switch (Mode)
		{
		case PressureDirection.Push:
			PhysicalBehaviour.rigidbody.AddRelativeForce(ForceVector, ForceMode2D.Force);
			break;
		case PressureDirection.Pull:
			PhysicalBehaviour.rigidbody.AddRelativeForce(-ForceVector, ForceMode2D.Force);
			break;
		}
	}

	private void UpdateDisplayTexture()
	{
		material.SetTexture(ShaderProperties.Get("_LEDMultiply"), GetTargetDisplayTexture());
	}

	private Texture2D GetTargetDisplayTexture()
	{
		switch (Mode)
		{
		case PressureDirection.Push:
			return Push;
		case PressureDirection.Pull:
			return Pull;
		case PressureDirection.None:
			return Idle;
		default:
			return Idle;
		}
	}

	public void Use(ActivationPropagation activation)
	{
		if (activation.Channel != 1)
		{
			NextMode();
		}
		else
		{
			PreviousMode();
		}
	}

	private void OnDestroy()
	{
		Object.Destroy(material);
	}
}

using System.Collections.Generic;
using UnityEngine;

public class PointerBehaviour : MonoBehaviour, Messages.IUse
{
	public bool Activated;

	public float Speed = 1f;

	public float Force = 1f;

	public float Delta;

	[SkipSerialisation]
	public Transform RotorWheel;

	[SkipSerialisation]
	public AudioSource AudioSource;

	[SkipSerialisation]
	public HingeJoint2D Hinge;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public SpriteRenderer ActiveLight;

	[SkipSerialisation]
	public float VolumeMultiplier = 0.5f;

	private void Start()
	{
		ActiveLight.enabled = Activated;
		List<ContextMenuButton> buttons = PhysicalBehaviour.ContextMenuOptions.Buttons;
		ContextMenuButton[] array = new ContextMenuButton[1];
		ContextMenuButton contextMenuButton = new ContextMenuButton("pointerSetSpeed", "Set pointer speed", "Set the speed of the pointer", delegate
		{
			Utils.OpenFloatInputDialog(Speed, this, delegate(PointerBehaviour w, float v)
			{
				w.Speed = Mathf.Clamp(v, 0f, 10f);
			}, "Set pointer speed from 0 to 10", "Speed");
		})
		{
			LabelWhenMultipleAreSelected = "Set pointer speed"
		};
		array[0] = contextMenuButton;
		buttons.AddRange(array);
	}

	public void ToggleActivation()
	{
		Activated = !Activated;
		ActiveLight.enabled = Activated;
	}

	public void Use(ActivationPropagation activation)
	{
		ToggleActivation();
	}

	public void FixedUpdate()
	{
		JointMotor2D motor = Hinge.motor;
		if (Activated)
		{
			Delta = GetMouseAngle();
		}
		motor.motorSpeed = Mathf.DeltaAngle(Hinge.jointAngle, Delta) * Speed;
		motor.maxMotorTorque = Force * (1f + PhysicalBehaviour.Charge);
		Hinge.motor = motor;
		float num = Mathf.Abs(Hinge.jointSpeed) * 0.1f;
		if (num > 0.5f)
		{
			if (!AudioSource.isPlaying)
			{
				AudioSource.Play();
			}
			AudioSource.volume = Mathf.Clamp01(num * num * VolumeMultiplier);
		}
		else if (AudioSource.isPlaying)
		{
			AudioSource.Stop();
		}
	}

	private float GetMouseAngle()
	{
		Vector3 normalized = (Global.main.MousePosition - RotorWheel.position).normalized;
		normalized = base.transform.InverseTransformDirection(normalized);
		return Vector2.SignedAngle(new Vector2(0f - normalized.x, normalized.y), Vector2.up);
	}

	private void OnDisable()
	{
		Hinge.useMotor = false;
		AudioSource.Stop();
	}

	private void OnEnable()
	{
		Hinge.useMotor = true;
	}
}

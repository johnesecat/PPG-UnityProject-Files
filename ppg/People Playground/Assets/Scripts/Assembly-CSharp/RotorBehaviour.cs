using System;
using System.Collections.Generic;
using UnityEngine;

public class RotorBehaviour : MonoBehaviour, Messages.IUse
{
	public bool Activated;

	public float Speed = 1f;

	[SkipSerialisation]
	public HingeJoint2D Hinge;

	[SkipSerialisation]
	[Obsolete]
	public AudioSource AudioSource;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public SpriteRenderer ActiveLight;

	public const float MaxSpeed = 8000f;

	private void Start()
	{
		SetSpeed(Speed);
		ActiveLight.enabled = Activated;
		List<ContextMenuButton> buttons = PhysicalBehaviour.ContextMenuOptions.Buttons;
		ContextMenuButton[] array = new ContextMenuButton[2];
		ContextMenuButton contextMenuButton = new ContextMenuButton("rotorSetSpeed", "Set rotor speed", "Set the speed of the rotor", delegate
		{
			Utils.OpenFloatInputDialog(Speed, this, delegate(RotorBehaviour w, float v)
			{
				w.SetSpeed(v);
			}, "Set rotor speed", "Speed");
		})
		{
			LabelWhenMultipleAreSelected = "Set rotor speed"
		};
		array[0] = contextMenuButton;
		contextMenuButton = new ContextMenuButton("reverseRotor", "Reverse rotor", "Reverse rotor", delegate
		{
			SetSpeed(0f - Speed);
		})
		{
			LabelWhenMultipleAreSelected = "Reverse rotor"
		};
		array[1] = contextMenuButton;
		buttons.AddRange(array);
	}

	public void SetSpeed(float v)
	{
		Speed = Mathf.Clamp(v, -8000f, 8000f);
	}

	private void OnDisable()
	{
		Hinge.useMotor = false;
	}

	private void OnEnable()
	{
		Hinge.useMotor = true;
	}

	public void Use(ActivationPropagation activation)
	{
		if (activation.Channel == 1)
		{
			SetSpeed(0f - Speed);
			return;
		}
		Activated = !Activated;
		ActiveLight.enabled = Activated;
	}

	public float GetScaledSpeed()
	{
		return Speed * (float)((!(base.transform.localScale.x < 0f)) ? 1 : (-1));
	}

	public void FixedUpdate()
	{
		JointMotor2D motor = Hinge.motor;
		motor.maxMotorTorque = 100f + PhysicalBehaviour.Charge * 4f;
		motor.motorSpeed = (Activated ? (0f - GetScaledSpeed()) : 0f);
		Hinge.motor = motor;
	}
}

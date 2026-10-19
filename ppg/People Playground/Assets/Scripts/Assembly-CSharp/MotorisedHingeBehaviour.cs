using System.Collections.Generic;
using UnityEngine;

public class MotorisedHingeBehaviour : MonoBehaviour, Messages.IUse
{
	public bool Activated;

	public float Delta = 90f;

	public float Speed = 1f;

	public float Force = 1f;

	[SkipSerialisation]
	public Transform IndicatorArm;

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
		SetDelta(Delta);
		ActiveLight.enabled = Activated;
		List<ContextMenuButton> buttons = PhysicalBehaviour.ContextMenuOptions.Buttons;
		ContextMenuButton[] array = new ContextMenuButton[2];
		ContextMenuButton contextMenuButton = new ContextMenuButton("servoSetDelta", "Set servo target angle", "Set the target angle of the servo", delegate
		{
			Utils.OpenFloatInputDialog(Delta, this, delegate(MotorisedHingeBehaviour w, float v)
			{
				w.SetDelta(v);
			}, "Set servo delta", "Angle");
		})
		{
			LabelWhenMultipleAreSelected = "Set servo target angle"
		};
		array[0] = contextMenuButton;
		contextMenuButton = new ContextMenuButton("servoSetSpeed", "Set servo speed", "Set the speed of the servo", delegate
		{
			Utils.OpenFloatInputDialog(Speed, this, delegate(MotorisedHingeBehaviour w, float v)
			{
				w.Speed = Mathf.Clamp(v, 0f, 10f);
			}, "Set servo speed from 0 to 10", "Speed");
		})
		{
			LabelWhenMultipleAreSelected = "Set servo speed"
		};
		array[1] = contextMenuButton;
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
		motor.motorSpeed = Mathf.DeltaAngle(Hinge.jointAngle, Activated ? GetScaledDelta() : 0f) * Speed;
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

	private float GetScaledDelta()
	{
		return Delta * (float)((base.transform.localScale.x > 0f) ? 1 : (-1));
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

	public void SetDelta(float v)
	{
		Delta = v;
		if ((bool)IndicatorArm)
		{
			IndicatorArm.localEulerAngles = new Vector3(0f, 0f, Delta);
		}
	}
}

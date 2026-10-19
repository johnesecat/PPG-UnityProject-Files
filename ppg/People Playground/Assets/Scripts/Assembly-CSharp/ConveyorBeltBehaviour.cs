using UnityEngine;

public class ConveyorBeltBehaviour : MonoBehaviour, Messages.IUse
{
	[SkipSerialisation]
	public HingeJoint2D[] WheelJoints;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	public float Speed = 3f;

	public float ChargeInfluence = 1f;

	public bool Activated;

	public float FinalSpeed => (Speed + PhysicalBehaviour.Charge * ChargeInfluence) * base.transform.localScale.x;

	private void Start()
	{
		SetActivation();
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			Activated = !Activated;
			SetActivation();
		}
	}

	private void SetActivation()
	{
		HingeJoint2D[] wheelJoints = WheelJoints;
		for (int i = 0; i < wheelJoints.Length; i++)
		{
			wheelJoints[i].useMotor = Activated;
		}
	}

	private void FixedUpdate()
	{
		float motorSpeed = (Activated ? FinalSpeed : 0f);
		HingeJoint2D[] wheelJoints = WheelJoints;
		foreach (HingeJoint2D obj in wheelJoints)
		{
			obj.useMotor = Activated;
			JointMotor2D motor = obj.motor;
			motor.motorSpeed = motorSpeed;
			obj.motor = motor;
		}
	}

	private void OnDisable()
	{
		Activated = false;
		SetActivation();
	}
}

using UnityEngine;

public class EBikeBehaviour : MonoBehaviour, Messages.IUse
{
	public bool Activated;

	[SkipSerialisation]
	public float Speed = 100f;

	[SkipSerialisation]
	public HingeJoint2D Hinge;

	private void Start()
	{
		float num = Speed;
		if (base.transform.lossyScale.x > 0f)
		{
			num *= -1f;
		}
		JointMotor2D motor = Hinge.motor;
		motor.motorSpeed = num;
		Hinge.motor = motor;
		SetActivation();
	}

	public void Use(ActivationPropagation activation)
	{
		Activated = !Activated;
		SetActivation();
	}

	private void SetActivation()
	{
		if ((bool)Hinge)
		{
			Hinge.useMotor = Activated;
		}
	}
}

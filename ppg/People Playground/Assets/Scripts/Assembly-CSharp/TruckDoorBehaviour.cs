using UnityEngine;

public class TruckDoorBehaviour : MonoBehaviour, Messages.IUse
{
	public bool Activated;

	private HingeJoint2D joint;

	private void Awake()
	{
		joint = GetComponent<HingeJoint2D>();
	}

	public void Use(ActivationPropagation activation)
	{
		Activated = !Activated;
	}

	private void FixedUpdate()
	{
		if ((bool)joint)
		{
			JointMotor2D motor = joint.motor;
			motor.motorSpeed = Mathf.DeltaAngle(joint.jointAngle, Activated ? 0f : (90f * base.transform.root.localScale.x)) * 3f;
			joint.motor = motor;
		}
	}
}

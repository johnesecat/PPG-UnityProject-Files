using NaughtyAttributes;
using UnityEngine;

public class AutomaticWheelJointCreator : MonoBehaviour
{
	public Rigidbody2D[] Wheels;

	public float Frequency = 8f;

	public float Dampening = 1f;

	[Button("yo", EButtonEnableMode.Always)]
	private void Awake()
	{
		Rigidbody2D[] wheels = Wheels;
		foreach (Rigidbody2D rigidbody2D in wheels)
		{
			WheelJoint2D wheelJoint2D = base.gameObject.AddComponent<WheelJoint2D>();
			wheelJoint2D.connectedBody = rigidbody2D;
			wheelJoint2D.anchor = rigidbody2D.transform.localPosition;
			wheelJoint2D.connectedAnchor = Vector2.zero;
			JointSuspension2D suspension = wheelJoint2D.suspension;
			suspension.frequency = Frequency;
			suspension.dampingRatio = Dampening;
			wheelJoint2D.suspension = suspension;
			wheelJoint2D.useMotor = true;
		}
	}
}

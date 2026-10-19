using UnityEngine;

public class PropellerBehaviour : MonoBehaviour
{
	public Rigidbody2D rigidbody;

	public HingeJoint2D hinge;

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody2D>();
	}

	public void SetMotorAngle(float angle, float speed = 15f)
	{
		JointMotor2D motor = hinge.motor;
		motor.motorSpeed = (0f - speed) * Mathf.DeltaAngle(base.transform.eulerAngles.z, angle);
		hinge.motor = motor;
	}
}

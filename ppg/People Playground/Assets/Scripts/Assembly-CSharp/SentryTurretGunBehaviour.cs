using UnityEngine;

public class SentryTurretGunBehaviour : SentryTurretControllable
{
	public HingeJoint2D turretJoint;

	public CanShoot firearmBehaviour;

	public SentryTurretBehaviour sentryBehaviour;

	private float timer;

	private bool canFire;

	public const float FireInterval = 0.15f;

	public override Vector2 BarrelPosition => firearmBehaviour.BarrelPosition;

	private void Update()
	{
		if (sentryBehaviour.State == SentryTurretAIState.Firing || sentryBehaviour.State == SentryTurretAIState.Panicking)
		{
			timer += Time.deltaTime;
		}
	}

	private void FixedUpdate()
	{
		JointMotor2D motor = turretJoint.motor;
		switch (sentryBehaviour.State)
		{
		case SentryTurretAIState.Idle:
			canFire = false;
			motor.motorSpeed = Mathf.DeltaAngle(turretJoint.jointAngle, 0f) * 3f;
			motor.motorSpeed = Mathf.Clamp(motor.motorSpeed, -25f, 25f);
			break;
		case SentryTurretAIState.Firing:
			if (sentryBehaviour.Controller.HasTarget)
			{
				Vector3 normalized = (sentryBehaviour.Controller.Target.transform.position - base.transform.position).normalized;
				float num = Mathf.Atan2(normalized.y, normalized.x) * 57.29578f;
				num -= base.transform.parent.eulerAngles.z;
				motor.motorSpeed = base.transform.root.localScale.x * Mathf.DeltaAngle(turretJoint.jointAngle, 0f - num) * 3f;
				motor.motorSpeed = Mathf.Clamp(motor.motorSpeed, -35f, 35f);
				if (timer > 1f)
				{
					canFire = true;
				}
				if (canFire && timer > 0.15f)
				{
					timer = 0f;
					firearmBehaviour.Shoot();
				}
			}
			break;
		case SentryTurretAIState.Searching:
			canFire = false;
			motor.motorSpeed = Mathf.DeltaAngle(turretJoint.jointAngle, Mathf.Sin(Time.time * 3f) * 45f) * 3f;
			motor.motorSpeed = Mathf.Clamp(motor.motorSpeed, -25f, 25f);
			break;
		case SentryTurretAIState.Panicking:
			motor.motorSpeed = Mathf.DeltaAngle(turretJoint.jointAngle, Mathf.Sin(Time.time * 6f) * 45f) * 3f;
			if (timer > 0.15f)
			{
				timer = 0f;
				firearmBehaviour.Shoot();
			}
			break;
		}
		turretJoint.motor = motor;
	}

	public override void Disable()
	{
		GetComponent<LineRenderer>().enabled = false;
	}
}

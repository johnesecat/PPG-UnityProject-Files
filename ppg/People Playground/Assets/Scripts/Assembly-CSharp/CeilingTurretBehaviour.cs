using UnityEngine;
using UnityEngine.Events;

public class CeilingTurretBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public float SightRadius = 20f;

	[SkipSerialisation]
	public float MotorSpeedMultiplier = 1f;

	[SkipSerialisation]
	public LayerMask LayerMask;

	[SkipSerialisation]
	public FirearmBehaviour FirearmBehaviour;

	[SkipSerialisation]
	public HingeJoint2D Joint;

	[SkipSerialisation]
	public UnityEvent OnSight = new UnityEvent();

	[SkipSerialisation]
	public UnityEvent OnLoseTarget = new UnityEvent();

	[SkipSerialisation]
	public float SweepAnimationSpeed = 4f;

	private readonly AutomaticSentryController sentryController = new AutomaticSentryController();

	private float targetMotorSpeed;

	private float combatTime;

	private bool hadTarget;

	private void Start()
	{
		sentryController.SightRange = SightRadius;
		sentryController.LayerMask = LayerMask;
		sentryController.AimFuzziness = 0.001f;
		sentryController.OnShoot.AddListener(delegate
		{
			combatTime = 1f;
			if (FirearmBehaviour.ShotIndex == 0)
			{
				FirearmBehaviour.Shoot();
			}
			else
			{
				FirearmBehaviour.ShootContinuous();
			}
		});
		sentryController.OnSight.AddListener(delegate
		{
			hadTarget = true;
		});
		sentryController.OnSight.AddListener(OnSight.Invoke);
		OnLoseTarget?.Invoke();
	}

	private void Update()
	{
		sentryController.GetTargetMotorSpeed(FirearmBehaviour.transform, FirearmBehaviour.BarrelPosition, FirearmBehaviour.BarrelDirection, out targetMotorSpeed);
		if (!sentryController.HasTarget)
		{
			if (hadTarget)
			{
				OnLoseTarget?.Invoke();
			}
			hadTarget = false;
			targetMotorSpeed = Mathf.Max(0f, 1f - combatTime) * Mathf.DeltaAngle(Joint.jointAngle, Mathf.LerpAngle(Joint.limits.min, Joint.limits.max, Utils.Triangle(Time.time * SweepAnimationSpeed) * 0.5f + 0.5f));
		}
		else
		{
			hadTarget = true;
		}
	}

	private void FixedUpdate()
	{
		combatTime *= 0.99f;
		JointMotor2D motor = Joint.motor;
		motor.motorSpeed = targetMotorSpeed * MotorSpeedMultiplier;
		Joint.motor = motor;
	}

	private void OnDisable()
	{
		Joint.useMotor = false;
	}

	private void OnEnable()
	{
		Joint.useMotor = true;
	}
}

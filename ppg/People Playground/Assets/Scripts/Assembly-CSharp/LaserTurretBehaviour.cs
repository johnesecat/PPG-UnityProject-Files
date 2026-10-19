using NaughtyAttributes;
using UnityEngine;

public class LaserTurretBehaviour : CanShoot, Messages.IUse, Messages.IOnEMPHit
{
	[SkipSerialisation]
	public Vector2 barrelPosition;

	[SkipSerialisation]
	public Vector2 barrelDirection;

	[SkipSerialisation]
	public float radius;

	[SkipSerialisation]
	public PhysicalBehaviour physicalBehaviour;

	[SkipSerialisation]
	public HingeJoint2D joint;

	[SkipSerialisation]
	public AudioClip targetAcquired;

	[SkipSerialisation]
	public AudioSource audioSource;

	[SkipSerialisation]
	public LayerMask layerMask;

	[SkipSerialisation]
	public float recoil = 5f;

	[SkipSerialisation]
	public ParticleSystem muzzleFlash;

	[SkipSerialisation]
	public float interval = 0.5f;

	[SkipSerialisation]
	public float cooldown;

	[SkipSerialisation]
	public GameObject projectile;

	[SkipSerialisation]
	public float inaccuracy = 0.05f;

	[SkipSerialisation]
	public float motorSpeed = 3f;

	[SkipSerialisation]
	public GameObject GlowEffect;

	[SkipSerialisation]
	public GameObject BreakEffect;

	[ReadOnly]
	public bool IsBroken;

	private readonly AutomaticSentryController sentryController = new AutomaticSentryController();

	private float lastTimeShot;

	[SkipSerialisation]
	public override Vector2 BarrelPosition => base.transform.TransformPoint(barrelPosition);

	[SkipSerialisation]
	public Vector2 BarrelDirection => base.transform.TransformDirection(barrelDirection) * base.transform.localScale.x;

	private void Start()
	{
		sentryController.SightRange = radius;
		sentryController.LayerMask = layerMask;
		sentryController.OnShoot.AddListener(Shoot);
		sentryController.OnSight.AddListener(delegate
		{
			audioSource.PlayOneShot(targetAcquired);
		});
		if (IsBroken)
		{
			SetAsBroken();
		}
	}

	public void OnEMPHit()
	{
		SetAsBroken();
	}

	private void SetAsBroken()
	{
		if (!IsBroken)
		{
			Object.Instantiate(BreakEffect, base.transform.position, Quaternion.identity);
		}
		joint.useMotor = false;
		IsBroken = true;
		GlowEffect.SetActive(value: false);
	}

	private void FixedUpdate()
	{
		if (!IsBroken)
		{
			sentryController.GetTargetMotorSpeed(base.transform, BarrelPosition, BarrelDirection, out var num);
			JointMotor2D motor = joint.motor;
			motor.motorSpeed = num * motorSpeed;
			joint.motor = motor;
		}
	}

	public void Use(ActivationPropagation activation)
	{
		if (!IsBroken)
		{
			Shoot();
		}
	}

	public override void Shoot()
	{
		float time = Time.time;
		if (!(time - lastTimeShot < cooldown - physicalBehaviour.charge / 100f))
		{
			lastTimeShot = time;
			CameraShakeBehaviour.main.Shake(4f, base.transform.position);
			physicalBehaviour.rigidbody.AddForceAtPosition(BarrelDirection * (0f - recoil), BarrelPosition);
			audioSource.Play();
			muzzleFlash.Play();
			Object.Instantiate(projectile, BarrelPosition, Quaternion.identity).transform.right = BarrelDirection + Random.insideUnitCircle * inaccuracy;
		}
	}
}

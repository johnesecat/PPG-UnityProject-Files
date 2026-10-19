using UnityEngine;

public class AcceleratorGunBehaviour : CanShoot
{
	[SkipSerialisation]
	public AudioSource LoopAudioSource;

	[SkipSerialisation]
	public AudioSource OneShotAudioSource;

	[SkipSerialisation]
	public AudioClip[] SingleShotSound;

	[SkipSerialisation]
	public AudioClip FullPowerShotSound;

	[SkipSerialisation]
	public AudioClip ChargeUpSound;

	[SkipSerialisation]
	public AudioClip FullPowerWaitingLoopSound;

	[SkipSerialisation]
	public AudioClip LowEnergyBoltLoopSound;

	[SkipSerialisation]
	public AudioClip HighEnergyBoltLoopSound;

	[SkipSerialisation]
	public Rigidbody2D Rigidbody;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public GameObject ProjectilePrefab;

	[SkipSerialisation]
	public Vector2 LocalBarrelPosition;

	[SkipSerialisation]
	public Vector2 LocalBarrelDirection;

	[SkipSerialisation]
	public float RecoilIntensity = 1f;

	[SkipSerialisation]
	public ParticleSystem MuzzleflashSystem;

	[SkipSerialisation]
	public ParticleSystem ChargeUpParticleSystem;

	public float MaxChargeTime = 2.357f;

	public float CurrentChargeProgress;

	public override Vector2 BarrelPosition => Rigidbody.GetRelativePoint(LocalBarrelPosition * base.transform.lossyScale);

	public Vector2 BarrelDirection => base.transform.TransformVector(LocalBarrelDirection);

	public void ShootLowPower()
	{
		OneShotAudioSource.PlayOneShot(SingleShotSound.PickRandom());
		SpawnProjectile(15f + CurrentChargeProgress * 10f, 2f, 80f + CurrentChargeProgress * 10f, 0.6f + CurrentChargeProgress * 0.4f);
		PhysicalBehaviour.rigidbody.AddForceAtPosition(0.1f * (0f - RecoilIntensity) * BarrelDirection, BarrelPosition);
		MuzzleflashSystem.Play();
	}

	public void ShootFullPower()
	{
		OneShotAudioSource.PlayOneShot(FullPowerShotSound);
		SpawnProjectile(250f, 6f, 25f, 1.5f);
		PhysicalBehaviour.rigidbody.AddForceAtPosition(BarrelDirection * (0f - RecoilIntensity), BarrelPosition);
		MuzzleflashSystem.Play();
	}

	private void SpawnProjectile(float energy, float radius, float speed, float size)
	{
		GameObject gameObject = Object.Instantiate(ProjectilePrefab, BarrelPosition, Quaternion.identity);
		gameObject.transform.right = BarrelDirection;
		gameObject.transform.localScale = Vector3.one * size;
		if (gameObject.TryGetComponent<AcceleratorBoltBehaviour>(out var component))
		{
			component.EnergyLevel = energy;
			component.AoERadius = radius;
			component.Speed = speed;
			component.ActivationDelay = radius / speed * 2f;
			component.CameraShakeAmount = energy * 0.05f;
			if (gameObject.TryGetComponent<AudioSource>(out var component2))
			{
				component2.clip = ((energy > 100f) ? HighEnergyBoltLoopSound : LowEnergyBoltLoopSound);
				component2.Play();
			}
		}
	}

	private void Update()
	{
		if (PhysicalBehaviour.StartedBeingUsedContinuously())
		{
			LoopAudioSource.loop = false;
			LoopAudioSource.PlayOneShot(ChargeUpSound);
			ChargeUpParticleSystem.transform.localScale = Vector3.zero;
			ChargeUpParticleSystem.Play();
		}
		if (PhysicalBehaviour.IsBeingUsedContinuously())
		{
			CurrentChargeProgress += Time.deltaTime / MaxChargeTime;
			float num = Time.smoothDeltaTime / MaxChargeTime;
			ChargeUpParticleSystem.transform.localScale += new Vector3(num, num, num);
			if (CurrentChargeProgress >= 0.9999f && LoopAudioSource.clip != FullPowerWaitingLoopSound)
			{
				LoopAudioSource.clip = FullPowerWaitingLoopSound;
				LoopAudioSource.loop = true;
				LoopAudioSource.Play();
			}
		}
		if (PhysicalBehaviour.StoppedBeingUsedContinuously())
		{
			ChargeUpParticleSystem.Stop();
			LoopAudioSource.Stop();
			LoopAudioSource.clip = null;
			if (CurrentChargeProgress >= 0.9f)
			{
				ShootFullPower();
			}
			else if (CurrentChargeProgress >= 0f)
			{
				ShootLowPower();
			}
			CurrentChargeProgress = 0f;
		}
		CurrentChargeProgress = Mathf.Clamp01(CurrentChargeProgress);
	}

	public override void Shoot()
	{
		ShootLowPower();
	}
}

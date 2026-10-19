using System;
using System.Collections;
using UnityEngine;

public class ArchelixCasterBehaviour : CanShoot, Messages.IUse
{
	[SerializeField]
	private GameObject projectilePrefab;

	[SkipSerialisation]
	public AudioClip[] ShootSounds;

	[SkipSerialisation]
	public ParticleSystem MuzzleFlash;

	[SkipSerialisation]
	public Rigidbody2D Rigidbody;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public int BurstShotCount = 3;

	[SkipSerialisation]
	public int BulletsPerShot = 2;

	[SkipSerialisation]
	public float BurstShotInterval = 0.1f;

	[SkipSerialisation]
	public Vector2 LocalBarrelPosition;

	[SkipSerialisation]
	public Vector2 LocalBarrelDirection;

	public float ScreenShakeIntensity = 0.2f;

	private bool phaseOffset;

	public override Vector2 BarrelPosition => Rigidbody.GetRelativePoint(LocalBarrelPosition * base.transform.lossyScale);

	public Vector2 BarrelDirection => base.transform.TransformVector(LocalBarrelDirection);

	private void Start()
	{
		if (!PoolGenerator.Instance.GetFor(projectilePrefab, out var _))
		{
			PoolGenerator.Instance.Ensure(projectilePrefab).MaxPoolSize = 1024u;
		}
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			Shoot();
		}
	}

	public override void Shoot()
	{
		if (base.enabled)
		{
			StartCoroutine(BurstShot());
		}
	}

	private IEnumerator BurstShot()
	{
		PhysicalBehaviour.PlayClipOnce(ShootSounds.PickRandom());
		for (int i = 0; i < BurstShotCount; i++)
		{
			if (!base.enabled)
			{
				break;
			}
			MuzzleFlash.Play();
			CameraShakeBehaviour.main.Shake(ScreenShakeIntensity, base.transform.position);
			for (int j = 0; j < BulletsPerShot; j++)
			{
				GameObject obj = PoolGenerator.Instance.RequestPrefab(projectilePrefab, BarrelPosition);
				obj.transform.right = BarrelDirection;
				if (obj.TryGetComponent<ArchelixCasterBoltBehaviour>(out var component))
				{
					phaseOffset = !phaseOffset;
					component.Direction = BarrelDirection;
					component.Phase = (phaseOffset ? ((float)Math.PI / 2f) : 0f);
				}
			}
			yield return new WaitForSeconds(BurstShotInterval);
		}
	}
}

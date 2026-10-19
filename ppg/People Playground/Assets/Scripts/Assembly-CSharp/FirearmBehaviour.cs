using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FirearmBehaviour : CanShoot, Messages.IUse
{
	public Vector2 barrelPosition;

	public Vector2 barrelDirection;

	[Space]
	public Vector2 casingPosition;

	public Vector2 casingDirection;

	[Space]
	[SkipSerialisation]
	public AudioClip[] ShotSounds;

	[SkipSerialisation]
	public AudioClip TriggerSound;

	public float ShotVolume = 1f;

	public float FireDelay;

	public float InitialInaccuracy = 0.001f;

	[SkipSerialisation]
	public List<float> InaccuracyMultipliers = new List<float>();

	[Space]
	public bool Automatic;

	public int BulletsPerShot = 1;

	public float AutomaticFireInterval = 0.05f;

	[SkipSerialisation]
	public GameObject MuzzleFlashEffectOverride;

	[SkipSerialisation]
	public Cartridge Cartridge;

	[Space]
	public LayerMask LayersToHit;

	[Space]
	[Obsolete]
	public bool StartInCollider = true;

	public bool EjectShells = true;

	public bool UseCustomCasingTexture = true;

	public bool IgnoreUse;

	public bool ApplyCharge;

	public float ShotHeatMultiplier = 1f;

	[SkipSerialisation]
	public float CasingSizeMultiplier = 1f;

	[SkipSerialisation]
	public PhysicalBehaviour Phys;

	[SkipSerialisation]
	public UnityEvent OnFire = new UnityEvent();

	private AudioSource audioSource;

	private ParticleSystem flash;

	private Transform muzzleTransform;

	private Rigidbody2D rb;

	private float timer;

	private ParticleSystem bulletCasing;

	private LayerMask WaterLayer;

	private int shotIndex;

	public bool MuzzleShockwave = true;

	[SkipSerialisation]
	public DistantSoundBehaviour.SoundType DistantSound;

	[SkipSerialisation]
	public BallisticsEmitter BallisticsEmitter;

	[SkipSerialisation]
	public int ShotIndex => shotIndex;

	public override Vector2 BarrelPosition
	{
		get
		{
			if (!rb)
			{
				return base.transform.TransformPoint(barrelPosition);
			}
			return rb.GetRelativePoint(barrelPosition * base.transform.lossyScale);
		}
	}

	public Vector2 BarrelDirection => base.transform.TransformVector(barrelDirection);

	public Vector2 CasingPosition => base.transform.TransformPoint(casingPosition);

	public Vector2 CasingDirection => base.transform.TransformDirection(casingDirection);

	public void ResetIndex()
	{
		shotIndex = 0;
	}

	private void Awake()
	{
		Phys = GetComponent<PhysicalBehaviour>();
		WaterLayer = LayerMask.GetMask("Water") | (int)LayersToHit;
		bulletCasing = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/BulletCasing"), base.transform).GetComponent<ParticleSystem>();
		ParticleSystem.MainModule main = bulletCasing.main;
		main.startSize = 0.18f * CasingSizeMultiplier;
		rb = GetComponent<Rigidbody2D>();
		audioSource = base.gameObject.AddComponent<AudioSource>();
		audioSource.spread = 35f;
		audioSource.volume = ShotVolume;
		audioSource.minDistance = 15f;
		audioSource.spatialBlend = 1f;
		audioSource.dopplerLevel = 0f;
		if (!MuzzleFlashEffectOverride)
		{
			muzzleTransform = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/MuzzleSmoke"), base.transform).transform;
			flash = muzzleTransform.GetComponentInChildren<ParticleSystem>();
		}
		if (UseCustomCasingTexture && (bool)Cartridge.Casing)
		{
			bulletCasing.GetComponent<ParticleSystemRenderer>().sharedMaterial = Cartridge.Casing;
		}
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (Automatic && (bool)Phys && Phys.IsBeingUsedContinuously() && !IgnoreUse && ShotIndex != 0)
		{
			ShootContinuous();
		}
	}

	private void Start()
	{
		BallisticsEmitter = new BallisticsEmitter(this, Cartridge);
		BallisticsEmitter.LayersToHit = LayersToHit;
		BallisticsEmitter.WaterLayer = WaterLayer;
		SetTraversalSettings();
		BallisticsEmitter.BulletRicochetCallback = OnRicochet;
	}

	private void SetTraversalSettings()
	{
		if (UserPreferenceManager.Current.TracerBullets)
		{
			BallisticsEmitter.MaxRange = Mathf.Clamp(Cartridge.StartSpeed / 4f, 4f, 8f);
			BallisticsEmitter.DoTraversal = true;
		}
		else
		{
			BallisticsEmitter.MaxRange = 1000f;
			BallisticsEmitter.DoTraversal = false;
		}
	}

	private void OnRicochet(BallisticsEmitter.CallbackParams args)
	{
		UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Ricochet"), args.Position, Quaternion.identity);
	}

	public void ShootContinuous()
	{
		if (Automatic && timer > AutomaticFireInterval)
		{
			Shoot();
		}
	}

	public void Use(ActivationPropagation activation)
	{
		if (!IgnoreUse)
		{
			shotIndex = 0;
			if (FireDelay > float.Epsilon)
			{
				StartCoroutine(DelayedShot());
			}
			else
			{
				Shoot();
			}
		}
	}

	private IEnumerator DelayedShot()
	{
		if (shotIndex == 0)
		{
			audioSource.PlayOneShot(TriggerSound, 1f);
		}
		yield return new WaitForSeconds(FireDelay);
		Shoot();
	}

	public override void Shoot()
	{
		if (!base.enabled)
		{
			return;
		}
		ModAPI.InvokeGunShot(this, this);
		bool flag = shotIndex == 0;
		if (ShotSounds != null && ShotSounds.Length != 0)
		{
			if (Automatic)
			{
				audioSource.PlayOneShot(ShotSounds.PickRandom(), flag ? 1f : 0.5f);
			}
			else
			{
				audioSource.PlayOneShot(ShotSounds.PickRandom());
			}
		}
		if (UserPreferenceManager.Current.DistantSoundEffects && DistantSound != 0)
		{
			DistantSoundBehaviour.Instance.Play(DistantSound, base.transform.position, 0.5f);
		}
		if (flag)
		{
			SetTraversalSettings();
		}
		if ((bool)Phys && (bool)Cartridge)
		{
			Phys.Temperature += Cartridge.Recoil / Phys.GetHeatCapacity() * 0.05f * ShotHeatMultiplier;
		}
		shotIndex++;
		timer = 0f;
		if (MuzzleShockwave)
		{
			ExplosionCreator.CreatePulseExplosion(BarrelPosition, Cartridge.Recoil / 25f, Cartridge.Recoil / 3f, soundAndEffects: false, breakObjects: false);
		}
		Transform transform;
		if (!MuzzleFlashEffectOverride)
		{
			transform = muzzleTransform;
			transform.localScale = Vector3.one * Mathf.Clamp(Mathf.Pow(Cartridge.Damage * (float)BulletsPerShot, 0.6f) / 4f, 0.5f, 2.5f);
			transform.localScale = new Vector3((float)((base.transform.lossyScale.x > 0f) ? 1 : (-1)) * transform.localScale.x, transform.localScale.y, transform.localScale.z);
			CameraShakeBehaviour.main.Shake(Cartridge.ImpactForce * 0.8f * (float)BulletsPerShot / 0.9f, base.transform.position);
			flash.Play(withChildren: true);
		}
		else
		{
			transform = UnityEngine.Object.Instantiate(MuzzleFlashEffectOverride, BarrelPosition, base.transform.rotation).transform;
		}
		transform.right = BarrelDirection;
		transform.position = BarrelPosition;
		if (EjectShells)
		{
			bulletCasing.transform.position = CasingPosition;
			bulletCasing.transform.right = CasingDirection;
			bulletCasing.Play();
		}
		for (int i = 0; i < BulletsPerShot; i++)
		{
			Fire();
		}
		OnFire?.Invoke();
	}

	private void Fire()
	{
		rb.AddForceAtPosition(0.5f * (0f - Cartridge.Recoil) * base.transform.lossyScale.x * base.transform.right, BarrelPosition, ForceMode2D.Impulse);
		if (ApplyCharge)
		{
			BallisticsEmitter.Charge = Phys.Charge;
		}
		else
		{
			BallisticsEmitter.Charge = 0f;
		}
		BallisticsEmitter.Emit(BarrelPosition, BarrelDirection + UnityEngine.Random.insideUnitCircle * Mathf.Max(0.0025f, InitialInaccuracy * GetInaccuracyMultiplier()));
	}

	private void OnDestroy()
	{
		BallisticsEmitter.Dispose();
	}

	private float GetInaccuracyMultiplier()
	{
		float num = 1f;
		foreach (float inaccuracyMultiplier in InaccuracyMultipliers)
		{
			num *= inaccuracyMultiplier;
		}
		return num;
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileLauncherBehaviour : MonoBehaviour, Messages.IUse
{
	public Vector2 barrelPosition;

	public Vector2 barrelDirection;

	[SkipSerialisation]
	public GameObject projectilePrefab;

	[SkipSerialisation]
	public GameObject muzzleFlashPrefab;

	[SkipSerialisation]
	public SpawnableAsset projectileAsset;

	public bool IsAutomatic;

	public float AutomaticInterval;

	public float projectileLaunchStrength;

	public float recoilMultiplier = 1f;

	[SkipSerialisation]
	public AudioClip launchSound;

	[SkipSerialisation]
	public AudioClip[] LaunchClips;

	[SkipSerialisation]
	public AudioSource launchAudioSource;

	public float ScreenShake;

	public bool ShouldPutProjectileInOtherLayer;

	[Layer]
	[ShowIf("ShouldPutProjectileInOtherLayer")]
	public int LayerToPutProjectileOn = -1;

	[SkipSerialisation]
	public UnityEvent OnLaunchEvent;

	public bool RemoveLaunchedObjectsWithMe;

	private readonly HashSet<UnityEngine.Object> linkedObjects = new HashSet<UnityEngine.Object>();

	private Rigidbody2D rb;

	private PhysicalBehaviour phys;

	private float automaticTimer;

	private static readonly List<Collider2D> bufferA = new List<Collider2D>(8);

	private static readonly List<Collider2D> bufferB = new List<Collider2D>(8);

	[SkipSerialisation]
	public event EventHandler<GameObject> OnLaunch;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		phys = GetComponent<PhysicalBehaviour>();
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			automaticTimer = 0f;
			Launch();
		}
	}

	private void Update()
	{
		if (phys.IsBeingUsedContinuously() && IsAutomatic)
		{
			automaticTimer += Time.deltaTime;
		}
	}

	private void FixedUpdate()
	{
		if (IsAutomatic && automaticTimer > AutomaticInterval)
		{
			automaticTimer = 0f;
			Launch();
		}
	}

	public void Launch()
	{
		StartCoroutine(LaunchRoutine());
	}

	private IEnumerator LaunchRoutine()
	{
		yield return new WaitForFixedUpdate();
		Quaternion rotation = ((base.transform.lossyScale.x < 0f) ? (Quaternion.Euler(0f, 0f, 180f) * base.transform.rotation) : base.transform.rotation);
		GameObject gameObject;
		PhysicalBehaviour component;
		if ((bool)projectileAsset)
		{
			gameObject = UnityEngine.Object.Instantiate(projectileAsset.Prefab, GetBarrelPosition(), rotation);
			component = gameObject.GetComponent<PhysicalBehaviour>();
			component.SpawnSpawnParticles = false;
			gameObject.AddComponent<AudioSourceTimeScaleBehaviour>();
			gameObject.AddComponent<TexturePackApplier>();
			gameObject.AddComponent<SerialiseInstructions>().OriginalSpawnableAsset = projectileAsset;
			gameObject.name = projectileAsset.name;
			CatalogBehaviour.PerformMod(projectileAsset, gameObject);
		}
		else
		{
			gameObject = UnityEngine.Object.Instantiate(projectilePrefab, GetBarrelPosition(), rotation);
			component = gameObject.GetComponent<PhysicalBehaviour>();
		}
		if (ShouldPutProjectileInOtherLayer)
		{
			gameObject.layer = LayerToPutProjectileOn;
		}
		this.OnLaunch?.Invoke(this, gameObject);
		OnLaunchEvent?.Invoke();
		if ((bool)component)
		{
			if (component.SimulateTemperature && (bool)phys && phys.SimulateTemperature)
			{
				component.Temperature = phys.Temperature;
			}
			phys.GetComponentsInChildren(bufferA);
			component.GetComponentsInChildren(bufferB);
			foreach (Collider2D item in bufferA)
			{
				foreach (Collider2D item2 in bufferB)
				{
					IgnoreCollisionStackController.RequestIgnoreCollision(item, item2);
				}
			}
			component.rigidbody.AddForce(GetBarrelDirection() * projectileLaunchStrength, ForceMode2D.Impulse);
		}
		rb.AddForce((0f - projectileLaunchStrength) * recoilMultiplier * gameObject.transform.right, ForceMode2D.Impulse);
		if (RemoveLaunchedObjectsWithMe)
		{
			linkedObjects.Add(gameObject);
		}
		AudioClip audioClip = (launchSound ? launchSound : ((LaunchClips != null) ? LaunchClips.PickRandom() : null));
		if ((bool)audioClip)
		{
			if ((bool)launchAudioSource)
			{
				launchAudioSource.PlayOneShot(audioClip);
			}
			else
			{
				phys.PlayClipOnce(audioClip);
			}
		}
		CameraShakeBehaviour.main.Shake(ScreenShake, base.transform.position);
		if (!muzzleFlashPrefab)
		{
			yield break;
		}
		if (muzzleFlashPrefab.CompareTag("Poolable"))
		{
			GameObject gameObject2 = PoolGenerator.Instance.RequestPrefab(muzzleFlashPrefab, GetBarrelPosition());
			if ((bool)gameObject2)
			{
				gameObject2.transform.right = GetBarrelDirection();
			}
		}
		else
		{
			UnityEngine.Object.Instantiate(muzzleFlashPrefab, GetBarrelPosition(), base.transform.rotation).transform.right = GetBarrelDirection();
		}
	}

	public Vector2 GetBarrelPosition()
	{
		return base.transform.TransformPoint(barrelPosition);
	}

	public Vector2 GetBarrelDirection()
	{
		return base.transform.TransformDirection(barrelDirection) * base.transform.localScale.x;
	}

	private void OnDestroy()
	{
		foreach (UnityEngine.Object linkedObject in linkedObjects)
		{
			UnityEngine.Object.Destroy(linkedObject);
		}
	}
}

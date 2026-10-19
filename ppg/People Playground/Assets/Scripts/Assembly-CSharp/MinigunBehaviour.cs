using System.Collections;
using UnityEngine;

public class MinigunBehaviour : MonoBehaviour, Messages.IUse
{
	[SkipSerialisation]
	public FirearmBehaviour FirearmBehaviour;

	private PhysicalBehaviour physicalBehaviour;

	private SpriteRenderer renderer;

	private MaterialPropertyBlock propertyBlock;

	private float rotationSpeed;

	private float rotation;

	[SkipSerialisation]
	public AudioSource RattleAudioSource;

	[SkipSerialisation]
	public AudioSource FireAudioSource;

	[SkipSerialisation]
	public AudioClip BarrelStart;

	[SkipSerialisation]
	public AudioClip BarrelLoop;

	[SkipSerialisation]
	public AudioClip BarrelStop;

	private float timeHeld;

	private bool directlyUsed;

	public float UpperTemperatureBound = 500f;

	private void Awake()
	{
		physicalBehaviour = GetComponent<PhysicalBehaviour>();
		renderer = GetComponent<SpriteRenderer>();
		propertyBlock = new MaterialPropertyBlock();
		renderer.GetPropertyBlock(propertyBlock);
	}

	public void Use(ActivationPropagation activation)
	{
		FirearmBehaviour.ResetIndex();
	}

	private IEnumerator SoundRoutine()
	{
		RattleAudioSource.Stop();
		RattleAudioSource.clip = BarrelStart;
		RattleAudioSource.Play();
		yield return new WaitForSeconds(BarrelStart.length);
		RattleAudioSource.clip = BarrelLoop;
		RattleAudioSource.loop = true;
		RattleAudioSource.Play();
		yield return new WaitUntil(() => rotationSpeed < 0.06f);
		RattleAudioSource.loop = false;
		RattleAudioSource.clip = BarrelStop;
		RattleAudioSource.Play();
		directlyUsed = false;
	}

	private void FixedUpdate()
	{
		if (physicalBehaviour.IsBeingUsedContinuously())
		{
			rotationSpeed *= 0.98f;
		}
		else
		{
			rotationSpeed *= 0.92f;
		}
	}

	private void Update()
	{
		rotation += rotationSpeed * Time.deltaTime * 60f;
		if (timeHeld > 0.4f)
		{
			if (!FireAudioSource.isPlaying)
			{
				FireAudioSource.Play();
			}
			if (FirearmBehaviour.ShotIndex == 0)
			{
				FirearmBehaviour.Shoot();
			}
			else
			{
				FirearmBehaviour.ShootContinuous();
			}
		}
		else if (FireAudioSource.isPlaying)
		{
			FireAudioSource.Stop();
		}
		if (physicalBehaviour.StartedBeingUsedContinuously())
		{
			StopAllCoroutines();
			StartCoroutine(SoundRoutine());
		}
		if (physicalBehaviour.IsBeingUsedContinuously())
		{
			timeHeld += Time.deltaTime;
			rotationSpeed = Mathf.Lerp(rotationSpeed, 0.15f, Utils.GetLerpFactorDeltaTime(0.99f, Time.deltaTime));
			return;
		}
		timeHeld = 0f;
		if (FireAudioSource.isPlaying)
		{
			FireAudioSource.Stop();
		}
	}

	private void OnWillRenderObject()
	{
		propertyBlock.SetFloat(ShaderProperties.Get("_Rotation"), rotation);
		float num = Mathf.Clamp01(physicalBehaviour.Temperature / UpperTemperatureBound);
		propertyBlock.SetFloat(ShaderProperties.Get("_Progress"), num * num);
		renderer.SetPropertyBlock(propertyBlock);
	}
}

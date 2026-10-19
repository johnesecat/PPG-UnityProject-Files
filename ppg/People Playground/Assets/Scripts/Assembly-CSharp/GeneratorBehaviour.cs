using NaughtyAttributes;
using UnityEngine;

public class GeneratorBehaviour : MonoBehaviour, Messages.IUse
{
	public ParticleSystem particles;

	public PhysicalBehaviour physicalBehaviour;

	public bool Activated;

	public float TargetCharge = 100f;

	public AudioSource generatorAudioSource;

	public float TargetEnergyReachSpeed = 1f;

	public bool Vibrate;

	[ShowIf("Vibrate")]
	public float VibrateForce;

	[ShowIf("Vibrate")]
	public float VibrateRotationSpeed;

	[HideInInspector]
	public float WarmupProgress;

	public bool InstantChargeChange;

	private SpriteRenderer renderer;

	private MaterialPropertyBlock propertyBlock;

	private Vector2 ForceVector
	{
		get
		{
			float f = Time.time * VibrateRotationSpeed;
			return new Vector2(Mathf.Cos(f), Mathf.Sin(f)) * VibrateForce;
		}
	}

	private void Awake()
	{
		renderer = GetComponent<SpriteRenderer>();
	}

	private void Start()
	{
		propertyBlock = new MaterialPropertyBlock();
		renderer.GetPropertyBlock(propertyBlock);
		UpdateActivation();
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			WarmupProgress = 0f;
			Activated = !Activated;
			UpdateActivation();
		}
	}

	private void UpdateActivation()
	{
		if (Activated)
		{
			if ((bool)generatorAudioSource)
			{
				generatorAudioSource.Play();
			}
			if ((bool)particles)
			{
				particles.Play();
			}
		}
		else
		{
			if ((bool)generatorAudioSource)
			{
				generatorAudioSource.Stop();
			}
			if ((bool)particles)
			{
				particles.Stop();
			}
		}
	}

	private void Update()
	{
		WarmupProgress = Mathf.Clamp01(WarmupProgress);
		if (Activated)
		{
			if (InstantChargeChange)
			{
				physicalBehaviour.Charge = Mathf.Max(physicalBehaviour.Charge, TargetCharge);
			}
			else
			{
				physicalBehaviour.Charge = Mathf.Max(physicalBehaviour.Charge, Mathf.Lerp(0f, TargetCharge, WarmupProgress));
			}
			WarmupProgress += Time.smoothDeltaTime * 60f * TargetEnergyReachSpeed;
		}
		else if (InstantChargeChange)
		{
			physicalBehaviour.Charge = 0f;
		}
		if ((bool)generatorAudioSource)
		{
			generatorAudioSource.volume = physicalBehaviour.Charge / TargetCharge * 0.2f;
		}
	}

	private void FixedUpdate()
	{
		if (Activated)
		{
			float num = TargetCharge / 4f;
			if (physicalBehaviour.Temperature < num)
			{
				physicalBehaviour.Temperature = Mathf.Lerp(physicalBehaviour.Temperature, num, 0.02f / physicalBehaviour.GetHeatCapacity());
			}
			if (Vibrate)
			{
				Vector2 relativeForce = ForceVector * WarmupProgress;
				physicalBehaviour.rigidbody.AddRelativeForce(relativeForce, ForceMode2D.Force);
			}
		}
	}

	private void OnDisable()
	{
		Activated = false;
		if ((bool)generatorAudioSource)
		{
			generatorAudioSource.Stop();
		}
		if ((bool)particles)
		{
			particles.Stop();
		}
	}

	private void OnWillRenderObject()
	{
		if (base.enabled)
		{
			float value = physicalBehaviour.Charge / TargetCharge;
			propertyBlock.SetFloat(ShaderProperties.Get("_Progress"), value);
			renderer.SetPropertyBlock(propertyBlock);
		}
	}
}

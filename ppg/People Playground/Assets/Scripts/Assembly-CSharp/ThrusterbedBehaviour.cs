using UnityEngine;

public class ThrusterbedBehaviour : MonoBehaviour, Messages.IUse
{
	[SkipSerialisation]
	public AnimationCurve StartupStrengthCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public float StartupDuration = 3f;

	public bool Activated;

	public float ThrustingForce = 320f;

	public Vector2 RelativeDirection = Vector2.up;

	[SkipSerialisation]
	public ParticleSystem effect;

	[SkipSerialisation]
	public AudioSource lowAudio;

	[SkipSerialisation]
	public AudioSource highAudio;

	[SkipSerialisation]
	public AudioClip IgnitionSound;

	[SkipSerialisation]
	public AudioClip ExtinguishSound;

	public bool DoBurnOthers = true;

	public float TemperatureTarget = 600f;

	public int BurnRayCount = 3;

	public float BurnRayWidth = 1f;

	public float BurnRayLength = 5f;

	public Vector2 BurnRayOffset;

	public Vector2 BurnRayPerpendicular = new Vector2(0f, 1f);

	public LayerMask LayersToBurn;

	private float time;

	private Rigidbody2D rb;

	private PhysicalBehaviour physicalBehaviour;

	private static readonly RaycastHit2D[] buffer = new RaycastHit2D[32];

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		physicalBehaviour = GetComponent<PhysicalBehaviour>();
	}

	private void Start()
	{
		UpdateActivated();
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			Activated = !Activated;
			UpdateActivated();
		}
	}

	private void UpdateActivated()
	{
		UpdateParticleSystem(0f);
		UpdateAudio(0f);
		if (Activated)
		{
			if ((bool)IgnitionSound)
			{
				lowAudio.PlayOneShot(IgnitionSound);
			}
			effect.Play(withChildren: false);
			lowAudio.Play();
			highAudio.Play();
		}
		else
		{
			effect.Stop(withChildren: false);
			lowAudio.Stop();
			highAudio.Stop();
			if ((bool)ExtinguishSound)
			{
				physicalBehaviour.PlayClipOnce(ExtinguishSound);
			}
		}
		time = 0f;
	}

	private void FixedUpdate()
	{
		if (Activated)
		{
			time += Time.deltaTime;
			float num = StartupStrengthCurve.Evaluate(Mathf.Clamp01(time));
			UpdateParticleSystem(num);
			if (DoBurnOthers)
			{
				BurnOthers(num);
			}
			Thrust(num);
			UpdateAudio(num);
			physicalBehaviour.Temperature = Mathf.Lerp(physicalBehaviour.Temperature, TemperatureTarget + physicalBehaviour.Charge, 0.005f / physicalBehaviour.GetHeatCapacity());
			CameraShakeBehaviour.main.Shake((num + physicalBehaviour.Charge * 0.1f) * 0.1f, base.transform.position);
		}
	}

	private void UpdateAudio(float progress)
	{
		highAudio.volume = progress * physicalBehaviour.Charge;
		highAudio.minDistance = 10f + progress * physicalBehaviour.Charge;
		lowAudio.volume = progress;
	}

	private void Shocked(Zap zap)
	{
		if (!Activated)
		{
			Use(null);
		}
	}

	private void Thrust(float progress)
	{
		float num = Mathf.Max(1f, physicalBehaviour.Charge * 2f);
		rb.AddForce(num * progress * ThrustingForce * base.transform.TransformVector(RelativeDirection));
	}

	private void BurnOthers(float progress)
	{
		float num = physicalBehaviour.Charge * 0.2f * progress;
		Vector2 vector = base.transform.TransformVector(RelativeDirection);
		Vector2 vector2 = vector * num;
		Vector2 vector3 = BurnRayWidth / (float)Mathf.Max(1, BurnRayCount - 1) * BurnRayPerpendicular;
		for (int i = 0; i < BurnRayCount; i++)
		{
			Vector2 vector4 = base.transform.TransformPoint(BurnRayOffset + vector3 * i - BurnRayWidth / 2f * BurnRayPerpendicular);
			int num2 = Physics2D.LinecastNonAlloc(vector4, vector4 + vector * (0f - BurnRayLength) - vector2, buffer, LayersToBurn);
			if (num2 > 0)
			{
				BurnInBuffer(progress, num2);
			}
		}
	}

	private void BurnInBuffer(float progress, int count)
	{
		Vector3 vector = base.transform.TransformVector(RelativeDirection);
		for (int i = 0; i < count; i++)
		{
			Collider2D collider = buffer[i].collider;
			if (collider.transform == base.transform)
			{
				continue;
			}
			Rigidbody2D attachedRigidbody = collider.attachedRigidbody;
			if ((bool)attachedRigidbody)
			{
				attachedRigidbody.AddForce(vector * progress * (-15f - physicalBehaviour.Charge * 2f));
				attachedRigidbody.AddTorque(Random.Range(-0.1f, 0.1f));
			}
			if (Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collider.transform, out var value))
			{
				value.Temperature = Mathf.Lerp(physicalBehaviour.Temperature, TemperatureTarget + physicalBehaviour.Charge, 0.01f / value.GetHeatCapacity());
				value.BurnProgress += 0.02f * Random.value + physicalBehaviour.Charge * 0.01f;
				if ((double)Random.value > 0.95)
				{
					value.Ignite();
				}
			}
		}
	}

	private void UpdateParticleSystem(float progress)
	{
		ParticleSystem.MainModule main = effect.main;
		main.startSpeedMultiplier = progress * Random.Range(5f, 12f) + physicalBehaviour.Charge * 0.5f;
		float num = Mathf.Max(0.1f, physicalBehaviour.Charge / 8f);
		main.startColor = 4f * new Color(num, num - physicalBehaviour.Charge * 0.2f, num + physicalBehaviour.Charge * 0.6f) * Mathf.Max(1f, physicalBehaviour.Charge * 5f);
		ParticleSystem.NoiseModule noise = effect.noise;
		noise.strengthMultiplier = progress * 2f;
	}

	private void OnDisable()
	{
		Activated = false;
		effect.Stop(withChildren: false);
		lowAudio.Stop();
		highAudio.Stop();
	}
}

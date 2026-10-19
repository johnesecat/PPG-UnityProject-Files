using UnityEngine;

public class IonThrusterBehaviour : MonoBehaviour, Messages.IUse
{
	[SkipSerialisation]
	public AudioClip ActivationSound;

	[SkipSerialisation]
	public AudioSource ThrusterAudioSource;

	[SkipSerialisation]
	public ParticleSystem FX;

	[SkipSerialisation]
	public Vector2 LocalDirection;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public float Thrust = 10f;

	private ParticleSystem.MainModule vfxMain;

	private ParticleSystem.EmissionModule vfxEmission;

	public bool Activated;

	private void Awake()
	{
		vfxMain = FX.main;
		vfxEmission = FX.emission;
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			Activated = !Activated;
			UpdateEffectState();
			if (Activated)
			{
				CameraShakeBehaviour.main.Shake(2f, base.transform.position);
				ThrusterAudioSource.PlayOneShot(ActivationSound);
			}
		}
	}

	private void Start()
	{
		UpdateEffectState();
	}

	private void OnEnable()
	{
		UpdateEffectState();
	}

	private void OnDisable()
	{
		FX.Stop();
		ThrusterAudioSource.Stop();
	}

	private void FixedUpdate()
	{
		UpdateEffectState();
		if (Activated)
		{
			PhysicalBehaviour.rigidbody.AddForce(GetGlobalThrustVector(), ForceMode2D.Force);
		}
	}

	private Vector2 GetGlobalThrustVector()
	{
		float num = ((!(base.transform.lossyScale.x > 0f)) ? 1 : (-1));
		return (Thrust + PhysicalBehaviour.Charge * 4f) * num * base.transform.TransformDirection(LocalDirection);
	}

	private void UpdateEffectState()
	{
		if (Activated)
		{
			if (!FX.isEmitting)
			{
				FX.Play();
			}
			if (!ThrusterAudioSource.isPlaying)
			{
				ThrusterAudioSource.Play();
			}
			float num = PhysicalBehaviour.Charge * 0.1f;
			vfxMain.startSpeedMultiplier = 5f + num;
			vfxEmission.rateOverTimeMultiplier = 15f + num;
		}
		else
		{
			if (FX.isEmitting)
			{
				FX.Stop();
			}
			if (ThrusterAudioSource.isPlaying)
			{
				ThrusterAudioSource.Stop();
			}
		}
	}
}

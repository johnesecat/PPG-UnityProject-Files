using UnityEngine;

public class FireworksBehaviour : MonoBehaviour, Messages.IUse, Messages.IShot
{
	[SkipSerialisation]
	public AudioSource Audio;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public ParticleSystem ParticleSystem;

	[SkipSerialisation]
	public GameObject[] Bursts;

	[SkipSerialisation]
	public GameObject[] LowChanceBursts;

	[SkipSerialisation]
	public GameObject FireworksNuke;

	[SkipSerialisation]
	public Transform Killzone;

	public Vector2 Thrust;

	public float Range = 5f;

	public float TimeUntilBurst;

	public float TimeUntilBurstRandomisation = 0.5f;

	private bool alreadyShot;

	[HideInInspector]
	public float RandomisedTimeUntilBurst;

	[HideInInspector]
	public bool Activated;

	[HideInInspector]
	public float Life;

	private void Awake()
	{
		RandomisedTimeUntilBurst = TimeUntilBurst + Random.Range(0f - TimeUntilBurstRandomisation, TimeUntilBurstRandomisation) * 0.5f;
	}

	private void Start()
	{
		UpdateActivation();
	}

	public void Use(ActivationPropagation activation)
	{
		if (!Activated)
		{
			Activated = true;
			Audio.Play();
			UpdateActivation();
		}
	}

	private void UpdateActivation()
	{
		if (Activated)
		{
			ParticleSystem.Play(withChildren: false);
		}
		else
		{
			ParticleSystem.Stop(withChildren: false);
		}
	}

	private void FixedUpdate()
	{
		if (PhysicalBehaviour.BurnProgress > 0.1f)
		{
			Life = float.NegativeInfinity;
			Activated = true;
			Explode();
		}
		else if (Activated)
		{
			PhysicalBehaviour.rigidbody.AddRelativeForce(Thrust * GetScale(), ForceMode2D.Force);
		}
	}

	private float GetScale()
	{
		return Mathf.Abs(base.transform.lossyScale.x * base.transform.lossyScale.y);
	}

	private void Update()
	{
		if (Activated)
		{
			Life += Time.deltaTime;
			if (Life > RandomisedTimeUntilBurst)
			{
				Explode();
			}
		}
		else
		{
			Killzone.transform.localScale = 2f * GetScale() * Range * Vector3.one;
		}
	}

	private void Explode()
	{
		float scale = GetScale();
		if (scale > 60f)
		{
			Object.Instantiate(FireworksNuke, base.transform.position, Quaternion.identity);
			Object.Destroy(base.gameObject);
			return;
		}
		GameObject obj = Object.Instantiate((((double)Random.value > 0.999) ? LowChanceBursts : Bursts).PickRandom(), base.transform.position, Quaternion.identity);
		obj.transform.localScale = Vector3.one * scale;
		obj.GetComponent<ExplosiveBehaviour>().Range = Range * scale;
		ParticleSystem.MainModule main = obj.GetComponent<ParticleSystem>().main;
		main.simulationSpeed = 1f / scale;
		Object.Destroy(base.gameObject);
		Object.Destroy(Killzone.gameObject);
	}

	public void Shot(Shot shot)
	{
		if (!alreadyShot)
		{
			alreadyShot = true;
			Life = float.NegativeInfinity;
			Activated = true;
			Explode();
		}
	}
}

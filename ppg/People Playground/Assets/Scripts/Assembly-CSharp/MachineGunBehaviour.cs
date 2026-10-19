using NaughtyAttributes;
using UnityEngine;

public class MachineGunBehaviour : MonoBehaviour, Messages.IUse
{
	[SkipSerialisation]
	public ParticleSystem Effect;

	[SkipSerialisation]
	public GameObject ImpactEffect;

	[SkipSerialisation]
	public GameObject RicochetEffect;

	[SkipSerialisation]
	public AudioClip[] Sounds;

	[SkipSerialisation]
	public AudioSource AudioSource;

	[SkipSerialisation]
	public Vector2 barrelPosition;

	[SkipSerialisation]
	public Vector2 barrelDirection;

	[SkipSerialisation]
	public Cartridge Cartridge;

	public bool ExplosiveRounds;

	[ShowIf("ExplosiveRounds")]
	public ExplosionCreator.ExplosionParameters ExplosionSettings;

	public float FireRateMultiplier = 1f;

	private float t;

	private Rigidbody2D rb;

	private PhysicalBehaviour phys;

	public float ShockwaveIntensity = 0.25f;

	public bool Automatic = true;

	[SkipSerialisation]
	public DistantSoundBehaviour.SoundType DistantSound;

	public BallisticsEmitter BallisticsEmitter { get; private set; }

	public Vector2 BarrelPosition => base.transform.TransformPoint(barrelPosition);

	public Vector2 BarrelDirection => base.transform.TransformDirection(barrelDirection) * base.transform.localScale.x;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		phys = GetComponent<PhysicalBehaviour>();
	}

	private void Start()
	{
		if (base.transform.root.localScale.x < 0f)
		{
			Effect.transform.eulerAngles = new Vector3(0f, 0f, 180f);
			Effect.transform.localScale = new Vector3(-0.8f, 0.8f, 0.8f);
		}
		BallisticsEmitter = new BallisticsEmitter(this, Cartridge);
		BallisticsEmitter.MaxRange = 5f;
		BallisticsEmitter.TracerWidth = 0.1f;
		BallisticsEmitter.DoTraversal = true;
		BallisticsEmitter.BulletEntryCallback = OnEntry;
		BallisticsEmitter.BulletExitCallback = OnExit;
		BallisticsEmitter.BulletRicochetCallback = OnRicochet;
	}

	private void Update()
	{
		if (Automatic && phys.IsBeingUsedContinuously())
		{
			if (t > Mathf.Max(0.05f, 0.2f / Mathf.Clamp(phys.Charge * 0.1f, 1f, 8f)))
			{
				t = 0f;
				Shoot();
			}
			t += Time.deltaTime * FireRateMultiplier;
		}
	}

	private void OnEntry(BallisticsEmitter.CallbackParams info)
	{
		Object.Instantiate(ImpactEffect, info.Position, Quaternion.identity).transform.right = info.Direction * 1f;
	}

	private void OnExit(BallisticsEmitter.CallbackParams info)
	{
		GameObject obj = Object.Instantiate(ImpactEffect, info.Position, Quaternion.identity);
		obj.transform.localScale = new Vector2(3f, 3f);
		obj.transform.right = info.Direction;
	}

	private void OnRicochet(BallisticsEmitter.CallbackParams info)
	{
		Object.Instantiate(RicochetEffect, info.Position, Quaternion.identity);
	}

	public void Use(ActivationPropagation activation)
	{
		if (Automatic)
		{
			t = 0f;
		}
		Shoot();
	}

	private void Shoot()
	{
		BallisticsEmitter.ExplosiveRound = ExplosiveRounds;
		if (ExplosiveRounds)
		{
			BallisticsEmitter.ExplosiveRoundParams = ExplosionSettings;
		}
		AudioSource.PlayOneShot(Sounds.PickRandom());
		rb.AddForceAtPosition(BarrelDirection * -15f, BarrelPosition, ForceMode2D.Impulse);
		Effect.Play();
		if (ShockwaveIntensity > float.Epsilon)
		{
			ExplosionCreator.CreatePulseExplosion(BarrelPosition, 3f * ShockwaveIntensity, 4f * ShockwaveIntensity, soundAndEffects: false, breakObjects: false);
		}
		BallisticsEmitter.Emit(BarrelPosition, BarrelDirection.normalized);
		if (DistantSound != 0)
		{
			DistantSoundBehaviour.Instance.Play(DistantSound, base.transform.position, 0.5f);
		}
	}

	private void OnDestroy()
	{
		BallisticsEmitter.Dispose();
	}
}

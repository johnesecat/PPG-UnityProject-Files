using UnityEngine;

public class SentryTurretBehaviour : MonoBehaviour, Messages.IOnEMPHit, Messages.IShot
{
	[SkipSerialisation]
	public SentryTurretControllable turretGun;

	[Space]
	[SkipSerialisation]
	public AudioClip[] TargetAcquired;

	[SkipSerialisation]
	public AudioClip[] TargetLost;

	[SkipSerialisation]
	public AudioClip[] Flying;

	[SkipSerialisation]
	public AudioClip[] ShotAt;

	[SkipSerialisation]
	public AudioClip[] OnFire;

	[SkipSerialisation]
	public AudioClip[] Tipped;

	[SkipSerialisation]
	public AudioClip[] Retiring;

	[Space]
	[SkipSerialisation]
	public AudioClip Alarm;

	[SkipSerialisation]
	public AudioClip Ping;

	[SkipSerialisation]
	public AudioClip Active;

	[SkipSerialisation]
	public AudioClip PistonDeploy;

	[SkipSerialisation]
	public AudioClip PistonRetract;

	[Space]
	[SkipSerialisation]
	public LayerMask BlockingLayers;

	[Space]
	[SkipSerialisation]
	public AudioSource AudioSource;

	public PhysicalBehaviour phys;

	private Rigidbody2D rb;

	private Vector2 oldVelocity;

	public SentryTurretAIState State;

	private float soundTimer;

	private float deathTimer;

	[HideInInspector]
	public float timeStartedSearching;

	[HideInInspector]
	public bool disabled;

	public AutomaticSentryController Controller = new AutomaticSentryController();

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		phys = GetComponent<PhysicalBehaviour>();
		State = SentryTurretAIState.Idle;
		Controller.MaxAngle = 100f;
	}

	private void Start()
	{
		if (disabled)
		{
			Disable();
		}
	}

	private void Update()
	{
		Controller.LayerMask = BlockingLayers;
		soundTimer += Time.deltaTime;
		if (State == SentryTurretAIState.Panicking)
		{
			deathTimer += Time.deltaTime;
		}
		else
		{
			deathTimer = 0f;
		}
	}

	public void OnEMPHit()
	{
		Explode();
	}

	private void FixedUpdate()
	{
		if (phys.OnFire && Random.value > 0.9f && !AudioSource.isPlaying)
		{
			PlaySound(OnFire);
		}
		if (phys.BurnProgress > 0.9f || phys.Temperature > 2500f || phys.Charge > 500f)
		{
			Explode();
		}
		if (disabled)
		{
			State = SentryTurretAIState.Idle;
			return;
		}
		Vector3 vector = turretGun.transform.right * Mathf.Sign(turretGun.transform.lossyScale.x);
		float motorSpeed;
		switch (State)
		{
		case SentryTurretAIState.Idle:
			Controller.GetTargetMotorSpeed(base.transform, turretGun.BarrelPosition, vector, out motorSpeed);
			if (Controller.HasTarget)
			{
				Activate();
			}
			break;
		case SentryTurretAIState.Firing:
			if (!Controller.HasTarget)
			{
				timeStartedSearching = Time.time;
				Searching();
			}
			else
			{
				LimbBehaviour target = Controller.Target;
				Controller.GetTargetMotorSpeed(base.transform, turretGun.BarrelPosition, vector, out motorSpeed);
				if (!Controller.HasTarget && !target.IsConsideredAlive)
				{
					State = SentryTurretAIState.Idle;
					PlaySound(Retiring);
					AudioSource.PlayOneShot(PistonRetract);
				}
			}
			if (soundTimer > 0.135f)
			{
				AudioSource.PlayOneShot(Alarm, 0.2f);
				soundTimer = 0f;
			}
			break;
		case SentryTurretAIState.Searching:
			if (soundTimer > 1.1f)
			{
				AudioSource.PlayOneShot(Ping);
				soundTimer = 0f;
			}
			if (Time.time - timeStartedSearching > 4f)
			{
				State = SentryTurretAIState.Idle;
				PlaySound(Retiring);
				AudioSource.PlayOneShot(PistonRetract);
			}
			Controller.GetTargetMotorSpeed(base.transform, turretGun.BarrelPosition, vector, out motorSpeed);
			if (Controller.HasTarget)
			{
				Activate();
			}
			break;
		case SentryTurretAIState.Panicking:
			if (soundTimer > 0.135f)
			{
				AudioSource.PlayOneShot(Alarm, 0.2f);
				soundTimer = 0f;
			}
			break;
		}
		if ((rb.velocity - oldVelocity).sqrMagnitude > 110f && oldVelocity.sqrMagnitude < rb.velocity.sqrMagnitude)
		{
			Panic();
		}
		if (Vector2.Dot(base.transform.up, Physics2D.gravity.normalized) > -0.4f)
		{
			State = SentryTurretAIState.Panicking;
			if (deathTimer > 1f)
			{
				PlaySound(Tipped);
				Disable();
			}
		}
		else if (deathTimer > 1f)
		{
			State = SentryTurretAIState.Idle;
		}
		oldVelocity = rb.velocity;
	}

	private void Explode()
	{
		CameraShakeBehaviour.main.Shake(10f, base.transform.position);
		Object.Instantiate(Resources.Load<GameObject>("Prefabs/Explosion"), base.transform.position, Quaternion.identity);
		GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>("Prefabs/SentryTurretDebris"), base.transform.position, base.transform.rotation);
		gameObject.transform.localScale = base.transform.localScale;
		DestroyableBehaviour.ApplyAncestorProperties(rb, phys, default(Vector2), gameObject);
		DestroyableBehaviour.ReplaceUndoEntryWith(base.transform, gameObject);
		Object.Destroy(base.gameObject);
	}

	private void Disable()
	{
		disabled = true;
		turretGun.Disable();
	}

	public void Shot(Shot shot)
	{
		if (!disabled)
		{
			PlaySound(ShotAt);
		}
	}

	private void Panic()
	{
		PlaySound(Flying);
	}

	private void Searching()
	{
		State = SentryTurretAIState.Searching;
		if ((double)Random.value > 0.2)
		{
			PlaySound(TargetLost);
		}
	}

	private void Activate()
	{
		State = SentryTurretAIState.Firing;
		AudioSource.PlayOneShot(Active);
		AudioSource.PlayOneShot(PistonDeploy);
		if ((double)Random.value > 0.2)
		{
			PlaySound(TargetAcquired);
		}
	}

	private void PlaySound(AudioClip[] set)
	{
		AudioSource.clip = set.PickRandom();
		AudioSource.Play();
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnergySwordBehaviour : MonoBehaviour, Messages.IOnBeforeSerialise, Messages.IOnAfterDeserialise, Messages.IUse
{
	public class SoftConnection
	{
		[NonSerialized]
		public FrictionJoint2D joint;

		public PhysicalBehaviour phys;

		public Collider2D coll;

		[NonSerialized]
		public bool shouldBeDeleted;

		public SoftConnection(FrictionJoint2D joint, PhysicalBehaviour phys, Collider2D coll)
		{
			this.joint = joint;
			this.phys = phys;
			this.coll = coll;
			shouldBeDeleted = false;
		}
	}

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public Collider2D MainCollider;

	[SkipSerialisation]
	public Collider2D BladeCollider;

	[SkipSerialisation]
	public LineSegment LocalBladeLine;

	[SkipSerialisation]
	public DamagableMachineryBehaviour DamagableMachinery;

	[SkipSerialisation]
	public GameObject BladeObject;

	[SkipSerialisation]
	public float SpeedCutThreshold = 1f;

	[SkipSerialisation]
	public LayerMask LayerMask;

	[SkipSerialisation]
	public float MinSoftness;

	[SkipSerialisation]
	public ParticleSystem HitParticle;

	[SkipSerialisation]
	public ParticleSystem BoilingVFX;

	[SkipSerialisation]
	public ParticleSystem RainParticles;

	[SkipSerialisation]
	public AudioClip[] ImpactSound;

	[SkipSerialisation]
	public AudioClip PowerOn;

	[SkipSerialisation]
	public AudioClip PowerOff;

	[SkipSerialisation]
	public float Damage = 256f;

	[SkipSerialisation]
	public Transform TipOfBlade;

	[SkipSerialisation]
	public GameObject MotionBlur;

	[SkipSerialisation]
	public AudioSource ImpactSource;

	[SkipSerialisation]
	public AudioSource LoopSource;

	[SkipSerialisation]
	public float BladeTemperature = 900f;

	public bool Activated;

	private float impactSoundHeat;

	private static readonly Collider2D[] buffer = new Collider2D[16];

	private static readonly RaycastHit2D[] rayBuffer = new RaycastHit2D[16];

	private readonly Dictionary<PhysicalBehaviour, SoftConnection> softConnections = new Dictionary<PhysicalBehaviour, SoftConnection>();

	private MapConfig currentMapConfig;

	[HideInInspector]
	public Guid[] SerialisableVictims = new Guid[0];

	private void Awake()
	{
		BladeObject.SetActive(value: false);
		MotionBlur.SetActive(value: false);
	}

	private void Start()
	{
		currentMapConfig = UnityEngine.Object.FindObjectOfType<MapConfig>();
		SetActivation(playActivationSounds: false);
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			Activated = !Activated;
			SetActivation();
			if (Activated)
			{
				PhysicalBehaviour.rigidbody.AddRelativeForce(Vector2.down * 0.1f, ForceMode2D.Impulse);
			}
		}
	}

	private void SetActivation(bool playActivationSounds = true)
	{
		if ((bool)MotionBlur)
		{
			MotionBlur.SetActive(Activated);
		}
		BladeObject.SetActive(Activated);
		if (Activated)
		{
			BoilingVFX.Play(withChildren: true);
			LoopSource.Play();
			if (playActivationSounds)
			{
				LoopSource.PlayOneShot(PowerOn, 2f);
			}
			return;
		}
		LoopSource.Stop();
		ImpactSource.Stop();
		HitParticle.Stop();
		BoilingVFX.Stop();
		RainParticles.Stop();
		foreach (KeyValuePair<PhysicalBehaviour, SoftConnection> softConnection in softConnections)
		{
			UnityEngine.Object.Destroy(softConnection.Value.joint);
		}
		if (playActivationSounds)
		{
			LoopSource.PlayOneShot(PowerOff, 2f);
		}
	}

	private void OnDisable()
	{
		if (Activated)
		{
			Activated = false;
			SetActivation();
		}
	}

	private void Update()
	{
		if (!Activated)
		{
			return;
		}
		if (currentMapConfig.Settings.Rain && (bool)currentMapConfig.RainPrefab)
		{
			if (!RainParticles.isEmitting)
			{
				RainParticles.Play();
			}
		}
		else if (RainParticles.isEmitting)
		{
			RainParticles.Stop();
		}
		impactSoundHeat -= Time.deltaTime;
		if (impactSoundHeat < float.Epsilon)
		{
			impactSoundHeat = 0f;
		}
	}

	private void FixedUpdate()
	{
		ProcessSoftConnections(shouldCut: true, 15f);
	}

	private void ProcessSoftConnections(bool shouldCut = true, float connectionStrength = 2f)
	{
		int num = BladeCollider.OverlapCollider(new ContactFilter2D
		{
			layerMask = LayerMask,
			useLayerMask = true
		}, buffer);
		foreach (KeyValuePair<PhysicalBehaviour, SoftConnection> softConnection in softConnections)
		{
			softConnection.Value.shouldBeDeleted = true;
		}
		float num2 = PhysicalBehaviour.Charge * 0.1f;
		float num3 = Time.fixedDeltaTime * 0.02f;
		bool flag = false;
		for (int i = 0; i < num; i++)
		{
			Collider2D collider2D = buffer[i];
			if (!(collider2D.transform.root != base.transform.root) || !Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collider2D.transform, out var value))
			{
				continue;
			}
			if (softConnections.TryGetValue(value, out var value2))
			{
				value2.shouldBeDeleted = !value2.coll || !value2.joint || !value2.phys;
				if (!value2.shouldBeDeleted)
				{
					flag = true;
					Vector3 position = GetHitPoint(collider2D);
					value2.joint.anchor = value2.phys.transform.InverseTransformPoint(position);
					value2.joint.maxForce = connectionStrength;
					value2.joint.maxTorque = connectionStrength;
					if (value2.phys.SimulateTemperature)
					{
						value2.phys.Temperature = Mathf.Lerp(value2.phys.Temperature, BladeTemperature, 0.00025f / value2.phys.GetHeatCapacity());
					}
					value2.phys.BurnProgress += num3;
					value2.phys.SendMessage("Damage", Damage * Time.fixedDeltaTime, SendMessageOptions.DontRequireReceiver);
					HitParticle.transform.position = position;
					if (shouldCut && UnityEngine.Random.value > 0.995f - num2)
					{
						value.BroadcastMessage("Slice", SendMessageOptions.DontRequireReceiver);
					}
				}
			}
			else if (shouldCut && !Physics2D.GetIgnoreCollision(collider2D, MainCollider))
			{
				SoftConnect(value, collider2D, connectionStrength);
			}
		}
		if (flag)
		{
			if (Activated)
			{
				if (!HitParticle.isPlaying)
				{
					HitParticle.Play();
				}
				if (!ImpactSource.isPlaying)
				{
					ImpactSource.Play();
					ImpactSource.time = ImpactSource.clip.length * UnityEngine.Random.value;
				}
			}
		}
		else
		{
			if (HitParticle.isPlaying)
			{
				HitParticle.Stop();
			}
			if (ImpactSource.isPlaying)
			{
				ImpactSource.Stop();
			}
		}
		while (true)
		{
			KeyValuePair<PhysicalBehaviour, SoftConnection>? keyValuePair = null;
			foreach (KeyValuePair<PhysicalBehaviour, SoftConnection> softConnection2 in softConnections)
			{
				if (softConnection2.Value.shouldBeDeleted)
				{
					keyValuePair = softConnection2;
					if ((bool)softConnection2.Value.coll)
					{
						IgnoreCollisionStackController.RequestDontIgnoreCollision(softConnection2.Value.coll, BladeCollider);
					}
					break;
				}
			}
			if (keyValuePair.HasValue)
			{
				KeyValuePair<PhysicalBehaviour, SoftConnection> value3 = keyValuePair.Value;
				if ((bool)value3.Value.joint)
				{
					UnityEngine.Object.Destroy(value3.Value.joint);
				}
				softConnections.Remove(value3.Key);
				continue;
			}
			break;
		}
	}

	private void OnDestroy()
	{
		UnityEngine.Object.Destroy(MotionBlur);
		foreach (KeyValuePair<PhysicalBehaviour, SoftConnection> softConnection in softConnections)
		{
			IgnoreCollisionStackController.RequestDontIgnoreCollision(softConnection.Value.coll, BladeCollider);
			UnityEngine.Object.Destroy(softConnection.Value.joint);
		}
	}

	private Vector2 GetHitPoint(Collider2D coll)
	{
		Vector3 vector = base.transform.TransformPoint(LocalBladeLine.A);
		Vector3 vector2 = base.transform.TransformPoint(LocalBladeLine.B) - vector;
		int num = Physics2D.CircleCastNonAlloc(vector, 0.05f, vector2, rayBuffer, vector2.magnitude);
		for (int i = 0; i < num; i++)
		{
			RaycastHit2D raycastHit2D = rayBuffer[i];
			if (raycastHit2D.collider == coll)
			{
				return raycastHit2D.point;
			}
		}
		Vector2 position = coll.ClosestPoint(base.transform.position);
		return BladeCollider.ClosestPoint(position);
	}

	public void OnBeforeSerialise()
	{
		SerialisableVictims = softConnections.Where(delegate(KeyValuePair<PhysicalBehaviour, SoftConnection> p)
		{
			SoftConnection value2 = p.Value;
			return !value2.shouldBeDeleted && (bool)value2.phys && (bool)value2.joint && (bool)value2.coll;
		}).Select(delegate(KeyValuePair<PhysicalBehaviour, SoftConnection> p)
		{
			SoftConnection value = p.Value;
			try
			{
				return value.phys.GetComponent<SerialisableIdentity>().UniqueIdentity;
			}
			catch (Exception)
			{
				Debug.LogWarning("Energy sword victim with invalid or non-existent ID");
				return default(Guid);
			}
		}).ToArray();
	}

	public void OnAfterDeserialise(List<GameObject> gameobjects)
	{
		IEnumerable<SerialisableIdentity> source = gameobjects.SelectMany((GameObject c) => c.GetComponentsInChildren<SerialisableIdentity>());
		int i;
		for (i = 0; i < SerialisableVictims.Length; i++)
		{
			SerialisableIdentity serialisableIdentity = source.FirstOrDefault((SerialisableIdentity s) => s.UniqueIdentity == SerialisableVictims[i]);
			if (!serialisableIdentity || !serialisableIdentity.TryGetComponent<PhysicalBehaviour>(out var component))
			{
				Debug.LogWarning($"Energy sword victim with ID {SerialisableVictims[i]} does not exist");
			}
			else if (!component || component == null)
			{
				Debug.LogWarning("Stab victim is null");
			}
			else
			{
				SoftConnect(component, component.GetComponent<Collider2D>(), 2f);
			}
		}
	}

	private void SoftConnect(PhysicalBehaviour otherPhys, Collider2D coll, float connectionStrength)
	{
		if ((bool)otherPhys && (bool)coll)
		{
			if (impactSoundHeat <= float.Epsilon)
			{
				LoopSource.PlayOneShot(ImpactSound.PickRandom(), 1.5f);
				impactSoundHeat = 0.5f;
			}
			Vector2 hitPoint = GetHitPoint(coll);
			FrictionJoint2D frictionJoint2D = otherPhys.gameObject.AddComponent<FrictionJoint2D>();
			frictionJoint2D.autoConfigureConnectedAnchor = true;
			frictionJoint2D.enableCollision = true;
			frictionJoint2D.maxForce = connectionStrength;
			frictionJoint2D.connectedBody = PhysicalBehaviour.rigidbody;
			frictionJoint2D.maxTorque = connectionStrength;
			frictionJoint2D.anchor = otherPhys.transform.InverseTransformPoint(hitPoint);
			IgnoreCollisionStackController.RequestIgnoreCollision(coll, BladeCollider);
			softConnections.Add(otherPhys, new SoftConnection(frictionJoint2D, otherPhys, coll));
		}
	}
}

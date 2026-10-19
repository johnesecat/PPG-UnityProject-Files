using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChainsawBehaviour : MonoBehaviour, Messages.IOnBeforeSerialise, Messages.IOnAfterDeserialise, Messages.IUse
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
	public Collider2D SawCollider;

	[SkipSerialisation]
	public AudioSource IdleLoop;

	[SkipSerialisation]
	public AudioSource RevLoop;

	[SkipSerialisation]
	public ParticleSystem OverchargeSystem;

	[SkipSerialisation]
	public AudioClip Rev;

	[SkipSerialisation]
	public SpriteRenderer Saw;

	[SkipSerialisation]
	public DamagableMachineryBehaviour DamagableMachinery;

	[HideInInspector]
	public Guid[] SerialisableVictims = new Guid[0];

	[SkipSerialisation]
	public float RotationSpeedDecay = 0.9f;

	[SkipSerialisation]
	public float UseRotationSpeedAddition = 15f;

	[SkipSerialisation]
	public float JitterIntensity = 0.005f;

	[SkipSerialisation]
	public float JitterSpeed = 0.1f;

	[SkipSerialisation]
	public float MinSoftness = 0.5f;

	[SkipSerialisation]
	public float SpeedCutThreshold = 1f;

	[SkipSerialisation]
	public float Damage = 25f;

	[SkipSerialisation]
	public LayerMask LayerMask;

	private MaterialPropertyBlock propertyBlock;

	private float rotationSpeed;

	private float chainPos;

	private float seed;

	private readonly Collider2D[] buffer = new Collider2D[8];

	private int bufferLength;

	private readonly Dictionary<PhysicalBehaviour, SoftConnection> softConnections = new Dictionary<PhysicalBehaviour, SoftConnection>();

	[SkipSerialisation]
	public float CurrentSpeed => rotationSpeed;

	private Vector2 ForceVector
	{
		get
		{
			float f = Time.time * (0f - (85f + PhysicalBehaviour.Charge));
			return (PhysicalBehaviour.Charge + 1f) * 2f * new Vector2(Mathf.Cos(f), Mathf.Sin(f)) * rotationSpeed;
		}
	}

	private void Start()
	{
		seed = UnityEngine.Random.value * 100f;
		propertyBlock = new MaterialPropertyBlock();
		Saw.GetPropertyBlock(propertyBlock);
		if (!DamagableMachinery.Destroyed)
		{
			IdleLoop.Play();
		}
	}

	private void FixedUpdate()
	{
		rotationSpeed *= RotationSpeedDecay;
		bool flag = rotationSpeed > SpeedCutThreshold;
		if (rotationSpeed > 0.1f && !RevLoop.isPlaying)
		{
			RevLoop.Play();
		}
		else if (rotationSpeed <= 0.1f && RevLoop.isPlaying)
		{
			RevLoop.Stop();
		}
		if (RevLoop.isPlaying)
		{
			RevLoop.volume = Mathf.Clamp01(rotationSpeed);
		}
		ProcessSoftConnections(flag, flag ? 2 : 500);
		SawCollider.isTrigger = false;
		if (flag && PhysicalBehaviour.Charge > 1f && !OverchargeSystem.isPlaying)
		{
			OverchargeSystem.Play();
		}
		else if ((!flag || PhysicalBehaviour.Charge <= 5f) && OverchargeSystem.isPlaying)
		{
			OverchargeSystem.Stop();
		}
		PhysicalBehaviour.rigidbody.AddRelativeForce(ForceVector, ForceMode2D.Force);
	}

	private void ProcessSoftConnections(bool shouldSaw = true, float connectionStrength = 2f)
	{
		bufferLength = SawCollider.OverlapCollider(new ContactFilter2D
		{
			layerMask = LayerMask,
			useLayerMask = true
		}, buffer);
		foreach (KeyValuePair<PhysicalBehaviour, SoftConnection> softConnection in softConnections)
		{
			softConnection.Value.shouldBeDeleted = true;
		}
		Vector2 relativeForce = ForceVector * 0.5f;
		float num = PhysicalBehaviour.Charge * 0.1f;
		for (int i = 0; i < bufferLength; i++)
		{
			Collider2D collider2D = buffer[i];
			if (!(collider2D.transform.root != base.transform.root) || !Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collider2D.transform, out var value))
			{
				continue;
			}
			value.rigidbody.AddRelativeForce(relativeForce, ForceMode2D.Force);
			Shot shot;
			if (softConnections.TryGetValue(value, out var value2))
			{
				value2.shouldBeDeleted = !value2.coll || !value2.joint || !value2.phys;
				if (value2.shouldBeDeleted)
				{
					continue;
				}
				Vector3 vector = GetHitPoint(collider2D);
				value2.joint.anchor = value2.phys.transform.InverseTransformPoint(vector);
				value2.joint.maxForce = connectionStrength;
				value2.joint.maxTorque = connectionStrength;
				if (shouldSaw)
				{
					if (UnityEngine.Random.value > 0.995f - num)
					{
						value.BroadcastMessage("Slice", SendMessageOptions.DontRequireReceiver);
					}
					else if (UnityEngine.Random.value > 0.85f)
					{
						Vector3 normalized = (base.transform.position - vector).normalized;
						PhysicalBehaviour physicalBehaviour = value;
						shot = new Shot(normalized, vector, Damage, triggerExplosiveOverride: false)
						{
							CanCrush = false
						};
						physicalBehaviour.BroadcastMessage("Shot", shot, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
			else
			{
				if (!shouldSaw)
				{
					continue;
				}
				if (value.Properties.Softness >= MinSoftness)
				{
					SoftConnect(value, collider2D, connectionStrength);
					continue;
				}
				Vector3 vector2 = GetHitPoint(collider2D);
				Vector3 normalized2 = (base.transform.position - vector2).normalized;
				PhysicalBehaviour.rigidbody.AddForceAtPosition((1f - value.Properties.Softness) * 50f * normalized2, vector2);
				if (UnityEngine.Random.value > 0.85f)
				{
					PhysicalBehaviour physicalBehaviour2 = value;
					shot = new Shot(normalized2, vector2, Damage, triggerExplosiveOverride: false)
					{
						CanCrush = false
					};
					physicalBehaviour2.BroadcastMessage("Shot", shot, SendMessageOptions.DontRequireReceiver);
				}
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
						IgnoreCollisionStackController.IgnoreCollisionSubstituteMethod(softConnection2.Value.coll, SawCollider, ignore: false);
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

	private void SoftConnect(PhysicalBehaviour otherPhys, Collider2D coll, float connectionStrength)
	{
		if ((bool)otherPhys && (bool)coll)
		{
			Vector2 hitPoint = GetHitPoint(coll);
			FrictionJoint2D frictionJoint2D = otherPhys.gameObject.AddComponent<FrictionJoint2D>();
			frictionJoint2D.autoConfigureConnectedAnchor = true;
			frictionJoint2D.enableCollision = true;
			frictionJoint2D.maxForce = connectionStrength;
			frictionJoint2D.connectedBody = PhysicalBehaviour.rigidbody;
			frictionJoint2D.maxTorque = connectionStrength;
			frictionJoint2D.anchor = otherPhys.transform.InverseTransformPoint(hitPoint);
			IgnoreCollisionStackController.IgnoreCollisionSubstituteMethod(coll, SawCollider);
			softConnections.Add(otherPhys, new SoftConnection(frictionJoint2D, otherPhys, coll));
		}
	}

	private Vector2 GetHitPoint(Collider2D coll)
	{
		Vector2 position = coll.ClosestPoint(base.transform.position);
		return SawCollider.ClosestPoint(position);
	}

	private void Update()
	{
		if (DamagableMachinery.Destroyed && IdleLoop.isPlaying)
		{
			IdleLoop.Stop();
		}
		else if (!DamagableMachinery.Destroyed && !IdleLoop.isPlaying)
		{
			IdleLoop.Play();
		}
		float num = Time.time * JitterSpeed;
		Saw.transform.localPosition = JitterIntensity * Mathf.Max(rotationSpeed, 0.5f) * Utils.GetPerlin2Mapped(num, 0f - num + seed);
		chainPos += rotationSpeed * Time.deltaTime;
		propertyBlock.SetFloat(ShaderProperties.Get("_Position"), chainPos);
		Saw.SetPropertyBlock(propertyBlock);
		if (PhysicalBehaviour.IsBeingUsedContinuously() && !DamagableMachinery.Destroyed)
		{
			rotationSpeed += Time.deltaTime * UseRotationSpeedAddition;
		}
	}

	public void Use(ActivationPropagation activation)
	{
		if (!DamagableMachinery.Destroyed)
		{
			PhysicalBehaviour.PlayClipOnce(Rev, 0.5f);
		}
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
				Debug.LogWarning("Chainsaw victim with invalid or non-existent ID");
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
				Debug.LogWarning($"Chainsaw victim with ID {SerialisableVictims[i]} does not exist");
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

	private void OnDestroy()
	{
		foreach (KeyValuePair<PhysicalBehaviour, SoftConnection> softConnection in softConnections)
		{
			UnityEngine.Object.Destroy(softConnection.Value.joint);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SharpOnAllSidesBehaviour : MonoBehaviour, Messages.IOnBeforeSerialise, Messages.IOnAfterDeserialise
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
	public Collider2D SharpCollider;

	[SkipSerialisation]
	public float ConnectionStrength = 300f;

	[SkipSerialisation]
	public float MinSpeed = 10f;

	[SkipSerialisation]
	public float MinSoftness = 0.5f;

	[SkipSerialisation]
	public Vector2 Tip = Vector2.up;

	[SkipSerialisation]
	public LayerMask LayerMask;

	[HideInInspector]
	public Guid[] SerialisableVictims = new Guid[0];

	private readonly Collider2D[] buffer = new Collider2D[16];

	private int bufferLength;

	private readonly Dictionary<PhysicalBehaviour, SoftConnection> softConnections = new Dictionary<PhysicalBehaviour, SoftConnection>();

	private void FixedUpdate()
	{
		bool flag = PhysicalBehaviour.rigidbody.GetRelativePointVelocity(Tip).magnitude > MinSpeed;
		ProcessSoftConnections(flag, flag ? 10f : ConnectionStrength);
	}

	private void ProcessSoftConnections(bool shouldSaw = true, float connectionStrength = 2f)
	{
		bufferLength = SharpCollider.OverlapCollider(new ContactFilter2D
		{
			layerMask = LayerMask,
			useLayerMask = true
		}, buffer);
		foreach (KeyValuePair<PhysicalBehaviour, SoftConnection> softConnection in softConnections)
		{
			softConnection.Value.shouldBeDeleted = true;
		}
		for (int i = 0; i < bufferLength; i++)
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
					Vector3 position = GetHitPoint(collider2D);
					value2.joint.anchor = value2.phys.transform.InverseTransformPoint(position);
					value2.joint.maxForce = connectionStrength;
					value2.joint.maxTorque = connectionStrength;
				}
			}
			else if (shouldSaw && (value.Properties.Softness >= MinSoftness || (PhysicalBehaviour.Properties.IgnoreStabResistance && !value.Properties.IgnoreStabResistance)))
			{
				SoftConnect(value, collider2D, connectionStrength);
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
						IgnoreCollisionStackController.IgnoreCollisionSubstituteMethod(softConnection2.Value.coll, SharpCollider, ignore: false);
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

	private void OnDisable()
	{
		OnDestroy();
	}

	private void OnDestroy()
	{
		foreach (KeyValuePair<PhysicalBehaviour, SoftConnection> softConnection in softConnections)
		{
			UnityEngine.Object.Destroy(softConnection.Value.joint);
		}
		softConnections.Clear();
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
				Debug.LogWarning("SharpOnSidesBehaviour victim with invalid or non-existent ID");
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
				Debug.LogWarning($"SharpOnSidesBehaviour victim with ID {SerialisableVictims[i]} does not exist");
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
			Vector2 hitPoint = GetHitPoint(coll);
			FrictionJoint2D frictionJoint2D = otherPhys.gameObject.AddComponent<FrictionJoint2D>();
			frictionJoint2D.autoConfigureConnectedAnchor = true;
			frictionJoint2D.enableCollision = true;
			frictionJoint2D.maxForce = connectionStrength;
			frictionJoint2D.connectedBody = PhysicalBehaviour.rigidbody;
			frictionJoint2D.maxTorque = connectionStrength;
			frictionJoint2D.anchor = otherPhys.transform.InverseTransformPoint(hitPoint);
			IgnoreCollisionStackController.IgnoreCollisionSubstituteMethod(coll, SharpCollider);
			Stabbing stabbing = new Stabbing(PhysicalBehaviour, otherPhys, (base.transform.position - otherPhys.transform.position).normalized, hitPoint);
			otherPhys.SendMessage("Stabbed", stabbing, SendMessageOptions.DontRequireReceiver);
			base.gameObject.SendMessage("Lodged", stabbing, SendMessageOptions.DontRequireReceiver);
			softConnections.Add(otherPhys, new SoftConnection(frictionJoint2D, otherPhys, coll));
		}
	}

	private Vector2 GetHitPoint(Collider2D coll)
	{
		Vector2 position = coll.ClosestPoint(base.transform.position);
		return SharpCollider.ClosestPoint(position);
	}
}

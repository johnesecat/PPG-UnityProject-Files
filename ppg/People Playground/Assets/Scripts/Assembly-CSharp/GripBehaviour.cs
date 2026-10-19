using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class GripBehaviour : MonoBehaviour, Messages.IUse, Messages.IOnBeforeSerialise
{
	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	public PhysicalBehaviour CurrentlyHolding;

	public Vector3 GripPosition;

	private FixedJoint2D joint;

	private static readonly Collider2D[] buffer = new Collider2D[32];

	[NonSerialized]
	[SkipSerialisation]
	public List<Collider2D> CollidersToIgnore = new List<Collider2D>();

	[ReadOnly]
	public bool isHolding;

	[HideInInspector]
	public Vector2 NearestHoldingPos;

	[HideInInspector]
	public Vector2 Anchor;

	[HideInInspector]
	public Vector2 ConnectedAnchor;

	private void Awake()
	{
		PhysicalBehaviour = GetComponent<PhysicalBehaviour>();
	}

	private void Start()
	{
		base.transform.root.GetComponentsInChildren(CollidersToIgnore);
		if (isHolding && (bool)CurrentlyHolding)
		{
			Attach(CurrentlyHolding, NearestHoldingPos);
		}
	}

	public void Use(ActivationPropagation activation)
	{
		if (!isHolding)
		{
			PickUpNearestObject();
		}
		else
		{
			DropObject();
		}
	}

	public void RefreshNoCollide(bool ignore)
	{
		if (!CurrentlyHolding)
		{
			return;
		}
		Collider2D[] componentsInChildren = CurrentlyHolding.GetComponentsInChildren<Collider2D>();
		foreach (Collider2D collider2D in componentsInChildren)
		{
			foreach (Collider2D item in CollidersToIgnore)
			{
				if ((bool)item && (bool)collider2D && item != collider2D)
				{
					IgnoreCollisionStackController.IgnoreCollisionSubstituteMethod(collider2D, item, ignore);
				}
			}
		}
	}

	public void DropObject()
	{
		if (isHolding)
		{
			isHolding = false;
			UnityEngine.Object.Destroy(joint);
			if ((bool)CurrentlyHolding)
			{
				StartCoroutine(WaitUntilCollisionReactivate());
				CurrentlyHolding.beingHeldByGripper = false;
				CurrentlyHolding.SendMessage("OnDrop", this, SendMessageOptions.DontRequireReceiver);
				CurrentlyHolding = null;
			}
		}
	}

	private IEnumerator WaitUntilCollisionReactivate()
	{
		PhysicalBehaviour other = CurrentlyHolding;
		int maxSecs = 15;
		float minDistance = 1f;
		do
		{
			yield return new WaitForSeconds(1f);
			maxSecs--;
		}
		while (maxSecs > 0 && !(Vector2.Distance(other.transform.position, base.transform.TransformPoint(GripPosition)) > minDistance));
		Collider2D[] componentsInChildren = other.GetComponentsInChildren<Collider2D>();
		foreach (Collider2D collider2D in componentsInChildren)
		{
			foreach (Collider2D item in CollidersToIgnore)
			{
				if ((bool)item && (bool)collider2D && item != collider2D)
				{
					IgnoreCollisionStackController.IgnoreCollisionSubstituteMethod(collider2D, item, ignore: false);
				}
			}
		}
	}

	private void Update()
	{
		if (isHolding && !CurrentlyHolding)
		{
			DropObject();
		}
	}

	private void FixedUpdate()
	{
		if ((bool)CurrentlyHolding && PhysicalBehaviour.SimulateTemperature && CurrentlyHolding.SimulateTemperature)
		{
			Utils.AverageTemperature(PhysicalBehaviour, CurrentlyHolding, 0.02f);
		}
	}

	private void PickUpNearestObject()
	{
		Vector2 worldPoint = base.transform.TransformPoint(GripPosition);
		int num = Physics2D.OverlapCircleNonAlloc(base.transform.TransformPoint(GripPosition), GripPosition.z, buffer);
		PhysicalBehaviour physicalBehaviour = null;
		float num2 = float.MaxValue;
		for (int i = 0; i < num; i++)
		{
			Collider2D collider2D = buffer[i];
			if (!(collider2D.transform.root == base.transform.root) && Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collider2D.transform, out var value))
			{
				float distance;
				Vector2 nearestLocalHoldingPoint = value.GetNearestLocalHoldingPoint(worldPoint, out distance);
				if (distance < num2)
				{
					num2 = distance;
					NearestHoldingPos = nearestLocalHoldingPoint;
					physicalBehaviour = value;
				}
			}
		}
		if ((bool)physicalBehaviour)
		{
			Attach(physicalBehaviour, NearestHoldingPos);
		}
	}

	private void Attach(PhysicalBehaviour phys, Vector2 otherLocalHoldingPosition)
	{
		isHolding = true;
		CurrentlyHolding = phys;
		phys.transform.position += base.transform.TransformPoint(GripPosition) - phys.transform.TransformPoint(otherLocalHoldingPosition);
		joint = base.gameObject.AddComponent<FixedJoint2D>();
		joint.connectedBody = phys.rigidbody;
		joint.anchor = GripPosition;
		joint.connectedAnchor = otherLocalHoldingPosition;
		joint.enableCollision = false;
		RefreshNoCollide(ignore: true);
		CurrentlyHolding.SendMessage("OnGripped", this, SendMessageOptions.DontRequireReceiver);
	}

	public void OnBeforeSerialise()
	{
		if ((bool)joint)
		{
			ConnectedAnchor = joint.connectedAnchor;
			NearestHoldingPos = ConnectedAnchor;
			Anchor = joint.anchor;
			GripPosition = new Vector3(Anchor.x, Anchor.y, GripPosition.z);
		}
	}
}

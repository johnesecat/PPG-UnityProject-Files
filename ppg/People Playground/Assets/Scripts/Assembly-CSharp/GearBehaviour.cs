using System;
using UnityEngine;
using UnityEngine.Events;

[Obsolete]
public class GearBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public int ToothCount = 64;

	[SkipSerialisation]
	public PhysicalBehaviour Phys;

	[SkipSerialisation]
	public LayerMask GearLayer;

	private UnityEvent onDestroy = new UnityEvent();

	private void Awake()
	{
		GearManagerBehaviour.Gears.Add(this);
	}

	private void OnTriggerEnter2D(Collider2D coll)
	{
		if (GearLayer.HasLayer(coll.gameObject.layer))
		{
			coll.GetComponentInParent<GearBehaviour>();
		}
	}

	private static bool IsInvalid(GearBehaviour g)
	{
		return !g;
	}

	public void ManagedUpdate(float dt)
	{
	}

	public void ManagedLateUpdate()
	{
	}

	private void OnDestroy()
	{
		onDestroy.Invoke();
		GearManagerBehaviour.Gears.Remove(this);
	}
}

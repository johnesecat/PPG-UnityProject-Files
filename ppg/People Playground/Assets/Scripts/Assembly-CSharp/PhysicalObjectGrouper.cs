using System;
using System.Collections.Generic;
using UnityEngine;

[Obsolete]
public static class PhysicalObjectGrouper
{
	public static void Group(IEnumerable<PhysicalBehaviour> physicals)
	{
		float num = 0f;
		Vector3 position = default(Vector3);
		GameObject gameObject = new GameObject("Group Container " + Guid.NewGuid().ToString());
		gameObject.layer = 9;
		Rigidbody2D rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
		int num2 = 0;
		foreach (PhysicalBehaviour physical in physicals)
		{
			num2++;
			position += physical.rigidbody.mass * physical.transform.position;
			num += physical.rigidbody.mass;
			UnityEngine.Object.Destroy(physical.rigidbody);
			physical.rigidbody = rigidbody2D;
		}
		position /= (float)num2;
		gameObject.transform.position = position;
		foreach (PhysicalBehaviour physical2 in physicals)
		{
			physical2.transform.root.SetParent(gameObject.transform);
		}
		rigidbody2D.mass = num;
		rigidbody2D.centerOfMass = gameObject.transform.InverseTransformPoint(position);
		rigidbody2D.useAutoMass = false;
	}
}

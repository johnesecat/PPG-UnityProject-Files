using System;
using UnityEngine;

[Obsolete]
public class GlassShardBehaviour : MonoBehaviour
{
	public Rigidbody2D Rigidbody;

	public float SpeedThreshold = 5f;

	public float RandomThreshold = 0.5f;

	private readonly Collider2D[] buffer = new Collider2D[4];

	private void FixedUpdate()
	{
		if (!(UnityEngine.Random.value > RandomThreshold) || !(Rigidbody.velocity.sqrMagnitude > SpeedThreshold * SpeedThreshold))
		{
			return;
		}
		int num = Physics2D.OverlapPointNonAlloc(base.transform.position, buffer);
		if (num == 0)
		{
			return;
		}
		for (int i = 0; i < num; i++)
		{
			Collider2D collider2D = buffer[i];
			if (Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collider2D.transform, out var value) && value.TryGetComponent<CirculationBehaviour>(out var component))
			{
				component.Cut(base.transform.position, Rigidbody.velocity.normalized);
			}
		}
	}
}

using System.Collections.Generic;
using UnityEngine;

public class SoapBehaviour : MonoBehaviour
{
	private static readonly List<DecalControllerBehaviour> results = new List<DecalControllerBehaviour>();

	private Vector2? lastPosition;

	private const float minDistanceBetweenSliding = 1f;

	private void OnCollisionStay2D(Collision2D collision)
	{
		if (lastPosition.HasValue)
		{
			if (Vector2.Distance(base.transform.position, lastPosition.Value) > 1f)
			{
				Clean(collision.transform);
				lastPosition = base.transform.position;
			}
		}
		else
		{
			Clean(collision.transform);
		}
	}

	private void Clean(Transform hit)
	{
		Vector3 position = base.transform.position;
		hit.GetComponentsInParent(includeInactive: false, results);
		foreach (DecalControllerBehaviour result in results)
		{
			if (!result || !result.decalHolder)
			{
				continue;
			}
			foreach (Transform item in result.decalHolder.transform)
			{
				if ((bool)item.gameObject && (item.position - position).sqrMagnitude < 5f)
				{
					result.localDecalPositions.Remove(item.localPosition);
					Object.Destroy(item.gameObject);
				}
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		lastPosition = base.transform.position;
		Clean(collision.transform);
	}
}

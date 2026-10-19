using System.Collections.Generic;
using UnityEngine;

public class PoolGenerator : MonoBehaviour
{
	public uint PoolSize = 256u;

	public float PoolInactivityThreshold = 300f;

	private readonly Dictionary<int, ObjectPoolBehaviour> pools = new Dictionary<int, ObjectPoolBehaviour>();

	public static PoolGenerator Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	public bool GetFor(GameObject prefab, out ObjectPoolBehaviour pool)
	{
		int instanceID = prefab.GetInstanceID();
		return pools.TryGetValue(instanceID, out pool);
	}

	public ObjectPoolBehaviour Ensure(GameObject prefab)
	{
		int instanceID = prefab.GetInstanceID();
		if (pools.TryGetValue(instanceID, out var value))
		{
			return value;
		}
		value = new GameObject(prefab.name + " pool").AddComponent<ObjectPoolBehaviour>();
		value.MaxPoolSize = PoolSize;
		value.Prefab = prefab;
		pools.Add(instanceID, value);
		return value;
	}

	public GameObject RequestPrefab(GameObject originalPrefab, Vector2 position)
	{
		return Ensure(originalPrefab).Request(position);
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class ObjectPoolBehaviour : MonoBehaviour
{
	public const string PoolableTag = "Poolable";

	public GameObject Prefab;

	public uint MaxPoolSize = 256u;

	public float InactivityThresholdInSeconds = 300f;

	private readonly HashSet<GameObject> createdObjects = new HashSet<GameObject>();

	private readonly HashSet<GameObject> availableObjects = new HashSet<GameObject>();

	private float inactivityTimer;

	public int GetPoolSize()
	{
		return createdObjects.Count;
	}

	public int GetActivePoolSize()
	{
		return createdObjects.Count((GameObject c) => c.activeSelf);
	}

	public int GetInactivePoolSize()
	{
		return availableObjects.Count;
	}

	private void Update()
	{
		if (GetPoolSize() != 0)
		{
			inactivityTimer += Time.deltaTime;
			if (inactivityTimer > InactivityThresholdInSeconds && availableObjects.Count == createdObjects.Count)
			{
				Clear();
				inactivityTimer = 0f;
			}
		}
	}

	[Button(null, EButtonEnableMode.Always)]
	public void Clear()
	{
		foreach (GameObject createdObject in createdObjects)
		{
			UnityEngine.Object.Destroy(createdObject);
		}
		createdObjects.Clear();
		availableObjects.Clear();
	}

	public GameObject Request(Vector2 position)
	{
		inactivityTimer = 0f;
		GameObject gameObject = null;
		if (availableObjects.Count > 0)
		{
			gameObject = availableObjects.FirstOrDefault();
			if (!gameObject)
			{
				Debug.LogError("A poolable object has been destroyed! This is not allowed to happen. They should be returned to the pool in an inactive state.");
				availableObjects.RemoveWhere((GameObject d) => !d);
				return null;
			}
		}
		if ((bool)gameObject)
		{
			availableObjects.Remove(gameObject);
			gameObject.transform.position = position;
			gameObject.SetActive(value: true);
			gameObject.BroadcastMessage("OnPoolableInitialised", this, SendMessageOptions.DontRequireReceiver);
			gameObject.BroadcastMessage("OnPoolableReinitialised", this, SendMessageOptions.DontRequireReceiver);
			return gameObject;
		}
		if (createdObjects.Count >= MaxPoolSize)
		{
			return null;
		}
		gameObject = UnityEngine.Object.Instantiate(Prefab, position, Quaternion.identity);
		gameObject.SetActive(value: true);
		gameObject.BroadcastMessage("OnPoolableInitialised", this, SendMessageOptions.DontRequireReceiver);
		createdObjects.Add(gameObject);
		return gameObject;
	}

	public void Return(GameObject poolable)
	{
		if (!poolable || !createdObjects.Contains(poolable))
		{
			Debug.LogError("Invalid object being returned to the pool");
			return;
		}
		poolable.SetActive(value: false);
		availableObjects.Add(poolable);
	}

	[Obsolete("GetPoolSize")]
	public int GetGetPoolSize()
	{
		return GetPoolSize();
	}

	[Obsolete("GetActivePoolSize")]
	public int GetGetActivePoolSize()
	{
		return GetActivePoolSize();
	}

	[Obsolete("GetInactivePoolSize")]
	public int GetGetInactivePoolSize()
	{
		return GetInactivePoolSize();
	}
}

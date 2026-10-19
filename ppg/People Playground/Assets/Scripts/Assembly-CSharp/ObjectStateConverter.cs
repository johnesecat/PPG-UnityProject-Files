using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ObjectStateConverter
{
	private static readonly HashSet<Transform> alreadyMessaged = new HashSet<Transform>();

	private static void SendMessageToEntireSpawnable(IEnumerable<GameObject> gameObjects, string message, object param = null)
	{
		alreadyMessaged.Clear();
		foreach (GameObject gameObject in gameObjects)
		{
			if (!alreadyMessaged.Contains(gameObject.transform.root))
			{
				alreadyMessaged.Add(gameObject.transform.root);
				if (param == null)
				{
					gameObject.transform.root.BroadcastMessage(message, SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					gameObject.transform.root.BroadcastMessage(message, param, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	public static ObjectState[] Convert(IEnumerable<GameObject> gameObjects, Vector3 newOrigin)
	{
		List<ObjectState> list = new List<ObjectState>();
		List<Transform> list2 = new List<Transform>();
		foreach (SerialisableIdentity item in gameObjects.SelectMany((GameObject c) => c.GetComponentsInChildren(typeof(SerialisableIdentity), includeInactive: true)))
		{
			item.Regenerate();
		}
		SendMessageToEntireSpawnable(gameObjects, "OnBeforeSerialise");
		foreach (GameObject gameObject in gameObjects)
		{
			Transform root = gameObject.transform.root;
			if (!list2.Contains(root))
			{
				list2.Add(root);
				SerialiseInstructions component = root.GetComponent<SerialiseInstructions>();
				if ((bool)component)
				{
					list.Add(component.GetCurrentState(newOrigin));
				}
			}
		}
		return list.ToArray();
	}

	public static GameObject[] Convert(IEnumerable<ObjectState> objectStates, Vector3 newOrigin, bool flipped = false)
	{
		List<GameObject> list = new List<GameObject>();
		Dictionary<GameObject, ObjectState> dictionary = new Dictionary<GameObject, ObjectState>();
		foreach (ObjectState objectState in objectStates)
		{
			GameObject gameObject;
			try
			{
				gameObject = objectState.CreateNew(newOrigin, flipped);
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				NotificationControllerBehaviour.Show("Deserialisation error! This contraption is incompatible.");
				NotificationControllerBehaviour.Show("This may be a version mismatch or a missing mod.");
				continue;
			}
			Vector3 localScale = gameObject.transform.localScale;
			localScale.x *= ((!flipped) ? 1 : (-1));
			gameObject.transform.localScale = localScale;
			list.Add(gameObject);
			dictionary.Add(gameObject, objectState);
		}
		IEnumerable<SerialisableIdentity> referencePool = list.SelectMany((GameObject c) => c.GetComponentsInChildren(typeof(SerialisableIdentity), includeInactive: true)).Cast<SerialisableIdentity>();
		foreach (GameObject item in list)
		{
			if (!dictionary.TryGetValue(item, out var value))
			{
				Debug.LogWarning(item?.ToString() + "does not have a state linked");
			}
			else
			{
				value.LinkReferences(item, referencePool);
			}
		}
		SendMessageToEntireSpawnable(list, "OnAfterDeserialise", list);
		return list.ToArray();
	}
}

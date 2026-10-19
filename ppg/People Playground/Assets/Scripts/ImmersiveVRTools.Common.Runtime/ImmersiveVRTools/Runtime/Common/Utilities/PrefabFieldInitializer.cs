using System;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
	public class PrefabFieldInitializer
	{
		public static bool TrySetIfNotAssigned(GameObject prefab, Action<GameObject> setPrefab, string prefabName)
		{
			return EditorFieldInitializerGeneric<GameObject>.TrySetIfNotAssigned(prefab, setPrefab, prefabName, "Prefab");
		}
	}
}

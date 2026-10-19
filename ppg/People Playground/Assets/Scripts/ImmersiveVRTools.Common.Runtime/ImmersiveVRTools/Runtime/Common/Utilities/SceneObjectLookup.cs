using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
	public static class SceneObjectLookup
	{
		public static List<T> FindObjectsOfTypeAll<T>()
		{
			List<T> list = new List<T>();
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				Scene sceneAt = SceneManager.GetSceneAt(i);
				if (sceneAt.isLoaded)
				{
					GameObject[] rootGameObjects = sceneAt.GetRootGameObjects();
					foreach (GameObject gameObject in rootGameObjects)
					{
						list.AddRange(gameObject.GetComponentsInChildren<T>(includeInactive: true));
					}
				}
			}
			return list;
		}
	}
}

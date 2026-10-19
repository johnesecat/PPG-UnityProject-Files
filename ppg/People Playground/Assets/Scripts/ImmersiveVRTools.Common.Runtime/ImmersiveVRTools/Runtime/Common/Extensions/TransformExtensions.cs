using System.Collections.Generic;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Extensions
{
	public static class TransformExtensions
	{
		public static Transform FindChildRecursive(this Transform parent, string name)
		{
			foreach (Transform item in parent)
			{
				if (item.name.Contains(name))
				{
					return item;
				}
				Transform transform2 = item.FindChildRecursive(name);
				if (transform2 != null)
				{
					return transform2;
				}
			}
			return null;
		}

		public static void RemoveAllChildren(this Transform transform)
		{
			foreach (Transform item in transform)
			{
				Object.Destroy(item.gameObject);
			}
		}

		public static void RemoveAllChildrenExcept(this Transform transform, List<Transform> except)
		{
			foreach (Transform item in transform)
			{
				if (!except.Contains(item))
				{
					Object.Destroy(item.gameObject);
				}
			}
		}

		public static IEnumerable<Transform> GetAllChildren(this Transform transform)
		{
			foreach (Transform item in transform)
			{
				yield return item;
			}
		}
	}
}

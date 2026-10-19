using System.Linq;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Extensions
{
	public static class ComponentExtensions
	{
		public static bool HasTag(this Component component)
		{
			if (!string.IsNullOrWhiteSpace(component.tag))
			{
				return component.tag != "Untagged";
			}
			return false;
		}

		public static string GetFullPath(this Component component)
		{
			return string.Join("/", (from t in component.GetComponentsInParent<Transform>()
				select t.name).Reverse().ToArray());
		}
	}
}

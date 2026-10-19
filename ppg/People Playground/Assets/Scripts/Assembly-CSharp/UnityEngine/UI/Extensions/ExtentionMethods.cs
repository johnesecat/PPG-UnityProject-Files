using System;

namespace UnityEngine.UI.Extensions
{
	public static class ExtentionMethods
	{
		public static T GetOrAddComponent<T>(this GameObject child) where T : Component
		{
			T val = child.GetComponent<T>();
			if ((Object)val == (Object)null)
			{
				val = child.AddComponent<T>();
			}
			return val;
		}

		public static bool IsPrefab(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				throw new ArgumentNullException("gameObject");
			}
			if (!gameObject.scene.IsValid() && !gameObject.scene.isLoaded && gameObject.GetInstanceID() >= 0)
			{
				return !gameObject.hideFlags.HasFlag(HideFlags.HideInHierarchy);
			}
			return false;
		}
	}
}

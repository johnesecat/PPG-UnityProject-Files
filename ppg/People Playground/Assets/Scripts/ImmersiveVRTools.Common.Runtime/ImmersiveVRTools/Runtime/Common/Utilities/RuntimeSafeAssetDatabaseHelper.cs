using System.Collections.Generic;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
	public class RuntimeSafeAssetDatabaseHelper
	{
		public static List<T> GetAllScriptableObjects<T>() where T : UnityEngine.ScriptableObject
		{
			return null;
		}

		public static T GetAssetOrSubAsset<T>(string name) where T : Object
		{
			return null;
		}

		public static bool TryGetAssetPath<T>(string name, out string assetPath) where T : class
		{
			assetPath = string.Empty;
			return false;
		}
	}
}

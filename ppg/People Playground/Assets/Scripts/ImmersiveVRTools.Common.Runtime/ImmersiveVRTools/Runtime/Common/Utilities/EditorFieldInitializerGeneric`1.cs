using System;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
	public class EditorFieldInitializerGeneric<T> where T : UnityEngine.Object
	{
		public static bool TrySetIfNotAssigned(object obj, Action<T> setObject, string name, string typeFilter)
		{
			return false;
		}
	}
}

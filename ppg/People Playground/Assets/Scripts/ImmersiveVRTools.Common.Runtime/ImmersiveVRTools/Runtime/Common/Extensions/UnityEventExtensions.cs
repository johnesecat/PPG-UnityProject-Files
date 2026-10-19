using System;
using UnityEngine;
using UnityEngine.Events;

namespace ImmersiveVRTools.Runtime.Common.Extensions
{
	public static class UnityEventExtensions
	{
		public static void SafeInvoke<T>(this UnityEvent<T> unityEvent, T args)
		{
			try
			{
				unityEvent?.Invoke(args);
			}
			catch (Exception arg)
			{
				UnityEngine.Debug.Log($"Exception when invoking event code for '{unityEvent?.GetType().Name}', {arg}");
				throw;
			}
		}

		public static void SafeInvoke(this UnityEvent unityEvent)
		{
			try
			{
				unityEvent?.Invoke();
			}
			catch (Exception arg)
			{
				UnityEngine.Debug.Log($"Exception when invoking event code for '{unityEvent?.GetType().Name}', {arg}");
				throw;
			}
		}
	}
}

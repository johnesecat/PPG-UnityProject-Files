using System;
using System.Collections;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
	public class DelayedExecutor
	{
		public static void ExecuteDelayed(Action action, float executeAfterSeconds, MonoBehaviour coroutineRunner)
		{
			coroutineRunner.StartCoroutine(ExecuteDelayed(action, executeAfterSeconds));
		}

		public static void ExecuteOnNextFrame(Action action, MonoBehaviour coroutineRunner)
		{
			ExecuteDelayed(action, 0f, coroutineRunner);
		}

		private static IEnumerator ExecuteDelayed(Action action, float executeAfterSeconds)
		{
			if (executeAfterSeconds == 0f)
			{
				yield return new WaitForEndOfFrame();
			}
			else
			{
				yield return new WaitForSeconds(executeAfterSeconds);
			}
			action();
		}
	}
}

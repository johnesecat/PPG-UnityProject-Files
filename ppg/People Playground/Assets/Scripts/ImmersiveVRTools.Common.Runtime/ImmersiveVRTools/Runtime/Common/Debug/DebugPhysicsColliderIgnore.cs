using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Debug
{
	public class DebugPhysicsColliderIgnore : MonoBehaviour
	{
		[ContextMenu("Log to console")]
		public void LogToConsole()
		{
			UnityEngine.Debug.LogWarning("DEBUG_ADDITIONAL_NATIVE_UNITY_CALLS_TRACKING_VIA_EXTENSION_METHODS - not specified as build symbol, tracking disabled");
		}
	}
}

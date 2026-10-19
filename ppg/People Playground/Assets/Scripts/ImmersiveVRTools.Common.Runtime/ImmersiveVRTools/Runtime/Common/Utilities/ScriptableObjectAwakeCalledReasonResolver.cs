using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
	public class ScriptableObjectAwakeCalledReasonResolver
	{
		public static ScriptableObjectAwakeCalledReason ResolveAwakeCallReason(UnityEngine.ScriptableObject so)
		{
			return ScriptableObjectAwakeCalledReason.ObjectStartedButNotCreated;
		}
	}
}

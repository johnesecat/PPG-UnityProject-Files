using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
	public class MissingAssetsNotifier : MonoBehaviour
	{
		private const string IgnoreMissingAssetsEditorPrefKey = "IgnoreMissingAssetsEditor";

		[ContextMenu("Check For Missing Assets")]
		public void CheckFormMissingAssets()
		{
		}
	}
}

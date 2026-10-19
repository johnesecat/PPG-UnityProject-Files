using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.PropertyDrawer
{
	public class InlineManagementAttribute : PropertyAttribute
	{
		public const int TOP_SPACING = 10;

		public string AfterCreationNewRunMethodName { get; set; }

		public bool IsCreatingNewDisabled { get; set; }

		public bool AddTopSpacing { get; set; }
	}
}

using ImmersiveVRTools.Runtime.Common.PropertyDrawer;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Variable
{
	public class FeatureReferenceComponent : MonoBehaviour
	{
		[SerializeField]
		[ReferenceOptions(ForceVariableOnly = true)]
		private FeatureReference _featureReference;

		public FeatureReference FeatureReference => _featureReference;
	}
}

using System;

namespace ImmersiveVRTools.Runtime.Common.Variable
{
	[Serializable]
	public class FeatureReference : Reference<string, FeatureVariable>
	{
		public FeatureReference(string Value)
			: base(Value)
		{
		}

		public FeatureReference()
		{
		}
	}
}

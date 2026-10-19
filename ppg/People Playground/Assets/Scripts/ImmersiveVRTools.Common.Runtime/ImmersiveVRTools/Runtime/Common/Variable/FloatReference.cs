using System;

namespace ImmersiveVRTools.Runtime.Common.Variable
{
	[Serializable]
	public class FloatReference : Reference<float, FloatVariable>
	{
		public FloatReference(float Value)
			: base(Value)
		{
		}

		public FloatReference()
		{
		}
	}
}

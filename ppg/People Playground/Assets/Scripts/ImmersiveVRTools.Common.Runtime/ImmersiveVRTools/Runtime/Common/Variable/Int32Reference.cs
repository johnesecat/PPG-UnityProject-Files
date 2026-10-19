using System;

namespace ImmersiveVRTools.Runtime.Common.Variable
{
	[Serializable]
	public class Int32Reference : Reference<int, Int32Variable>
	{
		public Int32Reference(int value)
			: base(value)
		{
		}

		public Int32Reference()
		{
		}
	}
}

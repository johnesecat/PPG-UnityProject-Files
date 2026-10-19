using System;

namespace ImmersiveVRTools.Runtime.Common.Variable
{
	[Serializable]
	public class StringReference : Reference<string, StringVariable>
	{
		public StringReference(string Value)
			: base(Value)
		{
		}

		public StringReference()
		{
		}
	}
}

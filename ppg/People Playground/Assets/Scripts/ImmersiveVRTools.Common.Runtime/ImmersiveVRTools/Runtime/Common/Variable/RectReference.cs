using System;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Variable
{
	[Serializable]
	public class RectReference : Reference<Rect, RectVariable>
	{
		public RectReference(Rect Value)
			: base(Value)
		{
		}

		public RectReference()
		{
		}
	}
}

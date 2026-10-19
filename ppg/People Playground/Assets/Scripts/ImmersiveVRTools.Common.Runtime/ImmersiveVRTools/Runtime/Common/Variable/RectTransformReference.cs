using System;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Variable
{
	[Serializable]
	public class RectTransformReference : Reference<RectTransform, RectTransformVariable>
	{
		public RectTransformReference(RectTransform Value)
			: base(Value)
		{
		}

		public RectTransformReference()
		{
		}
	}
}

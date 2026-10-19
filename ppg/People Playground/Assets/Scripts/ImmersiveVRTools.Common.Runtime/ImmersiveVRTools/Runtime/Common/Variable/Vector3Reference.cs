using System;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Variable
{
	[Serializable]
	public class Vector3Reference : Reference<Vector3, Vector3Variable>
	{
		public Vector3Reference(Vector3 Value)
			: base(Value)
		{
		}

		public Vector3Reference()
		{
		}
	}
}

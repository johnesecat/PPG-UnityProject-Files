using System;
using UnityEngine;

namespace Linefy.Serialization
{
	[Serializable]
	public class SerializationData_PrimitivesGroup : SerializationData_LinefyDrawcall
	{
		[HideInInspector]
		public int capacityChangeStep = 4;

		[Tooltip("Width factor. The used measuremnt units are defined by the WidthMode")]
		public float widthMultiplier = 1f;

		[Tooltip("Algorithm for calculating the Width.")]
		public WidthMode widthMode;
	}
}

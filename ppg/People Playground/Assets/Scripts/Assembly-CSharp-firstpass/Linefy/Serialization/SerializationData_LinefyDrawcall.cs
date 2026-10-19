using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Linefy.Serialization
{
	[Serializable]
	public class SerializationData_LinefyDrawcall
	{
		[HideInInspector]
		public string name;

		[Tooltip("The bound size. If value is negative then auto recalculation of bounds will performed")]
		public float boundsSize = 1000f;

		[Tooltip("Render queue of material.")]
		public int renderOrder;

		[Tooltip("Sets opaque or transparent material. When off, an opaque material with alpha clipping is used. Note that transparent mode may affects on object sorting. ")]
		public bool transparent;

		[Tooltip("The main color.")]
		public Color colorMultiplier = Color.white;

		[Tooltip("The main texture. ")]
		public Texture texture;

		[Tooltip("Shifts all vertices along the view direction by this value. Useful for preventing z-fight")]
		public float viewOffset;

		[Tooltip("Material depth offset factor.")]
		public float depthOffset;

		[Tooltip("The distance to camera which transparency fading start / end")]
		public RangeFloat fadeAlphaDistance = new RangeFloat(10000f, 10001f);

		[Tooltip("How should depth testing be performed. An wrapper for shader ZTest state")]
		public CompareFunction zTest = CompareFunction.LessEqual;
	}
}

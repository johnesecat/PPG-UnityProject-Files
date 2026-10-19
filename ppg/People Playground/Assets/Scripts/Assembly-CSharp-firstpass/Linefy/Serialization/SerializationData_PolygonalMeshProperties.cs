using System;
using UnityEngine;

namespace Linefy.Serialization
{
	[Serializable]
	public class SerializationData_PolygonalMeshProperties : SerializationData_LinefyDrawcall
	{
		[Tooltip("Ambient lighting of internal material.  0 = backface is black   1  = backface equals main color ( unlit shading )")]
		[Range(0f, 1f)]
		public float ambient = 1f;

		[Tooltip("Defines recalculation algorithm of mesh lighting data (normals and tangens).")]
		public LightingMode lighingMode = LightingMode.Lit;

		[Tooltip("The number of corners in a polygon, greater than or equal to which the polygon will dynamically re-triangulate when its shape changes.")]
		public int dynamicTriangulationThreshold = 4;

		[Tooltip("Defines mesh normals recalculation algorithm (weighted or unweighted).")]
		public NormalsRecalculationMode normalsRecalculationMode;

		[Tooltip("Texture transform. xy = scale zw = offset ")]
		public Vector4 textureTransform = new Vector4(1f, 1f, 0f, 0f);

		[Tooltip("Doublesided render mode of internal meaterial")]
		public bool doublesided = true;

		public SerializationData_PolygonalMeshProperties()
		{
			boundsSize = -1f;
		}
	}
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace LeTai.Asset.TranslucentImage
{
	public static class Extensions
	{
		private static Mesh fullscreenTriangle;

		private const float EPSILON01 = 5.9604645E-08f;

		private static Mesh FullscreenTriangle
		{
			get
			{
				if (fullscreenTriangle != null)
				{
					return fullscreenTriangle;
				}
				fullscreenTriangle = new Mesh
				{
					name = "Fullscreen Triangle"
				};
				fullscreenTriangle.SetVertices(new List<Vector3>
				{
					new Vector3(-1f, -1f, 0f),
					new Vector3(-1f, 3f, 0f),
					new Vector3(3f, -1f, 0f)
				});
				fullscreenTriangle.SetIndices(new int[3] { 0, 1, 2 }, MeshTopology.Triangles, 0, calculateBounds: false);
				fullscreenTriangle.UploadMeshData(markNoLongerReadable: false);
				return fullscreenTriangle;
			}
		}

		public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, Material material, int pass)
		{
			cmd.SetGlobalTexture("_MainTex", source);
			cmd.SetRenderTarget(destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
			cmd.DrawMesh(FullscreenTriangle, Matrix4x4.identity, material, 0, pass);
		}

		internal static bool Approximately(this Rect self, Rect other)
		{
			if (QuickApproximate(self.x, other.x) && QuickApproximate(self.y, other.y) && QuickApproximate(self.width, other.width))
			{
				return QuickApproximate(self.height, other.height);
			}
			return false;
		}

		private static bool QuickApproximate(float a, float b)
		{
			return Mathf.Abs(b - a) < 5.9604645E-08f;
		}

		public static Vector4 ToMinMaxVector(this Rect self)
		{
			return new Vector4(self.xMin, self.yMin, self.xMax, self.yMax);
		}

		public static Vector4 ToVector4(this Rect self)
		{
			return new Vector4(self.xMin, self.yMin, self.width, self.height);
		}
	}
}

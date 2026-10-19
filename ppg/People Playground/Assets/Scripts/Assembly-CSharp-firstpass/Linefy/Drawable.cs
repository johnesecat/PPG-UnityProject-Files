using UnityEngine;

namespace Linefy
{
	public abstract class Drawable
	{
		protected Matrix4x4 identityMatrix = Matrix4x4.identity;

		public Matrix4x4 onSceneGUIMatrix = Matrix4x4.identity;

		public Matrix4x4 _editorGUIMatrix = Matrix4x4.identity;

		protected bool _disposed;

		public bool disposed => _disposed;

		public void Draw()
		{
			Draw(identityMatrix, null, 0);
		}

		public void Draw(Matrix4x4 matrix)
		{
			Draw(matrix, null, 0);
		}

		public void Draw(int layer)
		{
			Draw(identityMatrix, null, layer);
		}

		public void Draw(Matrix4x4 matrix, int layer)
		{
			Draw(matrix, null, layer);
		}

		public void Draw(Matrix4x4 matrix, Camera camera)
		{
			Draw(matrix, camera, 0);
		}

		public abstract void Draw(Matrix4x4 matrix, Camera camera, int layer);

		public abstract void DrawNow(Matrix4x4 matrix);

		public abstract void Dispose();

		public abstract void GetStatistic(ref int linesCount, ref int totallinesCount, ref int dotsCount, ref int totalDotsCount, ref int polylinesCount, ref int totalPolylineVerticesCount);
	}
}

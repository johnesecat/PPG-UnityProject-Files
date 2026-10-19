using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[ExecuteInEditMode]
	public class MeshCreator : MonoBehaviour
	{
		public void CreateMesh(List<Vector2> points)
		{
			Vector2[] array = points.ToArray();
			int[] triangles = new Triangulator(array).Triangulate();
			Vector3[] array2 = new Vector3[array.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = new Vector3(array[i].x, array[i].y, 0f);
			}
			Mesh mesh = new Mesh();
			mesh.vertices = array2;
			mesh.triangles = triangles;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			GetComponent<MeshFilter>().mesh = mesh;
		}
	}
}

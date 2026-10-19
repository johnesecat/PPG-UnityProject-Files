using System.Diagnostics;
using UnityEngine;

namespace Linefy
{
	[HelpURL("https://polyflow.xyz/content/linefy/documentation-1-1/linefy-documentation.html#PolygonalMeshAsset")]
	public class PolygonalMeshAsset : ScriptableObject
	{
		public bool importFromObjFoldout = true;

		public string pathToObjFile;

		public float scaleFactor = 1f;

		public bool swapYZAxis;

		public bool flipNormals;

		public SmoothingGroupsImportMode smoothingGroupsImportMode;

		public SerializedPolygonalMesh serializedPolygonalMesh;

		public float lastImportMS;

		public bool ImportObjLocal()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			if (serializedPolygonalMesh == null)
			{
				UnityEngine.Debug.LogWarning("Something wrong with PolygonalMeshAsset serialization. serializedPolygonalMesh == null");
				return false;
			}
			serializedPolygonalMesh.ReadObjFromFile(pathToObjFile, smoothingGroupsImportMode, flipNormals, scaleFactor, swapYZAxis);
			stopwatch.Stop();
			lastImportMS = (float)stopwatch.ElapsedTicks / 10000f;
			return true;
		}

		public PolygonalMeshRenderer InstantiateRenderer(Material material)
		{
			GameObject gameObject = new GameObject($"{base.name} Polygonal Mesh Renderer");
			PolygonalMeshRenderer polygonalMeshRenderer = gameObject.AddComponent<PolygonalMeshRenderer>();
			gameObject.AddComponent<MeshFilter>();
			gameObject.AddComponent<MeshRenderer>().sharedMaterial = material;
			polygonalMeshRenderer.polygonalMeshAsset = this;
			polygonalMeshRenderer.LateUpdate();
			return polygonalMeshRenderer;
		}
	}
}

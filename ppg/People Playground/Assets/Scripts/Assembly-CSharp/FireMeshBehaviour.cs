using System.Collections.Generic;
using UnityEngine;

public class FireMeshBehaviour : MonoBehaviour
{
	public Vector2 Wind = new Vector2(0.5f, 1.5f);

	public float NoiseIntensity = 1f;

	public float NoiseFrequency = 1f;

	private Mesh mesh;

	private GameObject child;

	private MeshFilter filter;

	private MeshRenderer meshRenderer;

	private int[] indices;

	private Vector3[] vertices;

	private Color[] colors;

	private readonly List<int> pointyIndices = new List<int>();

	private void Awake()
	{
		child = new GameObject("fire_renderer", typeof(Optout));
		child.transform.SetParent(base.transform);
		filter = GetComponent<MeshFilter>();
		meshRenderer = GetComponent<MeshRenderer>();
		mesh = new Mesh();
		mesh.MarkDynamic();
		mesh.SetTriangles(indices, 0);
		mesh.SetColors(colors, 0, colors.Length);
		mesh.SetVertices(vertices, 0, vertices.Length);
	}

	private void AddTriangle(Vector2 bottomLeft, Vector2 bottomRight, Vector2 pointy)
	{
	}

	private void Update()
	{
		mesh.UploadMeshData(markNoLongerReadable: false);
	}

	private void OnDestroy()
	{
		Object.Destroy(child);
		Object.Destroy(mesh);
	}
}

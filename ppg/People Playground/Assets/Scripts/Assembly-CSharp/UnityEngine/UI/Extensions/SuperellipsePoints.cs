using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[ExecuteInEditMode]
	public class SuperellipsePoints : MonoBehaviour
	{
		public float xLimits = 1f;

		public float yLimits = 1f;

		[Range(1f, 96f)]
		public float superness = 4f;

		private float lastXLim;

		private float lastYLim;

		private float lastSuper;

		[Space]
		[Range(1f, 32f)]
		public int levelOfDetail = 4;

		private int lastLoD;

		[Space]
		public Material material;

		private List<Vector2> pointList = new List<Vector2>();

		private void Start()
		{
			RecalculateSuperellipse();
			GetComponent<MeshRenderer>().material = material;
			lastXLim = xLimits;
			lastYLim = yLimits;
			lastSuper = superness;
			lastLoD = levelOfDetail;
		}

		private void Update()
		{
			if (lastXLim != xLimits || lastYLim != yLimits || lastSuper != superness || lastLoD != levelOfDetail)
			{
				RecalculateSuperellipse();
			}
			lastXLim = xLimits;
			lastYLim = yLimits;
			lastSuper = superness;
			lastLoD = levelOfDetail;
		}

		private void RecalculateSuperellipse()
		{
			pointList.Clear();
			float num = levelOfDetail * 4;
			for (float num2 = 0f; num2 < xLimits; num2 += 1f / num)
			{
				float y = Superellipse(xLimits, yLimits, num2, superness);
				Vector2 item = new Vector2(num2, y);
				pointList.Add(item);
			}
			pointList.Add(new Vector2(xLimits, 0f));
			pointList.Add(Vector2.zero);
			GetComponent<MeshCreator>().CreateMesh(pointList);
		}

		private float Superellipse(float a, float b, float x, float n)
		{
			float num = Mathf.Pow(x / a, n);
			return Mathf.Pow(1f - num, 1f / n) * b;
		}
	}
}

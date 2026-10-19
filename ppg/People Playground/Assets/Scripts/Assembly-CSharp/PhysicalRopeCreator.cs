using System;
using NaughtyAttributes;
using UnityEngine;

public class PhysicalRopeCreator : MonoBehaviour
{
	public bool CreateOnStart;

	public bool AutoPointCount;

	[HideIf("AutoPointCount")]
	public int PointCount = 32;

	[ShowIf("AutoPointCount")]
	public float PointsPerUnit = 4f;

	public LineRenderer LineRenderer;

	public bool SetAsChild;

	[Space]
	public float NodeMass = 0.05f;

	[Layer]
	public int NodeLayer;

	[Tooltip("-1 to fall back to line width")]
	public float NodeRadius = -1f;

	public float NodeRandomForce = 0.5f;

	private void Start()
	{
		if (CreateOnStart)
		{
			CreateRope();
		}
	}

	public void CreateRope()
	{
		float totalDistance = 0f;
		for (int i = 1; i < LineRenderer.positionCount; i++)
		{
			totalDistance += Vector3.Distance(LineRenderer.GetPosition(i), LineRenderer.GetPosition(i - 1));
		}
		if (AutoPointCount)
		{
			PointCount = (int)(PointsPerUnit * totalDistance);
		}
		if (PointCount < 2)
		{
			throw new Exception("PointCount must be greater than 1");
		}
		Transform[] array = new Transform[PointCount];
		float num = ((NodeRadius < 0f) ? (LineRenderer.widthMultiplier / 2f) : NodeRadius);
		for (int j = 0; j < PointCount; j++)
		{
			float d2 = (float)j / ((float)PointCount - 1f) * totalDistance;
			Vector2 vector = getPointAt(d2);
			GameObject gameObject = new GameObject("Rope point " + j);
			gameObject.transform.position = vector;
			gameObject.layer = NodeLayer;
			if (SetAsChild)
			{
				gameObject.transform.SetParent(base.transform);
			}
			gameObject.AddComponent<BoxCollider2D>().size = Vector2.one * num;
			Rigidbody2D rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
			rigidbody2D.mass = NodeMass;
			rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
			rigidbody2D.AddForce(UnityEngine.Random.insideUnitCircle * NodeRandomForce, ForceMode2D.Impulse);
			if (j > 0)
			{
				DistanceJoint2D distanceJoint2D = gameObject.AddComponent<DistanceJoint2D>();
				distanceJoint2D.connectedBody = array[j - 1].GetComponent<Rigidbody2D>();
				distanceJoint2D.maxDistanceOnly = true;
			}
			array[j] = gameObject.transform;
		}
		LineRendererTargetsBehaviour lineRendererTargetsBehaviour = LineRenderer.gameObject.AddComponent<LineRendererTargetsBehaviour>();
		lineRendererTargetsBehaviour.Targets = array;
		lineRendererTargetsBehaviour.LineRenderer = LineRenderer;
		Vector2 getPointAt(float d)
		{
			d = Mathf.Clamp(d, 0f, totalDistance - float.Epsilon);
			float num2 = 0f;
			for (int k = 1; k < LineRenderer.positionCount; k++)
			{
				float lower = num2;
				num2 += Vector3.Distance(LineRenderer.GetPosition(k), LineRenderer.GetPosition(k - 1));
				if (num2 >= d)
				{
					float t = Mathf.Clamp(Utils.MapRange(lower, num2, 0f, 1f, d), 0f, 1f);
					Vector2 vector2 = Vector2.Lerp(LineRenderer.GetPosition(k - 1), LineRenderer.GetPosition(k), t);
					if (!LineRenderer.useWorldSpace)
					{
						vector2 = LineRenderer.transform.TransformPoint(vector2);
					}
					return vector2;
				}
			}
			throw new Exception("Attempt to get point along line at distance that exceeds the line or is less than 0");
		}
	}
}

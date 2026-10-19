using System;
using System.Collections.Generic;
using UnityEngine;

public class SerpentineBeltBehaviour : Hover
{
	public PhysicalBehaviour Self;

	public PhysicalBehaviour Other;

	private Vector2[] worldPointArray = Array.Empty<Vector2>();

	private readonly List<Vector2> vertices = new List<Vector2>();

	private Vector3[] linePositionBuffer = Array.Empty<Vector3>();

	[SkipSerialisation]
	public LineRenderer LineRenderer;

	private GameObject lineRendererGm;

	private float gearRatio = 1f;

	private float lastScale = -1f;

	private void Awake()
	{
		lineRendererGm = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/SerpentineBelt"));
		lineRendererGm.transform.SetParent(base.transform, worldPositionStays: false);
		LineRenderer = lineRendererGm.GetComponent<LineRenderer>();
		Color color2 = (LineRenderer.endColor = (LineRenderer.startColor = SerpentineBeltTool.WireColor));
		float num2 = (LineRenderer.endWidth = (LineRenderer.startWidth = SerpentineBeltTool.WireWidth));
	}

	private void Start()
	{
		ReallocatePointArray();
		RecalculateGearRatio();
	}

	public void RecalculateGearRatio()
	{
		gearRatio = Mathf.Max(0.01f, Self.GetScaledCircumference()) / Mathf.Max(0.01f, Other.GetScaledCircumference());
		gearRatio = Mathf.Clamp(gearRatio, 0.01f, 1000f);
	}

	protected override bool IsMouseInsideCollider()
	{
		Vector3 mousePosition = Global.main.MousePosition;
		float num = SerpentineBeltTool.WireWidth + 0.02f;
		for (int i = 0; i < linePositionBuffer.Length; i++)
		{
			Vector3 vector = linePositionBuffer[i];
			Vector3 vector2 = linePositionBuffer[(i + 1) % linePositionBuffer.Length];
			if (Mathf.Abs(Utils.SqrdDistanceFromPointToLineSegment(mousePosition, vector, vector2)) <= num * num)
			{
				return true;
			}
		}
		return false;
	}

	public override void OnMouseOverlapEvent(bool overlap)
	{
		base.OnMouseOverlapEvent(overlap);
		if (overlap && UserPreferenceManager.Current.ShowOutlines)
		{
			Color color2 = (LineRenderer.startColor = (LineRenderer.endColor = Color.red));
			LineRenderer.gameObject.layer = LayerMask.NameToLayer("ScreenUI");
		}
		else
		{
			Color color2 = (LineRenderer.startColor = (LineRenderer.endColor = SerpentineBeltTool.WireColor));
			LineRenderer.gameObject.layer = LayerMask.NameToLayer("Default");
		}
	}

	private void Update()
	{
		if ((bool)Self && (bool)Other)
		{
			if (Self != Other)
			{
				float a = Self.transform.localScale.sqrMagnitude * Other.transform.localScale.sqrMagnitude;
				if (!Mathf.Approximately(a, lastScale))
				{
					RecalculateGearRatio();
					lastScale = a;
				}
				UpdateBelt();
			}
			CheckMouseInput();
		}
		else
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void FixedUpdate()
	{
		if ((bool)Other && (bool)Self && Self != Other)
		{
			float num = Self.rigidbody.angularVelocity * 0.9999f;
			float num2 = Other.rigidbody.angularVelocity * 0.9999f;
			Self.rigidbody.angularVelocity += 0.5f * (num2 / gearRatio - num);
			Other.rigidbody.angularVelocity += 0.5f * (num * gearRatio - num2);
		}
	}

	private void UpdateBelt()
	{
		RecalculatePointArray();
		if (worldPointArray.Length < 3)
		{
			if (LineRenderer.enabled)
			{
				LineRenderer.enabled = false;
			}
			return;
		}
		if (!LineRenderer.enabled)
		{
			LineRenderer.enabled = true;
		}
		Utils.ComputeConvexHull(worldPointArray, vertices);
		if (linePositionBuffer.Length != vertices.Count)
		{
			Array.Resize(ref linePositionBuffer, vertices.Count);
		}
		for (int i = 0; i < vertices.Count; i++)
		{
			linePositionBuffer[i] = vertices[i];
		}
		if (LineRenderer.positionCount != vertices.Count)
		{
			LineRenderer.positionCount = vertices.Count;
		}
		LineRenderer.SetPositions(linePositionBuffer);
	}

	public void RecalculatePointArray()
	{
		if (worldPointArray.Length != Self.LocalGridPointLength + Other.LocalGridPointLength)
		{
			ReallocatePointArray();
		}
		int num = 0;
		for (int i = 0; i < Self.LocalGridPointLength; i++)
		{
			worldPointArray[num] = Self.transform.TransformPoint(Self.LocalColliderGridPoints[i]);
			num++;
		}
		for (int j = 0; j < Other.LocalGridPointLength; j++)
		{
			worldPointArray[num] = Other.transform.TransformPoint(Other.LocalColliderGridPoints[j]);
			num++;
		}
	}

	public void ReallocatePointArray()
	{
		if (!Self || !Other)
		{
			worldPointArray = Array.Empty<Vector2>();
			return;
		}
		int num = Self.LocalGridPointLength + Other.LocalGridPointLength;
		worldPointArray = new Vector2[num];
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if ((bool)lineRendererGm)
		{
			UnityEngine.Object.Destroy(lineRendererGm);
		}
	}

	protected override Bounds GetVisualBounds()
	{
		return LineRenderer.bounds;
	}

	public override void OnUserDelete()
	{
	}
}

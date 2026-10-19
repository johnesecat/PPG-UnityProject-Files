using System;
using UnityEngine;

public class SmoothLineControllerBehaviour : MonoBehaviour
{
	[Serializable]
	public struct Node
	{
		public Transform Transform;

		public Transform DirectionTarget;

		public Vector2 LocalDirection;

		public Vector3 GlobalDirection
		{
			get
			{
				if ((bool)DirectionTarget)
				{
					return DirectionTarget.position - Transform.position;
				}
				return Transform.TransformDirection(LocalDirection);
			}
		}
	}

	[SkipSerialisation]
	public LineRenderer LineRenderer;

	[SkipSerialisation]
	public float Length = 1f;

	[SkipSerialisation]
	public int Steps = 8;

	[SkipSerialisation]
	public bool AutoLength;

	[SkipSerialisation]
	public Node From;

	[SkipSerialisation]
	public Node To;

	[SkipSerialisation]
	public bool AutoAdjustWidth;

	private Vector3[] vertices;

	private void Start()
	{
		Initialise();
		UpdateLine();
	}

	public void Initialise()
	{
		vertices = new Vector3[Steps];
		LineRenderer.positionCount = Steps;
	}

	public void UpdateLine()
	{
		if (AutoLength && (bool)From.Transform && (bool)To.Transform)
		{
			Length = Vector2.Distance(From.Transform.position, To.Transform.position);
		}
		CalculateVertices();
		LineRenderer.SetPositions(vertices);
		if (AutoAdjustWidth && (bool)From.Transform && (bool)To.Transform)
		{
			LineRenderer.widthMultiplier = Mathf.Min(Mathf.Min(From.Transform.localScale.x, From.Transform.localScale.y), Mathf.Min(To.Transform.localScale.x, To.Transform.localScale.y));
		}
	}

	private void Update()
	{
		UpdateLine();
	}

	private void CalculateVertices()
	{
		float num = Length / 2f;
		Vector2 b = From.Transform.position;
		Vector2 b2 = From.Transform.position + From.GlobalDirection * num;
		Vector2 b3 = To.Transform.position + To.GlobalDirection * num;
		Vector2 b4 = To.Transform.position;
		for (int i = 0; i < Steps; i++)
		{
			float s = (float)i / (float)(Steps - 1);
			vertices[i] = GetBezierPoint(s, b, b2, b3, b4);
		}
	}

	private Vector2 GetBezierPoint(float s, Vector2 b0, Vector2 b1, Vector2 b2, Vector2 b3)
	{
		float num = 1f - s;
		float num2 = num * num * num;
		float num3 = s * num * num;
		float num4 = s * s * (1f - s);
		float num5 = s * s * s;
		float x = b0.x * num2 + 3f * b1.x * num3 + 3f * b2.x * num4 + b3.x * num5;
		float y = b0.y * num2 + 3f * b1.y * num3 + 3f * b2.y * num4 + b3.y * num5;
		return new Vector2(x, y);
	}
}

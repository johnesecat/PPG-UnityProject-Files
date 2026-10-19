using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ResizeHandles : MonoBehaviour
{
	private enum Axis
	{
		X,
		Y
	}

	private struct MultiTarget
	{
		public bool Flipped
		{
			[IsReadOnly]
			get;
			set; }

		public Vector2 Scale
		{
			[IsReadOnly]
			get;
			set; }

		public Vector2 Position
		{
			[IsReadOnly]
			get;
			set; }

		public MultiTarget(Transform transform)
		{
			Scale = transform.localScale;
			Position = transform.position;
			Flipped = false;
		}
	}

	public PhysicalBehaviour[] Targets;

	[Space]
	public LineRenderer HorizontalLine;

	public LineRenderer VerticalLine;

	[Space]
	public SpriteRenderer HorizontalHandle;

	public SpriteRenderer VerticalHandle;

	public const float HandleDiameter = 0.2f;

	public Vector2 HandleDistanceMultiplier = Vector2.one;

	public Color HorizontalColour = Color.red;

	public Color VerticalColour = Color.green;

	public Color UniformColour = Color.yellow;

	private MultiTarget[] initialTargets = Array.Empty<MultiTarget>();

	private float multiplyingScale = 1f;

	private bool isDragging;

	private Axis currentDragAxis;

	private Camera camera;

	public bool HasTargets => Targets.Count(IsValidTarget) != 0;

	public bool SingleTarget => Targets.Count(IsValidTarget) == 1;

	public PhysicalBehaviour Target => Targets[0];

	public float HandleRadiusMultiplier => camera.orthographicSize / 5f;

	public bool IsHovering { get; private set; }

	public bool ForceUniformScaling
	{
		get
		{
			if (!InputSystem.Held("fast"))
			{
				if (HasTargets)
				{
					return !SingleTarget;
				}
				return false;
			}
			return true;
		}
	}

	private static bool IsValidTarget(PhysicalBehaviour phys)
	{
		if ((bool)phys && !phys.isDisintegrated)
		{
			return phys.Resizable;
		}
		return false;
	}

	private void Awake()
	{
		camera = Camera.main;
	}

	private void Update()
	{
		if (!HasTargets)
		{
			IsHovering = false;
			base.gameObject.SetActive(value: false);
			return;
		}
		HandleDisplay();
		if (!isDragging)
		{
			CheckForDragStart();
		}
		else if (SingleTarget)
		{
			HandleSingleDrag();
		}
		else
		{
			HandleMultiDrag();
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Targets = Array.Empty<PhysicalBehaviour>();
		}
	}

	public void ResetHandles()
	{
		if (HasTargets && !SingleTarget)
		{
			multiplyingScale = 1f;
			initialTargets = Targets.Select((PhysicalBehaviour t) => new MultiTarget(t.transform)).ToArray();
		}
	}

	private void CheckForDragStart()
	{
		IsHovering = false;
		if (IsMouseHeldNear(HorizontalHandle.transform))
		{
			StartResizeDrag(Axis.X);
		}
		else if (IsMouseHeldNear(VerticalHandle.transform))
		{
			StartResizeDrag(Axis.Y);
		}
	}

	private void StartResizeDrag(Axis axis)
	{
		currentDragAxis = axis;
		isDragging = true;
	}

	private void HandleMultiDrag()
	{
		ResetHandles();
		if (Input.GetMouseButtonUp(0))
		{
			IsHovering = false;
			isDragging = false;
			return;
		}
		IsHovering = true;
		float num = multiplyingScale;
		Vector3 vector = ((currentDragAxis == Axis.X) ? (Vector3.right * num / HandleDistanceMultiplier.x) : (Vector3.up * num / HandleDistanceMultiplier.y));
		float f = Mathf.Sqrt(Vector3.Dot(Vector3.Project(Global.main.MousePosition - base.transform.position, vector), vector));
		if (float.IsNaN(f))
		{
			return;
		}
		multiplyingScale = Mathf.Abs(f);
		for (int i = 0; i < Targets.Length; i++)
		{
			PhysicalBehaviour physicalBehaviour = Targets[i];
			if (!(physicalBehaviour == null))
			{
				MultiTarget multiTarget = initialTargets[i];
				physicalBehaviour.transform.localScale = new Vector3(multiplyingScale * multiTarget.Scale.x * (float)((!multiTarget.Flipped) ? 1 : (-1)), multiplyingScale * multiTarget.Scale.y, 1f);
				physicalBehaviour.transform.position = ((Vector3)multiTarget.Position - base.transform.position) * multiplyingScale + base.transform.position;
				physicalBehaviour.RecalculateMassBasedOnSize();
			}
		}
	}

	private void HandleSingleDrag()
	{
		if (Input.GetMouseButtonUp(0))
		{
			IsHovering = false;
			isDragging = false;
			return;
		}
		IsHovering = true;
		Vector3 localScale = Target.transform.localScale;
		Vector3 vector = ((currentDragAxis == Axis.X) ? (Target.transform.right * localScale.x / HandleDistanceMultiplier.x) : (Target.transform.up * localScale.y / HandleDistanceMultiplier.y));
		Vector3 lhs = Vector3.Project(Global.main.MousePosition - base.transform.position, vector);
		bool forceUniformScaling = ForceUniformScaling;
		float num = Mathf.Sqrt(Vector3.Dot(lhs, vector));
		bool flag = Target.transform.localScale.x < 0f;
		if (float.IsNaN(num))
		{
			return;
		}
		num = Mathf.Max(0.1f, num);
		if (flag && (forceUniformScaling || currentDragAxis == Axis.X))
		{
			num = 0f - Mathf.Abs(num);
		}
		if (forceUniformScaling)
		{
			Target.transform.localScale = new Vector3(num, Mathf.Abs(num), 1f);
		}
		else
		{
			switch (currentDragAxis)
			{
			case Axis.X:
				Target.transform.localScale = new Vector3(num, localScale.y, 1f);
				break;
			case Axis.Y:
				Target.transform.localScale = new Vector3(localScale.x, Mathf.Abs(num), 1f);
				break;
			}
		}
		Target.RecalculateMassBasedOnSize();
	}

	private bool IsMouseHeldNear(Transform transform, float radius = 0.2f)
	{
		radius *= HandleRadiusMultiplier;
		if ((Global.main.MousePosition - transform.position).sqrMagnitude < radius * radius)
		{
			IsHovering = true;
			return Input.GetMouseButtonDown(0);
		}
		return false;
	}

	private void HandleDisplay()
	{
		if (HasTargets)
		{
			Quaternion rotation = Quaternion.identity;
			Vector2 vector;
			Vector2 vector2;
			if (SingleTarget)
			{
				Transform obj = Target.transform;
				vector = obj.position;
				rotation = obj.rotation;
				vector2 = obj.localScale;
			}
			else
			{
				Bounds selectionRect = GetSelectionRect();
				vector = selectionRect.center;
				vector2 = Vector2.one * multiplyingScale;
				Vector3 extents = selectionRect.extents;
				HandleDistanceMultiplier.x = extents.x;
				HandleDistanceMultiplier.y = extents.y;
			}
			base.transform.SetPositionAndRotation(vector, rotation);
			UpdateDisplayAxis(HorizontalHandle.transform, HorizontalLine, HandleDistanceMultiplier.x * vector2.x * Vector3.right);
			UpdateDisplayAxis(VerticalHandle.transform, VerticalLine, HandleDistanceMultiplier.y * vector2.y * Vector3.up);
			bool forceUniformScaling = ForceUniformScaling;
			Color color = (forceUniformScaling ? UniformColour : HorizontalColour);
			Color color2 = (forceUniformScaling ? UniformColour : VerticalColour);
			HorizontalHandle.color = color;
			VerticalHandle.color = color2;
			HorizontalLine.colorGradient = new Gradient
			{
				alphaKeys = new GradientAlphaKey[1]
				{
					new GradientAlphaKey(1f, 0f)
				},
				colorKeys = new GradientColorKey[1]
				{
					new GradientColorKey(color, 0f)
				}
			};
			VerticalLine.colorGradient = new Gradient
			{
				alphaKeys = new GradientAlphaKey[1]
				{
					new GradientAlphaKey(1f, 0f)
				},
				colorKeys = new GradientColorKey[1]
				{
					new GradientColorKey(color2, 0f)
				}
			};
		}
	}

	private void UpdateDisplayAxis(Transform handle, LineRenderer line, Vector3 position)
	{
		float handleRadiusMultiplier = HandleRadiusMultiplier;
		handle.localScale = 0.2f * handleRadiusMultiplier * Vector2.one;
		handle.localPosition = position;
		line.SetPosition(1, handle.localPosition);
		line.widthMultiplier = handleRadiusMultiplier * 2f;
	}

	private Bounds GetSelectionRect()
	{
		float num = float.MaxValue;
		float num2 = float.MaxValue;
		float num3 = float.MinValue;
		float num4 = float.MinValue;
		for (int i = 0; i < Targets.Length; i++)
		{
			Bounds bounds = Targets[i].spriteRenderer.bounds;
			Vector3 max = bounds.max;
			Vector3 min = bounds.min;
			if (max.x > num3)
			{
				num3 = max.x;
			}
			if (max.y > num4)
			{
				num4 = max.y;
			}
			if (min.x < num)
			{
				num = min.x;
			}
			if (min.y < num2)
			{
				num2 = min.y;
			}
		}
		Vector3 vector = new Vector3(Mathf.Abs(num - num3), Mathf.Abs(num2 - num4));
		return new Bounds(new Vector3(num, num2) + vector / 2f, vector);
	}
}

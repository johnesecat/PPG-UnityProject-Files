using System;
using UnityEngine;

public abstract class WireBehaviour : Hover, Messages.ISlice
{
	[SkipSerialisation]
	[HideInInspector]
	public LineRenderer lineRenderer;

	[SkipSerialisation]
	public AnchoredJoint2D untypedJoint;

	[SkipSerialisation]
	[HideInInspector]
	public Transform lineChild;

	public PhysicalBehaviour physicalBehaviour;

	public PhysicalBehaviour otherPhysicalBehaviour;

	[SkipSerialisation]
	protected Vector3[] vertices;

	[SkipSerialisation]
	protected Vector2[] points2d;

	[Obsolete]
	public Vector2 Joint_ConnectedAnchor;

	[Obsolete]
	public Vector2 Joint_Anchor;

	[SkipSerialisation]
	public Material WireMaterial;

	public float WireWidth;

	public Color WireColor = Color.white;

	[SkipSerialisation]
	public readonly Color LegacyColour = new Color(4f / 255f, 0.019607844f, 71f / (339f * (float)Math.PI));

	public string WireMaterialName;

	[SkipSerialisation]
	protected bool shouldIgnoreUpdate;

	protected bool usedToHaveConnectedBody;

	protected Vector2 currentConnectedAnchor;

	protected Vector2 currentAnchor;

	public float currentBreakingForce = float.PositiveInfinity;

	[SkipSerialisation]
	[Obsolete]
	protected EdgeCollider2D EdgeCollider => null;

	private void Awake()
	{
		physicalBehaviour = GetComponent<PhysicalBehaviour>();
		Global.main.LimbStatusToggled += Main_LimbStatusToggled;
	}

	public virtual int GetVertexCount()
	{
		return 12;
	}

	protected void Initialise()
	{
		vertices = new Vector3[GetVertexCount() + 1];
		points2d = new Vector2[vertices.Length];
		lineChild = new GameObject("Wire").transform;
		lineChild.transform.SetParent(base.transform);
		lineChild.transform.localPosition = Vector3.zero;
		lineChild.gameObject.AddComponent<Optout>();
		lineRenderer = lineChild.gameObject.AddComponent<LineRenderer>();
		lineRenderer.positionCount = GetVertexCount() + 1;
		lineRenderer.startColor = WireColor;
		lineRenderer.endColor = WireColor;
		lineRenderer.sortingLayerName = "Top";
		lineRenderer.widthMultiplier = WireWidth;
		lineRenderer.numCapVertices = 6;
		lineRenderer.alignment = LineAlignment.Local;
		lineRenderer.sortingOrder = -2;
		lineRenderer.sharedMaterial = WireMaterial;
		lineRenderer.textureMode = LineTextureMode.Tile;
		if ((bool)untypedJoint && (bool)untypedJoint.connectedBody)
		{
			usedToHaveConnectedBody = true;
			otherPhysicalBehaviour = untypedJoint.connectedBody.GetComponent<PhysicalBehaviour>();
		}
		Created();
		ModAPI.InvokeWireCreated(this, this);
		if ((bool)otherPhysicalBehaviour)
		{
			otherPhysicalBehaviour.BroadcastMessage("OnCableConnect", this, SendMessageOptions.DontRequireReceiver);
		}
		physicalBehaviour.BroadcastMessage("OnCableConnect", this, SendMessageOptions.DontRequireReceiver);
	}

	protected virtual void Update()
	{
		if (Global.main.GetPausedMenu())
		{
			return;
		}
		if (((bool)otherPhysicalBehaviour && otherPhysicalBehaviour.isDisintegrated) || (untypedJoint.connectedBody == null && usedToHaveConnectedBody))
		{
			shouldIgnoreUpdate = true;
			UnityEngine.Object.Destroy(this);
		}
		else if ((bool)lineChild)
		{
			CheckMouseInput();
			if (!IsMouseInsideBounds && Global.main.ShowLimbStatus && !float.IsInfinity(untypedJoint.breakForce))
			{
				float t = Mathf.Sqrt(GetNormalisedStress());
				Color color = Color.Lerp(WireColor, Color.red, t);
				lineRenderer.startColor = color;
				lineRenderer.endColor = color;
			}
		}
		else
		{
			IsMouseInsideBounds = false;
		}
	}

	private float GetNormalisedStress()
	{
		return untypedJoint.reactionForce.sqrMagnitude / (untypedJoint.breakForce * untypedJoint.breakForce);
	}

	public override void OnMouseOverlapEvent(bool overlap)
	{
		base.OnMouseOverlapEvent(overlap);
		if (overlap && UserPreferenceManager.Current.ShowOutlines)
		{
			lineRenderer.sharedMaterial = Resources.Load<Material>("Materials/DeleteWire");
			lineRenderer.gameObject.layer = LayerMask.NameToLayer("ScreenUI");
		}
		else
		{
			lineRenderer.sharedMaterial = WireMaterial;
			lineRenderer.gameObject.layer = LayerMask.NameToLayer("Default");
		}
	}

	private void OnJointBreak2D(Joint2D broken)
	{
		if (broken == untypedJoint)
		{
			JointBroken();
			UnityEngine.Object.Destroy(this);
		}
	}

	protected override void OnDestroy()
	{
		ModAPI.InvokeWireDestroyed(this, this);
		base.OnDestroy();
		if (UndoControllerBehaviour.FindRelevantAction(this, out var result))
		{
			UndoControllerBehaviour.DeregisterAction(result);
		}
		if ((bool)otherPhysicalBehaviour)
		{
			otherPhysicalBehaviour.BroadcastMessage("OnCableDisconnect", this, SendMessageOptions.DontRequireReceiver);
		}
		physicalBehaviour.BroadcastMessage("OnCableDisconnect", this, SendMessageOptions.DontRequireReceiver);
		if ((bool)lineChild)
		{
			UnityEngine.Object.Destroy(lineChild.gameObject);
		}
		UnityEngine.Object.Destroy(untypedJoint);
		UnityEngine.Object.Destroy(this);
		Global.main.LimbStatusToggled -= Main_LimbStatusToggled;
	}

	private void Main_LimbStatusToggled(object sender, bool e)
	{
		if (!e && (bool)lineRenderer)
		{
			lineRenderer.startColor = WireColor;
			lineRenderer.endColor = WireColor;
		}
	}

	private void FixedUpdate()
	{
		Tick();
	}

	public virtual void Slice()
	{
		untypedJoint.breakForce = 0f;
		untypedJoint.breakTorque = 0f;
	}

	public void SetColor(Color color)
	{
		lineRenderer.startColor = color;
		lineRenderer.endColor = color;
		WireColor = color;
	}

	public void SetThickness(float thickness)
	{
		lineRenderer.widthMultiplier = thickness;
	}

	protected override Bounds GetVisualBounds()
	{
		return lineRenderer.bounds;
	}

	protected override bool IsMouseInsideCollider()
	{
		if (!lineRenderer || vertices == null)
		{
			return false;
		}
		bool useWorldSpace = lineRenderer.useWorldSpace;
		Vector3 vector = Global.main.MousePosition;
		if (!useWorldSpace)
		{
			vector = lineRenderer.transform.InverseTransformPoint(vector);
		}
		float num = WireWidth * WireWidth;
		for (int i = 0; i < vertices.Length - 1; i++)
		{
			Vector3 vector2 = vertices[i];
			Vector3 vector3 = vertices[i + 1];
			if (Utils.SqrdDistanceFromPointToLineSegment(vector, vector2, vector3) <= num)
			{
				return true;
			}
		}
		return false;
	}

	protected virtual void JointBroken()
	{
	}

	protected virtual void Tick()
	{
	}

	protected virtual void Created()
	{
	}
}

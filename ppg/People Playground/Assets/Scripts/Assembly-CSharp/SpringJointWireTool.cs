using UnityEngine;

public class SpringJointWireTool : ToolBehaviour
{
	protected LineRenderer lineRenderer;

	protected Vector2 startPos;

	protected Vector2 startOffset;

	protected Vector2 endPos;

	protected Rigidbody2D endBody;

	protected Rigidbody2D startBody;

	protected bool hasDragged;

	protected bool AllowNothingConnection = true;

	protected Material WireMaterial;

	protected float WireWidth = 0.05f;

	protected Color WireColor = Color.white;

	private Collider2D[] overlapBuffer = new Collider2D[8];

	protected virtual bool ShouldPrioritiseInitialGameObject => false;

	protected virtual void Awake()
	{
		WireMaterial = Resources.Load<Material>("Materials/SpringCable");
		WireWidth = 0.2f;
		WireColor = Color.white;
	}

	public override void OnSelect()
	{
		startPos = Global.main.MousePosition;
		if ((bool)ActiveSingleSelected)
		{
			startOffset = ActiveSingleSelected.transform.InverseTransformPoint(startPos);
		}
		lineRenderer.SetPositions(new Vector3[2]
		{
			startPos,
			WireSnapController.GetTransformedWorldEndPoint(Global.main.MousePosition, startPos)
		});
		lineRenderer.enabled = true;
		hasDragged = true;
	}

	public override void OnHold()
	{
		if ((bool)ActiveSingleSelected)
		{
			startBody = ActiveSingleSelected.rigidbody;
			startPos = ActiveSingleSelected.transform.TransformPoint(startOffset);
		}
		lineRenderer.SetPositions(new Vector3[2]
		{
			startPos,
			WireSnapController.GetTransformedWorldEndPoint(Global.main.MousePosition, startPos)
		});
	}

	public override void OnDeselect()
	{
		if (!hasDragged)
		{
			return;
		}
		GetEndObject();
		SpringJoint2D springJoint2D = null;
		if ((ActiveSingleSelected != null && endBody != null) || AllowNothingConnection)
		{
			if ((bool)ActiveSingleSelected)
			{
				springJoint2D = ActiveSingleSelected.gameObject.AddComponent<SpringJoint2D>();
				springJoint2D.autoConfigureConnectedAnchor = false;
				springJoint2D.anchor = ActiveSingleSelected.transform.InverseTransformPoint(startPos);
				if ((bool)endBody)
				{
					if (ActiveSingleSelected == endBody.gameObject)
					{
						Object.Destroy(springJoint2D);
						springJoint2D = null;
					}
					else
					{
						springJoint2D.connectedBody = endBody;
						springJoint2D.connectedAnchor = endBody.transform.InverseTransformPoint(endPos);
					}
				}
				else
				{
					springJoint2D.connectedAnchor = endPos;
				}
			}
			else if ((bool)endBody)
			{
				springJoint2D = endBody.gameObject.AddComponent<SpringJoint2D>();
				springJoint2D.autoConfigureConnectedAnchor = false;
				springJoint2D.anchor = endBody.transform.InverseTransformPoint(endPos);
				springJoint2D.connectedAnchor = startPos;
			}
		}
		if ((bool)springJoint2D)
		{
			springJoint2D.enableCollision = true;
			NonSteamStatManager.Stats.Increment("WIRES_LAID");
			OnJointCreate(springJoint2D);
		}
		lineRenderer.enabled = false;
		hasDragged = false;
		endBody = null;
	}

	protected void GetEndObject()
	{
		endPos = WireSnapController.GetTransformedWorldEndPoint(Global.main.MousePosition, startPos);
		endBody = null;
		PhysicalBehaviour currentlyUnderMouse = SelectionController.Main.CurrentlyUnderMouse;
		if (ShouldPrioritiseInitialGameObject && (bool)startBody)
		{
			if (currentlyUnderMouse != null && currentlyUnderMouse.rigidbody == startBody)
			{
				endBody = startBody;
				return;
			}
			for (int i = 0; i < Physics2D.OverlapPointNonAlloc(Global.main.MousePosition, overlapBuffer, startBody.gameObject.layer); i++)
			{
				if (overlapBuffer[i].attachedRigidbody == startBody)
				{
					endBody = startBody;
					return;
				}
			}
		}
		if ((bool)currentlyUnderMouse && currentlyUnderMouse.gameObject.layer != 11)
		{
			endBody = currentlyUnderMouse.rigidbody;
		}
	}

	public override void OnFixedHold()
	{
	}

	public override void OnToolChosen()
	{
		lineRenderer = base.gameObject.GetComponent<LineRenderer>();
		if (!lineRenderer)
		{
			lineRenderer = base.gameObject.AddComponent<LineRenderer>();
		}
		lineRenderer.enabled = false;
		lineRenderer.startColor = WireColor;
		lineRenderer.endColor = WireColor;
		lineRenderer.sortingLayerName = "Top";
		lineRenderer.widthMultiplier = WireWidth;
		lineRenderer.alignment = LineAlignment.Local;
		lineRenderer.numCapVertices = 0;
		lineRenderer.sortingOrder = 2;
		lineRenderer.sharedMaterial = WireMaterial;
		lineRenderer.textureMode = LineTextureMode.Stretch;
	}

	public override void OnToolUnchosen()
	{
	}

	protected virtual void OnJointCreate(SpringJoint2D joint)
	{
		SpringCableBehaviour springCableBehaviour = joint.gameObject.AddComponent<SpringCableBehaviour>();
		springCableBehaviour.WireColor = WireColor;
		springCableBehaviour.WireMaterial = WireMaterial;
		springCableBehaviour.WireWidth = WireWidth;
		springCableBehaviour.typedJoint = joint;
	}
}

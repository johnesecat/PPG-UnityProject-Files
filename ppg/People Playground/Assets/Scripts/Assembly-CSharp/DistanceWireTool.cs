using UnityEngine;

public abstract class DistanceWireTool : ToolBehaviour
{
	protected LineRenderer lineRenderer;

	private Vector2 startPos;

	private Vector2 startOffset;

	private Vector2 endPos;

	private Rigidbody2D endBody;

	private bool hasDragged;

	protected bool AllowNothingConnection = true;

	protected Material WireMaterial;

	protected float WireWidth = 0.05f;

	protected Color WireColor = Color.white;

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
		DistanceJoint2D distanceJoint2D = null;
		if ((ActiveSingleSelected != null && endBody != null) || AllowNothingConnection)
		{
			if ((bool)ActiveSingleSelected)
			{
				distanceJoint2D = ActiveSingleSelected.gameObject.AddComponent<DistanceJoint2D>();
				distanceJoint2D.autoConfigureConnectedAnchor = false;
				distanceJoint2D.anchor = ActiveSingleSelected.transform.InverseTransformPoint(startPos);
				if ((bool)endBody)
				{
					if (ActiveSingleSelected == endBody.gameObject)
					{
						Object.Destroy(distanceJoint2D);
						distanceJoint2D = null;
					}
					else
					{
						distanceJoint2D.connectedBody = endBody;
						distanceJoint2D.connectedAnchor = endBody.transform.InverseTransformPoint(endPos);
					}
				}
				else
				{
					distanceJoint2D.connectedAnchor = endPos;
				}
			}
			else if ((bool)endBody)
			{
				distanceJoint2D = endBody.gameObject.AddComponent<DistanceJoint2D>();
				distanceJoint2D.autoConfigureConnectedAnchor = false;
				distanceJoint2D.anchor = endBody.transform.InverseTransformPoint(endPos);
				distanceJoint2D.connectedAnchor = startPos;
			}
		}
		if ((bool)distanceJoint2D)
		{
			distanceJoint2D.maxDistanceOnly = true;
			distanceJoint2D.enableCollision = true;
			NonSteamStatManager.Stats.Increment("WIRES_LAID");
			OnJointCreate(distanceJoint2D);
		}
		lineRenderer.enabled = false;
		hasDragged = false;
		endBody = null;
	}

	protected abstract void OnJointCreate(DistanceJoint2D joint);

	private void GetEndObject()
	{
		endPos = WireSnapController.GetTransformedWorldEndPoint(Global.main.MousePosition, startPos);
		endBody = null;
		PhysicalBehaviour currentlyUnderMouse = SelectionController.Main.CurrentlyUnderMouse;
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
		lineRenderer.numCapVertices = 6;
		lineRenderer.sortingOrder = 2;
		lineRenderer.sharedMaterial = WireMaterial;
		lineRenderer.textureMode = LineTextureMode.Tile;
	}

	public override void OnToolUnchosen()
	{
	}
}

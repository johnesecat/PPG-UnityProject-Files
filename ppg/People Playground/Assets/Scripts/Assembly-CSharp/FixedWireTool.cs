using System;
using UnityEngine;

public abstract class FixedWireTool : ToolBehaviour
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

	protected Color WireColor = new Color(4f / 255f, 0.019607844f, 71f / (339f * (float)Math.PI));

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
		FixedJoint2D fixedJoint2D = null;
		if ((ActiveSingleSelected != null && endBody != null) || AllowNothingConnection)
		{
			if ((bool)ActiveSingleSelected)
			{
				fixedJoint2D = ActiveSingleSelected.gameObject.AddComponent<FixedJoint2D>();
				fixedJoint2D.autoConfigureConnectedAnchor = true;
				fixedJoint2D.anchor = ActiveSingleSelected.transform.InverseTransformPoint(startPos);
				if ((bool)endBody)
				{
					if (ActiveSingleSelected == endBody.gameObject)
					{
						UnityEngine.Object.Destroy(fixedJoint2D);
						fixedJoint2D = null;
					}
					else
					{
						fixedJoint2D.connectedBody = endBody;
					}
				}
				else
				{
					fixedJoint2D.connectedAnchor = endPos;
				}
			}
			else if ((bool)endBody)
			{
				fixedJoint2D = endBody.gameObject.AddComponent<FixedJoint2D>();
				fixedJoint2D.autoConfigureConnectedAnchor = true;
				fixedJoint2D.anchor = endBody.transform.InverseTransformPoint(endPos);
			}
		}
		if ((bool)fixedJoint2D)
		{
			fixedJoint2D.enableCollision = true;
			NonSteamStatManager.Stats.Increment("WIRES_LAID");
			OnJointCreate(fixedJoint2D, ((bool)endBody && !fixedJoint2D.connectedBody) ? startPos : endPos);
		}
		lineRenderer.enabled = false;
		hasDragged = false;
		endBody = null;
	}

	protected abstract void OnJointCreate(FixedJoint2D joint, Vector2 worldSpaceEndPos);

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

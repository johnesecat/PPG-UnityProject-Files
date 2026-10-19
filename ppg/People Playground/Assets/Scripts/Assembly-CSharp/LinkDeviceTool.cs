using UnityEngine;

public abstract class LinkDeviceTool : ToolBehaviour
{
	protected LineRenderer lineRenderer;

	private Vector2 startPos;

	private Vector2 startOffset;

	private Vector2 endPos;

	private Rigidbody2D endBody;

	private bool hasDragged;

	protected Material WireMaterial;

	protected float WireWidth = 0.1f;

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
		if (ActiveSingleSelected != null && endBody != null)
		{
			PhysicalBehaviour component = ActiveSingleSelected.GetComponent<PhysicalBehaviour>();
			PhysicalBehaviour component2 = endBody.GetComponent<PhysicalBehaviour>();
			if (component == null || component2 == null)
			{
				return;
			}
			Vector3 vector = ActiveSingleSelected.transform.InverseTransformPoint(startPos);
			Vector3 vector2 = endBody.transform.InverseTransformPoint(WireSnapController.GetTransformedWorldEndPoint(Global.main.MousePosition, startPos));
			OnLinkCreate(vector, vector2, component, component2);
		}
		lineRenderer.enabled = false;
		hasDragged = false;
		endBody = null;
	}

	protected abstract void OnLinkCreate(Vector2 localFrom, Vector2 localTo, PhysicalBehaviour a, PhysicalBehaviour b);

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

using UnityEngine;

public class SerpentineBeltTool : ToolBehaviour
{
	protected LineRenderer lineRenderer;

	private Vector2 startPos;

	private Vector2 startOffset;

	private Vector2 endPos;

	private Rigidbody2D endBody;

	private bool hasDragged;

	public static float WireWidth = 0.03f;

	public static Color WireColor = new Color(0.009f, 0.007f, 0.007f, 1f);

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
		if (hasDragged)
		{
			GetEndObject();
			if ((bool)ActiveSingleSelected && (bool)endBody && endBody.TryGetComponent<PhysicalBehaviour>(out var component))
			{
				SerpentineBeltBehaviour serpentineBeltBehaviour = ActiveSingleSelected.gameObject.AddComponent<SerpentineBeltBehaviour>();
				serpentineBeltBehaviour.Self = ActiveSingleSelected;
				serpentineBeltBehaviour.Other = component;
				NonSteamStatManager.Stats.Increment("WIRES_LAID");
				UndoControllerBehaviour.RegisterAction(new ObjectCreationAction(serpentineBeltBehaviour, "mechanical belt"));
			}
			lineRenderer.enabled = false;
			hasDragged = false;
			endBody = null;
		}
	}

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
		lineRenderer.sharedMaterial = Resources.Load<Material>("Materials/Wire");
		lineRenderer.textureMode = LineTextureMode.Tile;
	}

	public override void OnToolUnchosen()
	{
	}
}

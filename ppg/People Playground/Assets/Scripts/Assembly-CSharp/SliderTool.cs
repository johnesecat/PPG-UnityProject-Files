using System;
using UnityEngine;

[Obsolete]
public class SliderTool : ToolBehaviour
{
	protected LineRenderer lineRenderer;

	protected Vector2 startPos;

	protected Vector2 startOffset;

	protected Vector2 endPos;

	protected Rigidbody2D endBody;

	protected bool hasDragged;

	protected bool AllowNothingConnection = true;

	protected Material WireMaterial;

	protected float WireWidth;

	protected Color WireColor;

	protected virtual void Awake()
	{
		WireMaterial = Resources.Load<Material>("Materials/SlidePiston");
		WireWidth = 0.1f;
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
			Global.main.MousePosition
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
			Global.main.MousePosition
		});
	}

	public override void OnDeselect()
	{
		if (!hasDragged)
		{
			return;
		}
		GetEndObject();
		SliderJoint2D sliderJoint2D = null;
		if ((ActiveSingleSelected != null && endBody != null) || AllowNothingConnection)
		{
			if ((bool)ActiveSingleSelected)
			{
				sliderJoint2D = ActiveSingleSelected.gameObject.AddComponent<SliderJoint2D>();
				sliderJoint2D.autoConfigureConnectedAnchor = false;
				sliderJoint2D.anchor = ActiveSingleSelected.transform.InverseTransformPoint(startPos);
				if ((bool)endBody)
				{
					if (ActiveSingleSelected == endBody.gameObject)
					{
						UnityEngine.Object.Destroy(sliderJoint2D);
						sliderJoint2D = null;
					}
					else
					{
						sliderJoint2D.connectedBody = endBody;
						sliderJoint2D.connectedAnchor = endBody.transform.InverseTransformPoint(endPos);
					}
				}
				else
				{
					sliderJoint2D.connectedAnchor = endPos;
				}
			}
			else if ((bool)endBody)
			{
				sliderJoint2D = endBody.gameObject.AddComponent<SliderJoint2D>();
				sliderJoint2D.autoConfigureConnectedAnchor = false;
				sliderJoint2D.anchor = endBody.transform.InverseTransformPoint(endPos);
				sliderJoint2D.connectedAnchor = startPos;
			}
		}
		if ((bool)sliderJoint2D)
		{
			sliderJoint2D.enableCollision = true;
			sliderJoint2D.autoConfigureAngle = true;
			OnJointCreate(sliderJoint2D);
		}
		lineRenderer.enabled = false;
		hasDragged = false;
		endBody = null;
	}

	protected void GetEndObject()
	{
		endPos = Global.main.MousePosition;
		endBody = null;
		Collider2D collider2D = Physics2D.OverlapPoint(Global.main.MousePosition, LayerMask.GetMask("Debris", "Objects"));
		if ((bool)collider2D && collider2D.gameObject.layer != 11)
		{
			Rigidbody2D component = collider2D.GetComponent<Rigidbody2D>();
			if ((bool)component)
			{
				endBody = component;
			}
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

	protected virtual void OnJointCreate(SliderJoint2D joint)
	{
		SliderJointWireBehaviour sliderJointWireBehaviour = joint.gameObject.AddComponent<SliderJointWireBehaviour>();
		UndoControllerBehaviour.RegisterAction(new ObjectCreationAction(sliderJointWireBehaviour, "slider cable"));
		sliderJointWireBehaviour.WireColor = WireColor;
		sliderJointWireBehaviour.WireMaterial = WireMaterial;
		sliderJointWireBehaviour.WireWidth = WireWidth;
		sliderJointWireBehaviour.typedJoint = joint;
	}
}

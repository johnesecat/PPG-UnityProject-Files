using System;
using UnityEngine;

public class SliderJointWireBehaviour : WireBehaviour
{
	[SkipSerialisation]
	public SliderJoint2D typedJoint;

	private const int VertexCount = 3;

	protected virtual void Start()
	{
		if (!typedJoint)
		{
			typedJoint = base.gameObject.AddComponent<SliderJoint2D>();
			untypedJoint = typedJoint;
			typedJoint.autoConfigureConnectedAnchor = false;
			typedJoint.enableCollision = true;
			if ((bool)otherPhysicalBehaviour)
			{
				typedJoint.connectedBody = otherPhysicalBehaviour.rigidbody;
			}
			if ((bool)typedJoint.connectedBody)
			{
				typedJoint.connectedAnchor = currentConnectedAnchor;
			}
			else
			{
				typedJoint.connectedAnchor = base.transform.root.TransformPoint(currentConnectedAnchor);
			}
			typedJoint.anchor = currentAnchor;
			if (!float.IsInfinity(currentBreakingForce))
			{
				typedJoint.breakForce = currentBreakingForce;
			}
			WireMaterial = Resources.Load<Material>("Materials/" + WireMaterialName);
		}
		else
		{
			untypedJoint = typedJoint;
			currentAnchor = typedJoint.anchor;
			currentBreakingForce = typedJoint.breakForce;
			if ((bool)typedJoint.connectedBody)
			{
				currentConnectedAnchor = typedJoint.connectedAnchor;
			}
			else
			{
				currentConnectedAnchor = base.transform.root.InverseTransformPoint(typedJoint.connectedAnchor);
			}
			WireMaterialName = (WireMaterial ? WireMaterial.name : WireMaterialName);
		}
		typedJoint.autoConfigureAngle = true;
		typedJoint.useLimits = false;
		Initialise();
		lineRenderer.numCapVertices = 0;
		lineRenderer.textureMode = LineTextureMode.RepeatPerSegment;
	}

	public override int GetVertexCount()
	{
		return 2;
	}

	protected override void Update()
	{
		base.Update();
		if (!Global.main.GetPausedMenu() && (bool)lineChild)
		{
			(Vector2, Vector2) points = GetPoints();
			for (int i = 0; i < 3; i++)
			{
				float t = (float)i / 2f;
				Vector3 vector = Vector3.Lerp(points.Item1, points.Item2, t);
				vertices[i] = vector;
			}
			lineRenderer.SetPositions(vertices);
		}
	}

	private (Vector2, Vector2) GetPoints()
	{
		Vector3 vector = base.transform.TransformPoint(untypedJoint.anchor);
		return new ValueTuple<Vector2, Vector2>(item2: (!untypedJoint.connectedBody) ? ((Vector3)untypedJoint.connectedAnchor) : untypedJoint.connectedBody.transform.TransformPoint(untypedJoint.connectedAnchor), item1: vector);
	}

	[Obsolete]
	private void CalculateEdgeCollider()
	{
		for (int i = 0; i < vertices.Length; i++)
		{
			points2d[i] = GetComponent<Collider>().transform.InverseTransformPoint(vertices[i]);
		}
		base.EdgeCollider.points = points2d;
		base.EdgeCollider.edgeRadius = WireWidth / 1.5f;
	}

	public override void OnUserDelete()
	{
	}
}

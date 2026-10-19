using System;
using UnityEngine;

public abstract class FixedJointWireBehaviour : WireBehaviour
{
	[SkipSerialisation]
	public FixedJoint2D typedJoint;

	public Vector2 localRenderEndpoint;

	private bool positionsCached;

	private const float deltaThreshold = 0.001f;

	protected virtual void Start()
	{
		if (!typedJoint)
		{
			typedJoint = base.gameObject.AddComponent<FixedJoint2D>();
			untypedJoint = typedJoint;
			typedJoint.autoConfigureConnectedAnchor = true;
			typedJoint.frequency = 0f;
			typedJoint.dampingRatio = 1f;
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
		typedJoint.enableCollision = false;
		Initialise();
		if ((bool)lineRenderer)
		{
			lineRenderer.transform.localPosition = default(Vector3);
			lineRenderer.transform.localScale = Vector3.one;
			lineRenderer.transform.localRotation = Quaternion.identity;
			lineRenderer.useWorldSpace = false;
		}
	}

	public override int GetVertexCount()
	{
		return 1;
	}

	protected override void Update()
	{
		base.Update();
		if (!shouldIgnoreUpdate)
		{
			Vector3 vector = untypedJoint.anchor;
			Vector3 vector2 = ((!untypedJoint.connectedBody) ? localRenderEndpoint : ((Vector2)base.transform.InverseTransformPoint(untypedJoint.connectedBody.transform.TransformPoint(localRenderEndpoint))));
			if ((vertices[0] - vector).sqrMagnitude > 0.001f || (vertices[1] - vector2).sqrMagnitude > 0.001f)
			{
				vertices[0] = vector;
				vertices[1] = vector2;
				positionsCached = false;
			}
			if (!positionsCached && (bool)lineRenderer)
			{
				lineRenderer.SetPositions(vertices);
				positionsCached = true;
			}
		}
	}

	[Obsolete]
	private void CalculateEdgeCollider()
	{
		for (int i = 0; i < vertices.Length; i++)
		{
			points2d[i] = vertices[i];
		}
		base.EdgeCollider.edgeRadius = WireWidth / 1.5f;
		base.EdgeCollider.points = points2d;
	}

	public override void OnUserDelete()
	{
	}
}

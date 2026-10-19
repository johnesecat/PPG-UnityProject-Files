using System;
using UnityEngine;

public abstract class DistanceJointWireBehaviour : WireBehaviour
{
	public float currentDistance;

	public bool currentMaxDistanceOnly;

	[SkipSerialisation]
	public DistanceJoint2D typedJoint;

	protected virtual void Start()
	{
		if (!typedJoint)
		{
			typedJoint = base.gameObject.AddComponent<DistanceJoint2D>();
			untypedJoint = typedJoint;
			typedJoint.autoConfigureConnectedAnchor = false;
			typedJoint.enableCollision = true;
			typedJoint.autoConfigureDistance = false;
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
			typedJoint.maxDistanceOnly = currentMaxDistanceOnly;
			typedJoint.distance = currentDistance;
			if (!float.IsInfinity(currentBreakingForce))
			{
				typedJoint.breakForce = currentBreakingForce;
			}
			WireMaterial = Resources.Load<Material>("Materials/" + WireMaterialName);
			typedJoint.autoConfigureDistance = true;
		}
		else
		{
			untypedJoint = typedJoint;
			currentAnchor = typedJoint.anchor;
			currentMaxDistanceOnly = typedJoint.maxDistanceOnly;
			currentDistance = typedJoint.distance;
			currentBreakingForce = typedJoint.breakForce;
			if ((bool)typedJoint.connectedBody)
			{
				currentConnectedAnchor = typedJoint.connectedAnchor;
			}
			else
			{
				currentConnectedAnchor = base.transform.root.InverseTransformPoint(typedJoint.connectedAnchor);
			}
			if (!WireMaterial)
			{
				WireMaterial = Resources.Load<Material>("Materials/" + WireMaterialName);
			}
			WireMaterialName = (WireMaterial ? WireMaterial.name : WireMaterialName);
		}
		Initialise();
	}

	protected virtual void CalculateParabola(Vector3 pointA, Vector3 pointB, int vertexCount, ref Vector3[] vertices)
	{
		if (vertexCount > 1)
		{
			float num = typedJoint.distance * typedJoint.distance;
			float num2 = Mathf.Pow(1f - (pointA - pointB).sqrMagnitude / num, 0.65f) * typedJoint.distance / 2f;
			for (int i = 0; i < vertexCount; i++)
			{
				float num3 = (float)i / (float)vertexCount;
				Vector3 vector = Vector3.Lerp(pointA, pointB, num3);
				if (num2 > float.Epsilon)
				{
					vector.y += ParabolaFunction(num3) * num2;
				}
				vertices[i] = vector;
			}
		}
		vertices[0] = pointA;
		vertices[vertexCount] = pointB;
	}

	protected override void Update()
	{
		base.Update();
		if (!shouldIgnoreUpdate)
		{
			Vector3 pointA = base.transform.TransformPoint(untypedJoint.anchor);
			Vector3 pointB = ((!untypedJoint.connectedBody) ? ((Vector3)untypedJoint.connectedAnchor) : untypedJoint.connectedBody.transform.TransformPoint(untypedJoint.connectedAnchor));
			CalculateParabola(pointA, pointB, GetVertexCount(), ref vertices);
			if ((bool)lineChild)
			{
				lineRenderer.SetPositions(vertices);
			}
			currentDistance = typedJoint.distance;
		}
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

	private float ParabolaFunction(float x)
	{
		return Utils.Cosh(2f * x * 1.316958f - 1.316958f) - 2f;
	}

	public override void OnUserDelete()
	{
	}
}

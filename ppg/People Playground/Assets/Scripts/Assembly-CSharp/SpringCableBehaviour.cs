using System;
using System.Linq;
using UnityEngine;

public class SpringCableBehaviour : WireBehaviour
{
	[SkipSerialisation]
	public SpringJoint2D typedJoint;

	public Vector2 localRenderEndpoint;

	private int vertexCount;

	public float targetDistance;

	public float SpringStrength = 2f;

	private readonly Vector2[] points = new Vector2[2];

	protected virtual void Start()
	{
		if (!typedJoint)
		{
			typedJoint = base.gameObject.AddComponent<SpringJoint2D>();
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
			typedJoint.distance = targetDistance;
		}
		else
		{
			untypedJoint = typedJoint;
			currentAnchor = typedJoint.anchor;
			currentBreakingForce = typedJoint.breakForce;
			targetDistance = typedJoint.distance;
			if ((bool)typedJoint.connectedBody)
			{
				currentConnectedAnchor = typedJoint.connectedAnchor;
			}
			else
			{
				currentConnectedAnchor = base.transform.root.InverseTransformPoint(typedJoint.connectedAnchor);
			}
			WireMaterialName = WireMaterial.name;
		}
		typedJoint.autoConfigureDistance = false;
		vertexCount = Mathf.Max(2, Mathf.CeilToInt(typedJoint.distance));
		Initialise();
		lineRenderer.textureMode = LineTextureMode.RepeatPerSegment;
		lineRenderer.numCapVertices = 0;
		typedJoint.frequency = SpringStrength;
	}

	public override int GetVertexCount()
	{
		return vertexCount - 1;
	}

	protected override void Update()
	{
		base.Update();
		if (!Global.main.GetPausedMenu() && (bool)lineChild)
		{
			Vector3 a = base.transform.TransformPoint(untypedJoint.anchor);
			Vector3 b = ((!untypedJoint.connectedBody) ? ((Vector3)untypedJoint.connectedAnchor) : untypedJoint.connectedBody.transform.TransformPoint(untypedJoint.connectedAnchor));
			for (int i = 0; i < vertexCount; i++)
			{
				float t = (float)i / (float)(vertexCount - 1);
				Vector3 vector = Vector3.Lerp(a, b, t);
				vertices[i] = vector;
			}
			lineRenderer.SetPositions(vertices);
		}
	}

	[Obsolete]
	private void CalculateEdgeCollider(Vector3[] vertices)
	{
		points[0] = GetComponent<Collider>().transform.InverseTransformPoint(vertices.First());
		points[1] = GetComponent<Collider>().transform.InverseTransformPoint(vertices.Last());
		base.EdgeCollider.points = points;
		base.EdgeCollider.edgeRadius = WireWidth / 1.5f;
	}

	public override void OnUserDelete()
	{
	}
}

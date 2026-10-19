using UnityEngine;

public class FixedCableTool : FixedWireTool
{
	private void Awake()
	{
		WireMaterial = Resources.Load<Material>("Materials/FixedCable");
		WireWidth = 0.06f;
		WireColor = Color.white;
	}

	protected override void OnJointCreate(FixedJoint2D joint, Vector2 worldSpaceEndpos)
	{
		FixedCableBehaviour fixedCableBehaviour = joint.gameObject.AddComponent<FixedCableBehaviour>();
		UndoControllerBehaviour.RegisterAction(new ObjectCreationAction(fixedCableBehaviour, "fixed cable"));
		fixedCableBehaviour.WireColor = WireColor;
		fixedCableBehaviour.WireMaterial = WireMaterial;
		fixedCableBehaviour.WireWidth = WireWidth;
		fixedCableBehaviour.typedJoint = joint;
		fixedCableBehaviour.Joint_Anchor = joint.anchor;
		fixedCableBehaviour.Joint_ConnectedAnchor = joint.connectedAnchor;
		if ((bool)joint.connectedBody)
		{
			fixedCableBehaviour.localRenderEndpoint = joint.connectedBody.transform.InverseTransformPoint(worldSpaceEndpos);
		}
		else
		{
			fixedCableBehaviour.localRenderEndpoint = joint.transform.InverseTransformPoint(worldSpaceEndpos);
		}
	}
}

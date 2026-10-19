using UnityEngine;

public class SpringWireTool : SpringJointWireTool
{
	protected override void OnJointCreate(SpringJoint2D joint)
	{
		SpringCableBehaviour springCableBehaviour = joint.gameObject.AddComponent<SpringCableBehaviour>();
		springCableBehaviour.WireColor = WireColor;
		springCableBehaviour.WireMaterial = WireMaterial;
		springCableBehaviour.WireWidth = WireWidth;
		springCableBehaviour.typedJoint = joint;
		springCableBehaviour.Joint_Anchor = joint.anchor;
		springCableBehaviour.Joint_ConnectedAnchor = joint.connectedAnchor;
		UndoControllerBehaviour.RegisterAction(new ObjectCreationAction(springCableBehaviour, "spring cable"));
	}
}

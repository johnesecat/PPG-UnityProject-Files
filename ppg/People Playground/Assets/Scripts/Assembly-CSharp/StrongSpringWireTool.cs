using UnityEngine;

public class StrongSpringWireTool : SpringJointWireTool
{
	protected override void Awake()
	{
		base.Awake();
		WireMaterial = Resources.Load<Material>("Materials/StrongSpringCable");
	}

	protected override void OnJointCreate(SpringJoint2D joint)
	{
		SpringCableBehaviour springCableBehaviour = joint.gameObject.AddComponent<SpringCableBehaviour>();
		springCableBehaviour.WireColor = WireColor;
		springCableBehaviour.WireMaterial = WireMaterial;
		springCableBehaviour.WireWidth = WireWidth;
		springCableBehaviour.typedJoint = joint;
		springCableBehaviour.Joint_Anchor = joint.anchor;
		springCableBehaviour.SpringStrength = 8f;
		springCableBehaviour.Joint_ConnectedAnchor = joint.connectedAnchor;
		UndoControllerBehaviour.RegisterAction(new ObjectCreationAction(springCableBehaviour, "strong spring cable"));
	}
}

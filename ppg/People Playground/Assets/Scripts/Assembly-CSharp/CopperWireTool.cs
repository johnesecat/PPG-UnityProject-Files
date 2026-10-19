using UnityEngine;

public class CopperWireTool : DistanceWireTool
{
	private void Awake()
	{
		WireMaterial = Resources.Load<Material>("Materials/CopperWire");
		WireColor = Color.white;
	}

	protected override void OnJointCreate(DistanceJoint2D joint)
	{
		CopperWireBehaviour copperWireBehaviour = joint.gameObject.AddComponent<CopperWireBehaviour>();
		UndoControllerBehaviour.RegisterAction(new ObjectCreationAction(copperWireBehaviour, "copper wire"));
		copperWireBehaviour.WireColor = WireColor;
		copperWireBehaviour.WireMaterial = WireMaterial;
		copperWireBehaviour.WireWidth = WireWidth;
		copperWireBehaviour.typedJoint = joint;
		copperWireBehaviour.typedJoint.breakForce = Utils.CalculateBreakForceForCable(joint, 250f, 1f, 0.5f);
		copperWireBehaviour.Joint_Anchor = joint.anchor;
		copperWireBehaviour.Joint_ConnectedAnchor = joint.connectedAnchor;
	}
}

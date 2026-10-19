using UnityEngine;

public class RigidWireTool : DistanceWireTool
{
	private void Awake()
	{
		WireMaterial = Resources.Load<Material>("Materials/RigidWire");
		WireWidth = 0.08f;
		WireColor = Color.white;
	}

	protected override void OnJointCreate(DistanceJoint2D joint)
	{
		RigidWireBehaviour rigidWireBehaviour = joint.gameObject.AddComponent<RigidWireBehaviour>();
		UndoControllerBehaviour.RegisterAction(new ObjectCreationAction(rigidWireBehaviour, "rigid wire"));
		rigidWireBehaviour.WireColor = WireColor;
		rigidWireBehaviour.WireMaterial = WireMaterial;
		rigidWireBehaviour.WireWidth = WireWidth;
		rigidWireBehaviour.typedJoint = joint;
		rigidWireBehaviour.typedJoint.maxDistanceOnly = false;
		rigidWireBehaviour.Joint_Anchor = joint.anchor;
		rigidWireBehaviour.Joint_ConnectedAnchor = joint.connectedAnchor;
	}
}

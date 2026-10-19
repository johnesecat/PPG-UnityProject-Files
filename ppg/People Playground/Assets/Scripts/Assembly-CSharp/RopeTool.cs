using UnityEngine;

public class RopeTool : DistanceWireTool
{
	private void Awake()
	{
		WireMaterial = Resources.Load<Material>("Materials/Rope");
		WireColor = Color.white;
	}

	protected override void OnJointCreate(DistanceJoint2D joint)
	{
		RopeBehaviour ropeBehaviour = joint.gameObject.AddComponent<RopeBehaviour>();
		UndoControllerBehaviour.RegisterAction(new ObjectCreationAction(ropeBehaviour, "rope"));
		ropeBehaviour.WireColor = WireColor;
		ropeBehaviour.WireMaterial = WireMaterial;
		ropeBehaviour.WireWidth = WireWidth;
		ropeBehaviour.typedJoint = joint;
		ropeBehaviour.typedJoint.breakForce = Utils.CalculateBreakForceForCable(joint, 350f, 1f, 0.5f);
		ropeBehaviour.Joint_Anchor = joint.anchor;
		ropeBehaviour.Joint_ConnectedAnchor = joint.connectedAnchor;
	}
}

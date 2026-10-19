using UnityEngine;

public class HeatPipeTool : DistanceWireTool
{
	private void Awake()
	{
		WireMaterial = Resources.Load<Material>("Materials/HeatPipe");
		WireWidth = 0.1f;
		WireColor = Color.white;
	}

	protected override void OnJointCreate(DistanceJoint2D joint)
	{
		HeatPipeBehaviour heatPipeBehaviour = joint.gameObject.AddComponent<HeatPipeBehaviour>();
		UndoControllerBehaviour.RegisterAction(new ObjectCreationAction(heatPipeBehaviour, "heat pipe"));
		heatPipeBehaviour.WireColor = WireColor;
		heatPipeBehaviour.WireMaterial = WireMaterial;
		heatPipeBehaviour.WireWidth = WireWidth;
		heatPipeBehaviour.typedJoint = joint;
		heatPipeBehaviour.typedJoint.maxDistanceOnly = false;
		heatPipeBehaviour.Joint_Anchor = joint.anchor;
		heatPipeBehaviour.Joint_ConnectedAnchor = joint.connectedAnchor;
	}
}

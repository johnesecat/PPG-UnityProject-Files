using UnityEngine;

public class EnergyWireTool : DistanceWireTool
{
	private void Awake()
	{
		WireMaterial = Resources.Load<Material>("Materials/Wire");
		WireColor = new Color(0f, 0f, 0.03f);
	}

	protected override void OnJointCreate(DistanceJoint2D joint)
	{
		EnergyWireBehaviour energyWireBehaviour = joint.gameObject.AddComponent<EnergyWireBehaviour>();
		UndoControllerBehaviour.RegisterAction(new ObjectCreationAction(energyWireBehaviour, "energy wire"));
		energyWireBehaviour.WireColor = WireColor;
		energyWireBehaviour.WireMaterial = WireMaterial;
		energyWireBehaviour.WireWidth = WireWidth;
		energyWireBehaviour.typedJoint = joint;
		energyWireBehaviour.Joint_Anchor = joint.anchor;
		energyWireBehaviour.Joint_ConnectedAnchor = joint.connectedAnchor;
	}
}

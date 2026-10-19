using UnityEngine;

public class CableWireTool : DistanceWireTool
{
	private void Awake()
	{
		WireMaterial = Resources.Load<Material>("Materials/Cable");
		WireColor = Color.white;
	}

	protected override void OnJointCreate(DistanceJoint2D joint)
	{
		CableBehaviour cableBehaviour = joint.gameObject.AddComponent<CableBehaviour>();
		UndoControllerBehaviour.RegisterAction(new ObjectCreationAction(cableBehaviour, "cable"));
		cableBehaviour.WireColor = WireColor;
		cableBehaviour.WireMaterial = WireMaterial;
		cableBehaviour.WireWidth = WireWidth;
		cableBehaviour.typedJoint = joint;
	}
}

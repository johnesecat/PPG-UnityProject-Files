using UnityEngine;

public class ChainWireTool : DistanceWireTool
{
	private void Awake()
	{
		WireMaterial = Resources.Load<Material>("Materials/ChainWire");
		WireWidth = 0.1f;
		WireColor = Color.white;
	}

	protected override void OnJointCreate(DistanceJoint2D joint)
	{
		ChainBehaviour chainBehaviour = joint.gameObject.AddComponent<ChainBehaviour>();
		UndoControllerBehaviour.RegisterAction(new ObjectCreationAction(chainBehaviour, "chain"));
		chainBehaviour.WireColor = WireColor;
		chainBehaviour.WireMaterial = WireMaterial;
		chainBehaviour.WireWidth = WireWidth;
		chainBehaviour.typedJoint = joint;
	}
}

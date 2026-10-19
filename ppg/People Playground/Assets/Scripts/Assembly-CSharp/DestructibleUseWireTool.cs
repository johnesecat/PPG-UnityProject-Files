using UnityEngine;

public class DestructibleUseWireTool : UseWireTool
{
	protected override void Awake()
	{
		WireColor = Color.white;
		WireMaterial = Resources.Load<Material>("Materials/DestructibleLogicWire");
		WireWidth = 0.05f;
		AllowNothingConnection = false;
	}

	protected override float GetBreakForce(AnchoredJoint2D joint)
	{
		return Utils.CalculateBreakForceForCable(joint, 850f, 1f, 0.5f);
	}
}

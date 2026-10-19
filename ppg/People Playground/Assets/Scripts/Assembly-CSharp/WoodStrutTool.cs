using UnityEngine;

public class WoodStrutTool : DistanceWireTool
{
	private void Awake()
	{
		WireMaterial = Resources.Load<Material>("Materials/WoodStrut");
		WireWidth = 0.08f;
		WireColor = Color.white;
	}

	public override void OnToolChosen()
	{
		base.OnToolChosen();
		lineRenderer.numCapVertices = 0;
	}

	protected override void OnJointCreate(DistanceJoint2D joint)
	{
		WoodStrutBehaviour woodStrutBehaviour = joint.gameObject.AddComponent<WoodStrutBehaviour>();
		UndoControllerBehaviour.RegisterAction(new ObjectCreationAction(woodStrutBehaviour, "wood strut"));
		woodStrutBehaviour.WireColor = WireColor;
		woodStrutBehaviour.WireMaterial = WireMaterial;
		woodStrutBehaviour.WireWidth = WireWidth;
		woodStrutBehaviour.typedJoint = joint;
		woodStrutBehaviour.typedJoint.maxDistanceOnly = false;
		woodStrutBehaviour.typedJoint.breakForce = Utils.CalculateBreakForceForCable(joint, 700f, 1f);
	}
}

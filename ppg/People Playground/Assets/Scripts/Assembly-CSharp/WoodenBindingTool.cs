using UnityEngine;

public class WoodenBindingTool : FixedWireTool
{
	private void Awake()
	{
		WireMaterial = Resources.Load<Material>("Materials/WoodenBinding");
		WireWidth = 0.1f;
		WireColor = Color.white;
	}

	protected override void OnJointCreate(FixedJoint2D joint, Vector2 worldSpaceEndpos)
	{
		WoodenBindingBehaviour woodenBindingBehaviour = joint.gameObject.AddComponent<WoodenBindingBehaviour>();
		UndoControllerBehaviour.RegisterAction(new ObjectCreationAction(woodenBindingBehaviour, "wooden binding"));
		woodenBindingBehaviour.WireColor = WireColor;
		woodenBindingBehaviour.WireMaterial = WireMaterial;
		woodenBindingBehaviour.WireWidth = WireWidth;
		woodenBindingBehaviour.typedJoint = joint;
		woodenBindingBehaviour.typedJoint.breakForce = Utils.CalculateBreakForceForCable(joint, 4000f);
		woodenBindingBehaviour.typedJoint.breakTorque = Utils.CalculateBreakForceForCable(joint, 4000f);
		woodenBindingBehaviour.Joint_Anchor = joint.anchor;
		woodenBindingBehaviour.Joint_ConnectedAnchor = joint.connectedAnchor;
		if ((bool)joint.connectedBody)
		{
			woodenBindingBehaviour.localRenderEndpoint = joint.connectedBody.transform.InverseTransformPoint(worldSpaceEndpos);
		}
		else
		{
			woodenBindingBehaviour.localRenderEndpoint = joint.transform.InverseTransformPoint(worldSpaceEndpos);
		}
	}
}

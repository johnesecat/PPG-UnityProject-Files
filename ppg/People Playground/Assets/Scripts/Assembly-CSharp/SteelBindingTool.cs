using UnityEngine;

public class SteelBindingTool : FixedWireTool
{
	private void Awake()
	{
		WireMaterial = Resources.Load<Material>("Materials/SteelBinding");
		WireWidth = 0.1f;
		WireColor = Color.white;
	}

	protected override void OnJointCreate(FixedJoint2D joint, Vector2 worldSpaceEndpos)
	{
		SteelBindingBehaviour steelBindingBehaviour = joint.gameObject.AddComponent<SteelBindingBehaviour>();
		UndoControllerBehaviour.RegisterAction(new ObjectCreationAction(steelBindingBehaviour, "steel binding"));
		steelBindingBehaviour.WireColor = WireColor;
		steelBindingBehaviour.WireMaterial = WireMaterial;
		steelBindingBehaviour.WireWidth = WireWidth;
		steelBindingBehaviour.typedJoint = joint;
		steelBindingBehaviour.typedJoint.breakForce = Utils.CalculateBreakForceForCable(joint, 6000f);
		steelBindingBehaviour.typedJoint.breakTorque = Utils.CalculateBreakForceForCable(joint, 6000f);
		steelBindingBehaviour.Joint_Anchor = joint.anchor;
		steelBindingBehaviour.Joint_ConnectedAnchor = joint.connectedAnchor;
		if ((bool)joint.connectedBody)
		{
			steelBindingBehaviour.localRenderEndpoint = joint.connectedBody.transform.InverseTransformPoint(worldSpaceEndpos);
		}
		else
		{
			steelBindingBehaviour.localRenderEndpoint = joint.transform.InverseTransformPoint(worldSpaceEndpos);
		}
	}
}

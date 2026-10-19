using UnityEngine;

public class BloodWireTool : DistanceWireTool
{
	private void Awake()
	{
		WireMaterial = Resources.Load<Material>("Materials/BloodWire");
		WireColor = Color.white;
		AllowNothingConnection = false;
	}

	public override void OnToolChosen()
	{
		base.OnToolChosen();
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		lineRenderer.GetPropertyBlock(materialPropertyBlock);
		materialPropertyBlock.SetFloat(ShaderProperties.Get("_Flow"), 0f);
		lineRenderer.SetPropertyBlock(materialPropertyBlock);
	}

	protected override void OnJointCreate(DistanceJoint2D joint)
	{
		BloodWireBehaviour bloodWireBehaviour = joint.gameObject.AddComponent<BloodWireBehaviour>();
		UndoControllerBehaviour.RegisterAction(new ObjectCreationAction(bloodWireBehaviour, "liquid conduit"));
		bloodWireBehaviour.WireColor = WireColor;
		bloodWireBehaviour.WireMaterial = WireMaterial;
		bloodWireBehaviour.WireWidth = WireWidth;
		bloodWireBehaviour.typedJoint = joint;
		bloodWireBehaviour.Joint_Anchor = joint.anchor;
		bloodWireBehaviour.Joint_ConnectedAnchor = joint.connectedAnchor;
	}
}

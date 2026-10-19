using UnityEngine;

public class BandageWireTool : SpringJointWireTool
{
	protected override bool ShouldPrioritiseInitialGameObject => true;

	protected override void Awake()
	{
		base.Awake();
		WireMaterial = Resources.Load<Material>("Materials/BandageWire");
		WireWidth = 0.07f;
		WireColor = Color.white;
	}

	public override void OnToolChosen()
	{
		base.OnToolChosen();
		lineRenderer.sortingLayerName = "Top";
		lineRenderer.sortingOrder = 3;
		lineRenderer.textureMode = LineTextureMode.Tile;
	}

	protected override void OnJointCreate(SpringJoint2D joint)
	{
		AppliedBandageBehaviour appliedBandageBehaviour = joint.gameObject.AddComponent<AppliedBandageBehaviour>();
		appliedBandageBehaviour.WireColor = WireColor;
		appliedBandageBehaviour.WireMaterial = WireMaterial;
		appliedBandageBehaviour.WireWidth = WireWidth;
		appliedBandageBehaviour.typedJoint = joint;
		appliedBandageBehaviour.Joint_Anchor = joint.anchor;
		appliedBandageBehaviour.Joint_ConnectedAnchor = joint.connectedAnchor;
		ModAPI.FindCartridge("9mm");
		UndoControllerBehaviour.RegisterAction(new ObjectCreationAction(appliedBandageBehaviour, "bandage"));
	}
}

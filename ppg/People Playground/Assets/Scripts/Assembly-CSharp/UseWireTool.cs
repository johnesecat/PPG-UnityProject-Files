using UnityEngine;

public class UseWireTool : DistanceWireTool
{
	private const string WireMaterialName = "Materials/LogicWire";

	protected virtual void Awake()
	{
		WireColor = Color.white;
		WireMaterial = Resources.Load<Material>("Materials/LogicWire");
		WireWidth = 0.05f;
		AllowNothingConnection = false;
	}

	public override void OnToolChosen()
	{
		base.OnToolChosen();
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		lineRenderer.GetPropertyBlock(materialPropertyBlock);
		materialPropertyBlock.SetColor(ShaderProperties.Get("_GlowColour"), GetWireColor());
		lineRenderer.SetPropertyBlock(materialPropertyBlock);
	}

	public override void OnToolUnchosen()
	{
		lineRenderer.SetPropertyBlock(new MaterialPropertyBlock());
	}

	protected virtual Color GetWireColor()
	{
		return new Color(1f, 11f, 0f);
	}

	protected virtual ushort GetWireChannel()
	{
		return 0;
	}

	protected virtual float GetBreakForce(AnchoredJoint2D joint)
	{
		return float.PositiveInfinity;
	}

	protected override void OnJointCreate(DistanceJoint2D joint)
	{
		UseWireBehaviour useWireBehaviour = CreateWire(joint);
		useWireBehaviour.Channel = GetWireChannel();
		useWireBehaviour.Color = GetWireColor();
		float breakForce = GetBreakForce(joint);
		useWireBehaviour.CanBreak = !float.IsInfinity(breakForce);
		useWireBehaviour.typedJoint.breakForce = breakForce;
	}

	protected UseWireBehaviour CreateWire(DistanceJoint2D joint)
	{
		UseWireBehaviour useWireBehaviour = joint.gameObject.AddComponent<UseWireBehaviour>();
		UndoControllerBehaviour.RegisterAction(new ObjectCreationAction(useWireBehaviour, "propagation wire"));
		useWireBehaviour.WireColor = WireColor;
		useWireBehaviour.WireMaterial = WireMaterial;
		useWireBehaviour.WireWidth = WireWidth;
		useWireBehaviour.typedJoint = joint;
		useWireBehaviour.Joint_Anchor = joint.anchor;
		useWireBehaviour.Joint_ConnectedAnchor = joint.connectedAnchor;
		return useWireBehaviour;
	}
}

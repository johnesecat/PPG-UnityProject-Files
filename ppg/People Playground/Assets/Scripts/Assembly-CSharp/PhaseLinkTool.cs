using UnityEngine;

public class PhaseLinkTool : LinkDeviceTool
{
	private void Awake()
	{
		WireMaterial = Resources.Load<Material>("Materials/PhaseLink");
		WireWidth = 0.08f;
		WireColor = Color.white;
	}

	protected override void OnLinkCreate(Vector2 localFrom, Vector2 localTo, PhysicalBehaviour a, PhysicalBehaviour b)
	{
		PhaseLinkBehaviour phaseLinkBehaviour = a.gameObject.AddComponent<PhaseLinkBehaviour>();
		UndoControllerBehaviour.RegisterAction(new ObjectCreationAction(phaseLinkBehaviour, "phase link"));
		phaseLinkBehaviour.Other = b;
		phaseLinkBehaviour.LocalOffset = localFrom;
		phaseLinkBehaviour.OtherLocalOffset = localTo;
	}
}

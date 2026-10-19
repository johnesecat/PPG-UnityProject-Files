using UnityEngine;

public class ResistorBehaviour : MonoBehaviour
{
	public float ResistorPower;

	public HingeJoint2D HandleJoint;

	public PhysicalBehaviour PhysicalBehaviour;

	private void FixedUpdate()
	{
		ResistorPower = 1f - Mathf.Clamp01((HandleJoint.jointAngle - HandleJoint.limits.min) / (Mathf.Abs(HandleJoint.limits.min) + HandleJoint.limits.max));
		PhysicalBehaviour.EnergyWireResistance = ResistorPower;
	}

	private void OnDisable()
	{
		PhysicalBehaviour.EnergyWireResistance = 1f;
	}
}

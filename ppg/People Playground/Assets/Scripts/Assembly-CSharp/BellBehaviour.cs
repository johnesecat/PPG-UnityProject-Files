using UnityEngine;

public class BellBehaviour : MonoBehaviour, Messages.IUse
{
	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[HideInInspector]
	public int Counter;

	[SkipSerialisation]
	public float Intensity = 1f;

	[SkipSerialisation]
	public float Offset = 0.1f;

	public void Use(ActivationPropagation activation)
	{
		Counter++;
		PhysicalBehaviour.PlayClipOnce(PhysicalBehaviour.OverrideImpactSounds.PickRandom());
		Vector2 vector = ((Counter % 2 == 0) ? Vector2.left : Vector2.right);
		PhysicalBehaviour.rigidbody.AddForceAtPosition(base.transform.TransformDirection(vector) * Intensity, base.transform.position - base.transform.up * Offset);
	}
}

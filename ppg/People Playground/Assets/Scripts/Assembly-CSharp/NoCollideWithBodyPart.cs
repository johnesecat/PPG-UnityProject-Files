using UnityEngine;

public class NoCollideWithBodyPart : MonoBehaviour
{
	[SkipSerialisation]
	public LimbBehaviour.BodyPart BodyPart;

	[SkipSerialisation]
	public Collider2D Collider;

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!(collision.otherCollider != Collider) && collision.transform.TryGetComponent<LimbBehaviour>(out var component) && component.RoughClassification == BodyPart)
		{
			IgnoreCollisionStackController.IgnoreCollisionSubstituteMethod(Collider, collision.collider);
		}
	}
}

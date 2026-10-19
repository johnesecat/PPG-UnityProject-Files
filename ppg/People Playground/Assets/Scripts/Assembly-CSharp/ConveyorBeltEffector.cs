using UnityEngine;

public class ConveyorBeltEffector : MonoBehaviour
{
	public Collider2D Collider;

	[Space]
	public float Speed = 1f;

	[Range(0f, 1f)]
	public float Strength = 0.1f;

	public Vector2 Direction = Vector2.right;

	[Space]
	public int MaximumBodies = 64;

	private ContactPoint2D[] buffer;

	private Vector2 worldDir;

	private void Awake()
	{
		buffer = new ContactPoint2D[MaximumBodies];
	}

	private void FixedUpdate()
	{
		if (!Mathf.Approximately(0f, Speed))
		{
			worldDir = base.transform.TransformDirection(Direction).normalized;
		}
	}

	private void ActOnContacts()
	{
		int contacts = Collider.GetContacts(buffer);
		for (int i = 0; i < contacts; i++)
		{
			ContactPoint2D contact = buffer[i];
			SlideBody(contact);
		}
	}

	private void SlideBody(ContactPoint2D contact)
	{
		Rigidbody2D rigidbody = contact.rigidbody;
		Rigidbody2D otherRigidbody = contact.otherRigidbody;
		Vector2 vector = worldDir * Speed - contact.relativeVelocity;
		if (rigidbody.bodyType == RigidbodyType2D.Dynamic)
		{
			rigidbody.AddForceAtPosition(vector * rigidbody.mass, contact.point);
		}
		if (otherRigidbody.bodyType == RigidbodyType2D.Dynamic)
		{
			otherRigidbody.AddForceAtPosition(-vector * otherRigidbody.mass, contact.point);
		}
	}
}

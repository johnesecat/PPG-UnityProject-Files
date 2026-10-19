using UnityEngine;

public class PointAtBehaviour : MonoBehaviour
{
	public Rigidbody2D Rigidbody;

	public Transform Target;

	public float Force;

	private void FixedUpdate()
	{
		if (Rigidbody.bodyType == RigidbodyType2D.Dynamic)
		{
			Vector2 normalized = ((Vector2)Target.position - Rigidbody.position).normalized;
			float rotation = Rigidbody.rotation;
			float target = Mathf.Atan2(normalized.y, normalized.x) * 57.29578f;
			float num = Mathf.DeltaAngle(rotation, target);
			if (Mathf.Abs(num) < 25f)
			{
				Rigidbody.angularVelocity *= Mathf.Clamp(num / 25f, 0.95f, 1f);
			}
			Rigidbody.AddTorque(num * Force * normalized.magnitude);
		}
	}
}

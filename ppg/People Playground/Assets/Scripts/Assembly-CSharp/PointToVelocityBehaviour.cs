using UnityEngine;

public class PointToVelocityBehaviour : MonoBehaviour
{
	private Rigidbody2D rb;

	public float intensity = 1f;

	public Vector2 direction = Vector2.right;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		if (rb.bodyType == RigidbodyType2D.Dynamic)
		{
			Vector3 vector = base.transform.TransformVector(direction);
			Vector2 velocity = rb.velocity;
			float current = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			float target = Mathf.Atan2(velocity.y, velocity.x) * 57.29578f;
			float num = Mathf.DeltaAngle(current, target);
			if (Mathf.Abs(num) < 25f)
			{
				rb.angularVelocity *= Mathf.Clamp(num / 25f, 0.95f, 1f);
			}
			rb.AddTorque(num * intensity * velocity.magnitude);
		}
	}
}

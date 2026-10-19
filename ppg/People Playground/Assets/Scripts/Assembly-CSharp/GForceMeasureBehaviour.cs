using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[SkipSerialisation]
public class GForceMeasureBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public float SustainedAccelerationRatio = 0.3f;

	private Rigidbody2D rb;

	private Vector2 velocity;

	private Vector2 velocityLastFrame;

	private Vector2 acceleration;

	private Vector2 sustainedAcceleration;

	[SkipSerialisation]
	public Vector2 Velocity => Velocity;

	[SkipSerialisation]
	public Vector2 VelocityLastFrame => velocityLastFrame;

	[SkipSerialisation]
	public Vector2 Acceleration => acceleration;

	[SkipSerialisation]
	public Vector2 SustainedAcceleration => sustainedAcceleration;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		velocityLastFrame = velocity;
		velocity = rb.velocity;
		acceleration = velocity - velocityLastFrame;
		sustainedAcceleration = Vector2.Lerp(sustainedAcceleration, acceleration, 0.5f);
	}

	private void OnDisable()
	{
		acceleration = Vector2.zero;
		sustainedAcceleration = Vector2.zero;
		velocity = Vector2.zero;
		velocityLastFrame = Vector2.zero;
	}
}

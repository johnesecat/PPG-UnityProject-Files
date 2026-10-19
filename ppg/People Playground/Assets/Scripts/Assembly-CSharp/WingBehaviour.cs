using System;
using UnityEngine;

[Obsolete]
public class WingBehaviour : MonoBehaviour
{
	private Rigidbody2D rigidbody;

	public float LiftForce;

	public Transform liftPosition;

	public ParticleSystem fireParticle;

	public PlaneBehaviour plane;

	private bool ruptured;

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		float value = Mathf.Abs(Vector2.Dot(rigidbody.velocity, base.transform.right)) * (0.01f * rigidbody.velocity.sqrMagnitude);
		value = Mathf.Clamp(value, 0f, 1500f);
		rigidbody.AddForceAtPosition(base.transform.up * LiftForce * value, liftPosition.position);
		float value2 = Mathf.DeltaAngle(base.transform.eulerAngles.z, 57.29578f * Mathf.Atan2(rigidbody.velocity.y, rigidbody.velocity.x));
		value2 = Mathf.Clamp(value2, -10f, 10f);
		rigidbody.AddTorque(value2 * value * LiftForce * 0.1f);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		float normalImpulse = collision.GetContact(0).normalImpulse;
		Debug.Log(normalImpulse);
		if (normalImpulse > 30f)
		{
			FuelTankRupture();
		}
	}

	private void FuelTankRupture()
	{
		if (!ruptured)
		{
			ruptured = true;
			fireParticle.Play();
			plane.enabled = false;
			plane.GetComponent<PhysicalBehaviour>().Ignite();
			ExplosionCreator.CreateFragmentationExplosion(128u, base.transform.position, 25f, 4f, particleAndSound: true, big: true);
		}
	}
}

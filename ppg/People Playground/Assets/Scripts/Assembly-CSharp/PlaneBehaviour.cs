using System;
using UnityEngine;

[Obsolete]
public class PlaneBehaviour : MonoBehaviour
{
	public float speed;

	private bool activated;

	public Rigidbody2D rigidbody;

	private void Use()
	{
		activated = !activated;
	}

	private void FixedUpdate()
	{
		if (activated)
		{
			rigidbody.AddRelativeForce(new Vector2(speed * base.transform.root.localScale.x, 0f), ForceMode2D.Force);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		float normalImpulse = collision.GetContact(0).normalImpulse;
		Debug.Log(normalImpulse);
		if (normalImpulse > 85f)
		{
			BroadcastMessage("FuelTankRupture");
		}
	}
}

using System;
using UnityEngine;

[Obsolete]
public class DroneBehaviour : AliveBehaviour, Messages.IUse, Messages.IShot
{
	public PropellerBehaviour[] propellers;

	public Rigidbody2D rigidbody;

	private PhysicalBehaviour physicalBehaviour;

	public float hoverHeight = 1.8f;

	public float strength = 1f;

	public AudioSource audioSource;

	public bool activated;

	private float health = 25f;

	private void Awake()
	{
		physicalBehaviour = GetComponent<PhysicalBehaviour>();
		audioSource.loop = true;
	}

	private void Start()
	{
		PropellerBehaviour[] array = propellers;
		foreach (PropellerBehaviour obj in array)
		{
			HingeJoint2D hingeJoint2D = (obj.hinge = obj.gameObject.AddComponent<HingeJoint2D>());
			hingeJoint2D.connectedBody = rigidbody;
			hingeJoint2D.useLimits = false;
			hingeJoint2D.useMotor = true;
			hingeJoint2D.autoConfigureConnectedAnchor = false;
			obj.gameObject.AddComponent<HingeJointLimitAutofixBehaviour>();
		}
	}

	public void Use(ActivationPropagation activation)
	{
		activated = !activated;
		if (activated)
		{
			audioSource.Play();
		}
		else
		{
			audioSource.Stop();
		}
	}

	private void Steer(float angle)
	{
		PropellerBehaviour[] array = propellers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetMotorAngle(angle);
		}
	}

	private void SpinPropellers(float intensity = 1f)
	{
		PropellerBehaviour[] array = propellers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].rigidbody.AddRelativeForce(Vector2.up * intensity);
		}
	}

	private void FixedUpdate()
	{
		if (physicalBehaviour.rigidbody.bodyType != 0)
		{
			return;
		}
		audioSource.volume = Mathf.Lerp(audioSource.volume, (!activated || !IsAlive()) ? 0f : (rigidbody.velocity.magnitude * 0.05f + 0.3f), 0.5f);
		if (!IsAlive())
		{
			physicalBehaviour.Charge = 3f;
		}
		else
		{
			if (!activated)
			{
				return;
			}
			RaycastHit2D raycastHit2D = Physics2D.Raycast(base.transform.position, Vector2.down, hoverHeight + 3f);
			if ((bool)raycastHit2D)
			{
				float num = Mathf.Max(hoverHeight - raycastHit2D.distance, 0.1f);
				rigidbody.angularVelocity *= 0.9f;
				SpinPropellers(num * (2f + physicalBehaviour.Charge));
				SpinPropellers(rigidbody.velocity.y * -1f);
				if (!SearchAndDestroy())
				{
					Steer((5f + physicalBehaviour.Charge) * rigidbody.velocity.x);
				}
			}
			else
			{
				Steer(0f);
			}
		}
	}

	private bool SearchAndDestroy()
	{
		RaycastHit2D hit = Physics2D.Raycast(base.transform.position - Vector3.up * 0.4f, Vector2.left, 500f);
		if (!IsValidTarget(hit))
		{
			hit = Physics2D.Raycast(base.transform.position - Vector3.up * 0.4f, Vector2.right, 500f);
		}
		if (!IsValidTarget(hit))
		{
			return false;
		}
		Steer((hit.collider.transform.position.x > base.transform.position.x) ? (-15) : 15);
		return true;
	}

	private bool IsValidTarget(RaycastHit2D hit)
	{
		if (!hit)
		{
			return false;
		}
		if (hit.transform.root == base.transform)
		{
			return false;
		}
		if (!hit.transform.root.GetComponent<AliveBehaviour>())
		{
			return false;
		}
		if (!hit.transform.root.GetComponent<AliveBehaviour>().IsAlive())
		{
			return false;
		}
		return true;
	}

	public void Shot(Shot shot)
	{
		health -= shot.damage;
	}

	public override bool IsAlive()
	{
		return health > 0f;
	}
}

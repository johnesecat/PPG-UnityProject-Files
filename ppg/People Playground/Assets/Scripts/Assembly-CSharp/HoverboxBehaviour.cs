using UnityEngine;

public class HoverboxBehaviour : MonoBehaviour, Messages.IUse
{
	public bool Activated;

	[SkipSerialisation]
	public GameObject Effects;

	[SkipSerialisation]
	public Rigidbody2D Rigidbody;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	private void Start()
	{
		SetActivation();
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			Activated = !Activated;
			SetActivation();
		}
	}

	private void FixedUpdate()
	{
		if (Activated)
		{
			Rigidbody.AddForce(-1f * Rigidbody.gravityScale * Rigidbody.mass * Physics2D.gravity);
			float num = Mathf.Pow(0.9f, Mathf.Max(1f, PhysicalBehaviour.Charge));
			Rigidbody.velocity *= num;
			Rigidbody.angularVelocity *= num;
		}
	}

	private void OnEnable()
	{
		SetActivation();
	}

	private void OnDisable()
	{
		Activated = false;
		SetActivation();
	}

	private void SetActivation()
	{
		Effects.SetActive(Activated);
	}
}

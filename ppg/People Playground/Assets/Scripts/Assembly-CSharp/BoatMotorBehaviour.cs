using UnityEngine;

public class BoatMotorBehaviour : MonoBehaviour, Messages.IUse
{
	public bool Activated;

	public float Force;

	[SkipSerialisation]
	public Vector2 LocalDirection;

	[SkipSerialisation]
	public AudioSource MotorSound;

	public float VisualRotationSpeed = 7000f;

	[SkipSerialisation]
	public CosmeticRotationBehaviour CosmeticRotation;

	private PhysicalBehaviour phys;

	private void Awake()
	{
		phys = GetComponent<PhysicalBehaviour>();
	}

	private void Start()
	{
		if (Activated)
		{
			MotorSound.Play();
		}
		else
		{
			MotorSound.Stop();
		}
		phys.ContextMenuOptions.Buttons.Add(new ContextMenuButton("reverseCar", "Reverse gear", "Reverse vehicle", delegate
		{
			Reverse();
		}));
	}

	private void Update()
	{
		CosmeticRotation.ShouldRotate = Activated;
		CosmeticRotation.RotationSpeedTarget = VisualRotationSpeed;
	}

	private void OnDisable()
	{
		CosmeticRotation.ShouldRotate = false;
		MotorSound.Stop();
	}

	public void Reverse()
	{
		VisualRotationSpeed *= -1f;
		Force *= -1f;
	}

	private void OnEnable()
	{
		if (Activated)
		{
			MotorSound.Play();
		}
	}

	private void FixedUpdate()
	{
		if (phys.IsUnderWater)
		{
			if (Activated)
			{
				Vector2 relativeForce = LocalDirection.normalized * (Force + phys.Charge);
				if (base.transform.localScale.x < 0f)
				{
					relativeForce.x *= -1f;
				}
				phys.rigidbody.AddRelativeForce(relativeForce, ForceMode2D.Force);
			}
			CosmeticRotation.Dampening = 0.3f;
		}
		else
		{
			CosmeticRotation.Dampening = 0.6f;
		}
	}

	public void Use(ActivationPropagation activation)
	{
		if (!base.enabled)
		{
			return;
		}
		if (activation.Channel == 1)
		{
			Reverse();
			return;
		}
		Activated = !Activated;
		if (Activated)
		{
			MotorSound.Play();
		}
		else
		{
			MotorSound.Stop();
		}
	}
}

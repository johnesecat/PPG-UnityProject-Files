using UnityEngine;

public class MagnetBehaviour : MonoBehaviour, Messages.IUse
{
	public float BaseRange = 15f;

	public float BaseIntensity = 5f;

	public float ChargeMultiplierMultiplier = 1f;

	public float JointCreationDistanceThreshold = 0.1f;

	public bool Reversed;

	[SkipSerialisation]
	public ParticleSystem particleSystem;

	[SkipSerialisation]
	public AudioSource audioSource;

	public bool Activated;

	private PhysicalBehaviour PhysicalBehaviour;

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			if (activation.Channel == 1)
			{
				Reversed = !Reversed;
			}
			else
			{
				Activated = !Activated;
			}
			UpdateActivated();
		}
	}

	private void OnDisable()
	{
		Activated = false;
		particleSystem.Stop();
		audioSource.Stop();
	}

	private void UpdateActivated()
	{
		if (Activated)
		{
			audioSource.Play();
			particleSystem.Play();
		}
		else
		{
			particleSystem.Stop();
			audioSource.Stop();
		}
	}

	private void Awake()
	{
		PhysicalBehaviour = GetComponent<PhysicalBehaviour>();
	}

	private void Start()
	{
		PhysicalBehaviour.ContextMenuOptions.Buttons.Add(new ContextMenuButton("reversePolarity", () => (!Reversed) ? "Set to repulse" : "Set to attract", "Reverses the polarity of the magnet", delegate
		{
			Reversed = !Reversed;
		}));
		UpdateActivated();
	}

	private void FixedUpdate()
	{
		if (!Activated)
		{
			return;
		}
		float num = BaseIntensity * Mathf.Max(1f, PhysicalBehaviour.Charge * ChargeMultiplierMultiplier);
		float num2 = BaseRange * Mathf.Max(1f, PhysicalBehaviour.Charge * ChargeMultiplierMultiplier);
		for (int i = 0; i < Global.main.PhysicalObjectsInWorld.Count; i++)
		{
			PhysicalBehaviour physicalBehaviour = Global.main.PhysicalObjectsInWorld[i];
			if (!(physicalBehaviour == PhysicalBehaviour) && physicalBehaviour.Properties.Conducting)
			{
				Vector2 vector = physicalBehaviour.transform.position - base.transform.position;
				if (Reversed)
				{
					vector *= -1f;
				}
				float sqrMagnitude = vector.sqrMagnitude;
				if (!(sqrMagnitude > num2 * num2) && !(sqrMagnitude <= 0.05f))
				{
					Vector2 vector2 = vector.normalized * (0f - num) / sqrMagnitude * physicalBehaviour.Properties.MagneticAttractionIntensity * physicalBehaviour.rigidbody.mass;
					physicalBehaviour.rigidbody.AddForce(vector2, ForceMode2D.Force);
					PhysicalBehaviour.rigidbody.AddForce(-vector2, ForceMode2D.Force);
				}
			}
		}
	}
}

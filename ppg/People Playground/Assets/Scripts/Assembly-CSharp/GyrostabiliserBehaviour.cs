using UnityEngine;

public class GyrostabiliserBehaviour : MonoBehaviour, Messages.IUse
{
	public float Intensity = 1f;

	public bool Activated;

	private PhysicalBehaviour physicalBehaviour;

	[SkipSerialisation]
	public GameObject Light;

	private void Awake()
	{
		physicalBehaviour = GetComponent<PhysicalBehaviour>();
	}

	private void Start()
	{
		Light.SetActive(Activated);
	}

	private void FixedUpdate()
	{
		if (physicalBehaviour.rigidbody.bodyType == RigidbodyType2D.Dynamic)
		{
			float num = (Activated ? (0.3f / Intensity / Mathf.Max(1f, physicalBehaviour.Charge)) : 1f);
			physicalBehaviour.rigidbody.angularVelocity *= num;
			if (Activated)
			{
				float num2 = physicalBehaviour.rigidbody.mass / 1.5f;
				float num3 = Intensity * Mathf.Clamp(physicalBehaviour.Charge, 1f, 5f) * num2 * num2;
				physicalBehaviour.rigidbody.angularVelocity *= Mathf.Pow(0.5f, num3);
				physicalBehaviour.rigidbody.AddTorque(Mathf.DeltaAngle(base.transform.eulerAngles.z, 0f) * num3);
			}
		}
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			Activated = !Activated;
			Light.SetActive(Activated);
		}
	}

	private void OnDisable()
	{
		Light.SetActive(value: false);
	}
}

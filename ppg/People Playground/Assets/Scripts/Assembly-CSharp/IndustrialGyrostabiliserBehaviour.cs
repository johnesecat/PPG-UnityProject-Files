using UnityEngine;

public class IndustrialGyrostabiliserBehaviour : MonoBehaviour, Messages.IUse
{
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
		SetActivated(Activated);
	}

	private void FixedUpdate()
	{
		_ = physicalBehaviour.rigidbody.bodyType;
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			SetActivated(!Activated);
		}
	}

	private void SetActivated(bool activated)
	{
		Activated = activated;
		Light.SetActive(Activated);
		physicalBehaviour.rigidbody.constraints = (Activated ? RigidbodyConstraints2D.FreezeRotation : RigidbodyConstraints2D.None);
	}

	private void OnDisable()
	{
		SetActivated(activated: false);
	}
}

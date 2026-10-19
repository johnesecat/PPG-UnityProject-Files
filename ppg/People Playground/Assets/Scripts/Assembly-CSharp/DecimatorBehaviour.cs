using UnityEngine;

public class DecimatorBehaviour : MonoBehaviour, Messages.IUse
{
	[SkipSerialisation]
	public BoxCollider2D DecimateTrigger;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public AudioSource AudioSource;

	[SkipSerialisation]
	public ParticleSystem ParticleSystem;

	[SkipSerialisation]
	public GameObject DeleteEffect;

	[SkipSerialisation]
	public GameObject BlackHole;

	[HideInInspector]
	public bool Activated;

	private static int lastDestroyedRootId = -1;

	private void Start()
	{
		UpdateActivation();
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			Activated = !Activated;
			UpdateActivation();
		}
	}

	private void FixedUpdate()
	{
		if (PhysicalBehaviour.charge > 50f)
		{
			Object.Instantiate(BlackHole, base.transform.position, Quaternion.identity);
			StatManager.UnlockAchievement("BH");
			Object.Destroy(base.gameObject);
		}
	}

	private void OnDisable()
	{
		Activated = false;
		UpdateActivation();
	}

	private void UpdateActivation()
	{
		DecimateTrigger.enabled = Activated;
		if (Activated)
		{
			ParticleSystem.Play();
			AudioSource.Play();
		}
		else
		{
			ParticleSystem.Stop();
			AudioSource.Stop();
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (base.enabled && Activated && !collision.isTrigger && Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collision.transform, out var value) && value.Deletable && !value.isDisintegrated)
		{
			int instanceID = value.transform.root.GetInstanceID();
			if (lastDestroyedRootId != instanceID)
			{
				lastDestroyedRootId = instanceID;
				Object.Destroy(value.transform.root.gameObject);
				Object.Instantiate(DeleteEffect, value.transform.position, Quaternion.identity);
			}
		}
	}
}

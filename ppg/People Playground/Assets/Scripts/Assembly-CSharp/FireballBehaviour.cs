using UnityEngine;

public class FireballBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public GameObject ExplosionEffect;

	[SkipSerialisation]
	public float Range;

	[SkipSerialisation]
	public ParticleSystem ParticleSystem;

	[SkipSerialisation]
	public Rigidbody2D Rigidbody;

	[SkipSerialisation]
	public DeleteAfterTime DAT;

	private Collider2D[] cols;

	private bool hitOnce;

	private void Awake()
	{
		cols = new Collider2D[24];
	}

	public void OnCollisionEnter2D(Collision2D collision)
	{
		if (hitOnce)
		{
			return;
		}
		hitOnce = true;
		int num = Physics2D.OverlapCircleNonAlloc(base.transform.position, Range, cols);
		for (int i = 0; i < num; i++)
		{
			Collider2D collider2D = cols[i];
			if (!collider2D.transform.TryGetComponent<PhysicalBehaviour>(out var component))
			{
				continue;
			}
			float num2 = (Range - Vector2.Distance(base.transform.position, collider2D.transform.position)) / Range;
			if (component.Properties.Flammability >= 0.05f)
			{
				if (Random.value > num2)
				{
					component.Ignite();
					component.BurnIntensity = num2 * 0.75f + Random.value / 5f;
				}
				component.BurnProgress += num2 * 0.25f;
			}
			component.rigidbody.AddForce(15f * num2 * (collider2D.transform.position - base.transform.position).normalized);
		}
		Object.Instantiate(ExplosionEffect, base.transform.position, Quaternion.identity);
		for (int j = 0; j < base.transform.childCount; j++)
		{
			ParticleSystem.EmissionModule emission = base.transform.GetChild(j).gameObject.GetComponent<ParticleSystem>().emission;
			emission.rateOverTime = 0f;
			emission.rateOverDistance = 0f;
		}
		ParticleSystem.EmissionModule emission2 = ParticleSystem.emission;
		emission2.rateOverTime = 0f;
		emission2.rateOverDistance = 0f;
		Rigidbody.isKinematic = true;
		Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
		DAT.enabled = true;
	}
}

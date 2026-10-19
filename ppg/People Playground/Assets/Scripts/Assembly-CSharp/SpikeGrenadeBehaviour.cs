using System.Collections;
using UnityEngine;

public class SpikeGrenadeBehaviour : MonoBehaviour, Messages.IUse
{
	[SkipSerialisation]
	public int SpikeCount = 12;

	[SkipSerialisation]
	public AudioClip IgniteSound;

	[SkipSerialisation]
	public GameObject SpikePrefab;

	[SkipSerialisation]
	public GameObject ExplosionVFX;

	[SkipSerialisation]
	public float DirectRange = 4f;

	[SkipSerialisation]
	public SpriteRenderer Light;

	[SkipSerialisation]
	public float SpikeForce = 10f;

	[SkipSerialisation]
	public float Delay = 2f;

	[SkipSerialisation]
	public LayerMask CollisionLayers;

	[HideInInspector]
	public bool Ignited;

	private void Start()
	{
		Light.enabled = false;
		if (Ignited)
		{
			StartCoroutine(IgniteRoutine());
		}
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			Ignite();
		}
	}

	public void Ignite()
	{
		if (!Ignited)
		{
			Ignited = true;
			StartCoroutine(IgniteRoutine());
		}
	}

	private IEnumerator IgniteRoutine()
	{
		Light.enabled = true;
		PhysicalBehaviour phys = GetComponent<PhysicalBehaviour>();
		phys.PlayClipOnce(IgniteSound, 2f);
		yield return new WaitForSeconds(Delay);
		Vector3 position = base.transform.position;
		for (int i = 0; i < SpikeCount; i++)
		{
			Vector3 vector = Random.insideUnitCircle.normalized;
			Physics2D.Raycast(position, vector, DirectRange, CollisionLayers);
			GameObject obj = Object.Instantiate(SpikePrefab, position, Quaternion.identity);
			obj.GetOrAddComponent<AudioSourceTimeScaleBehaviour>();
			PhysicalBehaviour component = obj.GetComponent<PhysicalBehaviour>();
			obj.transform.right = vector;
			component.rigidbody.AddForce(Random.Range(0.5f, 1.3f) * SpikeForce * vector, ForceMode2D.Impulse);
			component.Temperature = phys.Temperature;
		}
		ExplosionCreator.CreateExplosionWithWater(WaterBehaviour.IsPointUnderWater(base.transform.position), new ExplosionCreator.ExplosionParameters(1u, base.transform.position, 0f, 0f, createFx: false));
		Object.Instantiate(ExplosionVFX, base.transform.position, Quaternion.identity);
		Object.Destroy(base.gameObject);
	}
}

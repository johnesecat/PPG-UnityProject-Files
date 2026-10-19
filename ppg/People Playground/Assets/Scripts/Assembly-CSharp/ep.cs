using UnityEngine;

[NotDocumented]
public class ep : MonoBehaviour
{
	public SpawnableAsset ToSpawn;

	private float t;

	public float duration;

	private void Update()
	{
		t += Time.deltaTime / duration;
		if (t >= 1f)
		{
			End();
		}
	}

	private void FixedUpdate()
	{
		CameraShakeBehaviour.main.Shake(t * 0.5f, base.transform.position, 0.7f);
		if (!((double)Random.value > 0.9900000095367432 - (double)t * 0.15))
		{
			return;
		}
		foreach (PhysicalBehaviour item in Global.main.PhysicalObjectsInWorld)
		{
			if (!((double)Random.value > 0.5))
			{
				if (item.TryGetComponent<GlowtubeBehaviour>(out var component))
				{
					component.Use(new ActivationPropagation());
				}
				if (item.TryGetComponent<BulbBehaviour>(out var component2))
				{
					component2.Use(new ActivationPropagation());
				}
			}
		}
	}

	private void End()
	{
		ExplosionCreator.Explode(new ExplosionCreator.ExplosionParameters(32u, base.transform.position, 30f, 3500f, createFx: true, big: true));
		foreach (PhysicalBehaviour item in Global.main.PhysicalObjectsInWorld)
		{
			item.SendMessage("OnEMPHit", SendMessageOptions.DontRequireReceiver);
		}
		base.gameObject.SetActive(value: false);
		GameObject obj = Object.Instantiate(ToSpawn.Prefab, base.transform.position, Quaternion.identity);
		obj.AddComponent<AudioSourceTimeScaleBehaviour>();
		obj.AddComponent<SerialiseInstructions>().OriginalSpawnableAsset = ToSpawn;
		Save();
	}

	private void Save()
	{
		ItemPersistence.Add(ToSpawn.name);
		StatManager.IncrementInteger(StatManager.Stat.KEYPAD);
	}
}

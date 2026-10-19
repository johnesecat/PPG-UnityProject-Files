using UnityEngine;

public class AtomBombBehaviour : MonoBehaviour, Messages.IUse
{
	[SkipSerialisation]
	public GameObject ExplosionPrefab;

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			NonSteamStatManager.Stats.Increment("NUCLEAR_EXPLOSIONS");
			Object.Instantiate(ExplosionPrefab, base.transform.position, Quaternion.identity);
			Object.Destroy(base.gameObject);
		}
	}
}

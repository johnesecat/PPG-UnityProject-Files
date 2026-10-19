using UnityEngine;

public class SeismicChargeBehaviour : MonoBehaviour, Messages.IUse
{
	public GameObject ExplosionPrefab;

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			Object.Instantiate(ExplosionPrefab, base.transform.position, Quaternion.identity);
			Object.Destroy(base.gameObject);
		}
	}
}

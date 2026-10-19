using UnityEngine;

public class AndroidSparkCreator : MonoBehaviour
{
	public GameObject spark;

	private LimbBehaviour limb;

	private void Awake()
	{
		limb = GetComponent<LimbBehaviour>();
	}

	private void FixedUpdate()
	{
		if (limb.Health < limb.InitialHealth * 0.1f && Random.value > 0.9995f)
		{
			Object.Instantiate(spark, base.transform.position, Quaternion.identity);
		}
	}
}

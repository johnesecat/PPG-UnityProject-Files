using System.Collections;
using UnityEngine;

public class StaticNeedleBehaviour : MonoBehaviour
{
	public LimbBehaviour limb;

	public GameObject explosion;

	private void Start()
	{
		if ((bool)limb)
		{
			limb.SendMessage("Shot", new Shot(base.transform.right * -1f, base.transform.position, 0.3f));
			if (limb.GetComponentsInChildren<StaticNeedleBehaviour>().Length > 6)
			{
				Object.Instantiate(Resources.Load("Prefabs/NeedlerSuperExplosion"), base.transform.position, Quaternion.identity);
				ExplosiveBehaviour component = GetComponent<ExplosiveBehaviour>();
				component.FragmentForce = 1f;
				component.FragmentationRayCount = 32u;
				component.Range = 1f;
				component.Activate();
			}
			else
			{
				StartCoroutine(WaitAndExplode());
			}
		}
		else
		{
			base.gameObject.AddComponent<DeleteAfterTime>().Life = 5f;
		}
	}

	private IEnumerator WaitAndExplode()
	{
		yield return new WaitForSeconds(3f);
		Object.Instantiate(explosion, base.transform.position, Quaternion.identity);
		GetComponent<ExplosiveBehaviour>().Activate();
	}
}

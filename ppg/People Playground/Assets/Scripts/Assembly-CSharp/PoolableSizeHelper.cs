using UnityEngine;

public class PoolableSizeHelper : MonoBehaviour, Messages.IOnPoolableReinitialised
{
	private bool initialised;

	private Vector3 localScaleAtStart;

	private void Awake()
	{
		if (!initialised)
		{
			initialised = true;
			localScaleAtStart = base.transform.localScale;
		}
	}

	public void OnPoolableReinitialised(ObjectPoolBehaviour pool)
	{
		base.transform.localScale = localScaleAtStart;
	}
}

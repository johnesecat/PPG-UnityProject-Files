using UnityEngine;

public class ReturnToPoolAfterTime : MonoBehaviour, Messages.IOnPoolableInitialised
{
	public float Life = 1f;

	private ObjectPoolBehaviour pool;

	private float time;

	[SkipSerialisation]
	public float Progress => time / Life;

	private void Update()
	{
		time += Time.deltaTime;
		if (time > Life)
		{
			pool.Return(base.gameObject);
		}
	}

	public void OnPoolableInitialised(ObjectPoolBehaviour pool)
	{
		time = 0f;
		this.pool = pool;
	}
}

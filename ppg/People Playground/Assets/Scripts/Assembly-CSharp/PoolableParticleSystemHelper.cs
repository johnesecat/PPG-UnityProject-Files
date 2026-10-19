using System;
using UnityEngine;

public class PoolableParticleSystemHelper : MonoBehaviour, Messages.IOnPoolableInitialised
{
	private ObjectPoolBehaviour pool;

	private void Awake()
	{
		ParticleSystem.MainModule main = GetComponent<ParticleSystem>().main;
		main.stopAction = ParticleSystemStopAction.Callback;
	}

	public void OnPoolableInitialised(ObjectPoolBehaviour pool)
	{
		this.pool = pool;
	}

	private void OnParticleSystemStopped()
	{
		if (!pool)
		{
			throw new Exception("Poolable object is unpooled!");
		}
		pool.Return(base.gameObject);
	}
}

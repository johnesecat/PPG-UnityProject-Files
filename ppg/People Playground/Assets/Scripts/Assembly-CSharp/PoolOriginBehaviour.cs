using System;
using UnityEngine;

[Obsolete]
public class PoolOriginBehaviour : MonoBehaviour, Messages.IOnPoolableInitialised
{
	[SkipSerialisation]
	public ObjectPoolBehaviour OriginPool;

	public void OnPoolableInitialised(ObjectPoolBehaviour pool)
	{
		OriginPool = pool;
	}
}

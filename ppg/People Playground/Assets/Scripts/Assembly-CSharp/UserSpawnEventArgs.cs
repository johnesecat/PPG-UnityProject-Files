using System;
using UnityEngine;

public class UserSpawnEventArgs : EventArgs
{
	public readonly GameObject Instance;

	public readonly SpawnableAsset SpawnableAsset;

	public UserSpawnEventArgs(GameObject instance, SpawnableAsset spawnableAsset)
	{
		Instance = instance;
		SpawnableAsset = spawnableAsset;
	}
}

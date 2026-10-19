using System;
using Steamworks;
using UnityEngine;

public class SteamworksInitialiser : MonoBehaviour
{
	public static bool IsInitialised;

	public const int AppID = 1118200;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		UnityEngine.Object.DontDestroyOnLoad(this);
		if (!IsInitialised)
		{
			try
			{
				SteamClient.Init(1118200u);
				SteamClient.RestartAppIfNecessary(1118200u);
				IsInitialised = true;
				Debug.Log("Steamworks initialised");
				Debug.Log("Steam login: " + SteamClient.IsLoggedOn);
			}
			catch (Exception message)
			{
				IsInitialised = false;
				Debug.LogWarning(message);
			}
		}
	}

	private void Update()
	{
		SteamClient.RunCallbacks();
	}

	private void OnDestroy()
	{
		SteamClient.Shutdown();
	}
}

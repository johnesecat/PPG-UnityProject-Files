using UnityEngine;

public class ExistIfSteam : MonoBehaviour
{
	public enum ShowBehaviour
	{
		IsSteamBuild,
		SteamIsInitialised,
		SteamIsNotInitialised
	}

	public ShowBehaviour Behaviour = ShowBehaviour.SteamIsInitialised;

	private void Awake()
	{
		switch (Behaviour)
		{
		case ShowBehaviour.IsSteamBuild:
			base.gameObject.SetActive(value: true);
			break;
		case ShowBehaviour.SteamIsInitialised:
			base.gameObject.SetActive(SteamworksInitialiser.IsInitialised);
			break;
		case ShowBehaviour.SteamIsNotInitialised:
			base.gameObject.SetActive(!SteamworksInitialiser.IsInitialised);
			break;
		}
	}
}

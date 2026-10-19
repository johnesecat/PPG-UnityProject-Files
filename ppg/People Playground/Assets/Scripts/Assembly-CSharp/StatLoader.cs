using UnityEngine;

public class StatLoader : MonoBehaviour
{
	private void Start()
	{
		NonSteamStatManager.LoadFromFile("stats");
	}
}

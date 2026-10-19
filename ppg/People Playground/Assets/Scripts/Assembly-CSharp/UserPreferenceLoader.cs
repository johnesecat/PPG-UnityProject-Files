using UnityEngine;

public class UserPreferenceLoader : MonoBehaviour
{
	private void Awake()
	{
		UserPreferenceManager.Load();
	}

	private void OnDestroy()
	{
		UserPreferenceManager.Save();
	}
}

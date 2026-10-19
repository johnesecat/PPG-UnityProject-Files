using UnityEngine;

public class ActivateIfTutorial : MonoBehaviour
{
	private void Awake()
	{
		base.gameObject.SetActive(UserPreferenceManager.Current.ShowTutorial);
		if (base.gameObject.activeInHierarchy)
		{
			UserPreferenceManager.Current.ShowTutorial = false;
		}
	}
}

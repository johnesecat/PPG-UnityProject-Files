using UnityEngine;

public class PageSwitchButtonBehaviour : MonoBehaviour
{
	public string pageName;

	public void Switch()
	{
		MenuController.SetPage(pageName);
	}
}

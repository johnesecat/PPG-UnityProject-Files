using UnityEngine;

public class MenuController : MonoBehaviour
{
	public GameObject[] pages;

	private GameObject currentPage;

	public string ControlPageName;

	internal static MenuController main;

	public GameObject CurrentPage
	{
		get
		{
			return currentPage;
		}
		private set
		{
			currentPage = value;
			GameObject[] array = pages;
			foreach (GameObject gameObject in array)
			{
				gameObject.transform.localScale = ((gameObject == currentPage) ? Vector3.one : Vector3.zero);
				if (gameObject == currentPage)
				{
					gameObject.BroadcastMessage("OnPageSelected", SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	private void Awake()
	{
		main = this;
		main.CurrentPage = main.pages[0];
		GameObject[] array = pages;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: true);
		}
	}

	public static void SetPage(string pageName)
	{
		if (!(main == null))
		{
			GameObject pageByName = main.GetPageByName(pageName);
			if (!(pageByName == null))
			{
				main.CurrentPage = pageByName;
			}
		}
	}

	private GameObject GetPageByName(string name)
	{
		GameObject[] array = pages;
		foreach (GameObject gameObject in array)
		{
			if (gameObject.name == name)
			{
				return gameObject;
			}
		}
		return null;
	}
}

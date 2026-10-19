using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitchBehaviour : MonoBehaviour
{
	public string SceneName;

	private LoadingBarBehaviour loadingBar;

	private void Awake()
	{
		LoadingBarBehaviour[] array = Resources.FindObjectsOfTypeAll<LoadingBarBehaviour>();
		if (array != null && array.Length != 0)
		{
			loadingBar = array[0];
		}
	}

	public void Switch()
	{
		AsyncOperation task = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
		if ((bool)loadingBar)
		{
			MenuController.SetPage("Loading");
			StartCoroutine(UpdateProgress(task));
		}
	}

	private IEnumerator UpdateProgress(AsyncOperation task)
	{
		while (!task.isDone)
		{
			loadingBar.SetProgress(task.progress);
			yield return new WaitForEndOfFrame();
		}
	}
}

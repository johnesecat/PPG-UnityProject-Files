using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToScene : MonoBehaviour
{
	public string Scene;

	private void Start()
	{
		StartCoroutine(GoTo());
	}

	private IEnumerator GoTo()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		SceneManager.LoadScene(Scene);
	}
}

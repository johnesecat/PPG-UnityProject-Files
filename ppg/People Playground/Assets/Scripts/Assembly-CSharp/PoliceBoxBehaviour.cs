using System.Collections;
using UnityEngine;

public class PoliceBoxBehaviour : MonoBehaviour
{
	public SpriteRenderer SpriteRenderer;

	public float Duration = 1f;

	private bool isBusy;

	private void OnBecameVisible()
	{
		if (!isBusy)
		{
			StartCoroutine(FadeRoutine());
		}
	}

	private IEnumerator FadeRoutine()
	{
		Color clear = new Color(1f, 1f, 1f, 0f);
		float t = 0f;
		isBusy = true;
		while (true)
		{
			t += Time.deltaTime;
			float t2 = t / Duration;
			Color color = Color.Lerp(Color.white, clear, t2);
			SpriteRenderer.color = color;
			if (t > Duration)
			{
				break;
			}
			yield return new WaitForEndOfFrame();
		}
		base.gameObject.SetActive(value: false);
	}
}

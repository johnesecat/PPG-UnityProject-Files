using TMPro;
using UnityEngine;

public class FramerateViewBehaviour : MonoBehaviour
{
	public TextMeshProUGUI Text;

	private int framesRenderedLastSecond;

	private float t;

	private void Update()
	{
		framesRenderedLastSecond++;
		t += Time.unscaledDeltaTime;
		if (t > 1f)
		{
			Text.text = framesRenderedLastSecond + " fps";
			framesRenderedLastSecond = 0;
			t = 0f;
		}
	}
}

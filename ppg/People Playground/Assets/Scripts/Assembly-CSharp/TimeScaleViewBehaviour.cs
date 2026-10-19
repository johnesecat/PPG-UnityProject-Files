using TMPro;
using UnityEngine;

public class TimeScaleViewBehaviour : MonoBehaviour
{
	public TextMeshProUGUI Text;

	private void Update()
	{
		Text.text = Mathf.Round(Time.timeScale * 100f) + "%";
	}
}

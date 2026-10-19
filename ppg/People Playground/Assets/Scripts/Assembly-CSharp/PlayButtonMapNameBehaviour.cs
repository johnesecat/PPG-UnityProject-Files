using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayButtonMapNameBehaviour : MonoBehaviour
{
	private TextMeshProUGUI text;

	private Button button;

	private void Awake()
	{
		text = GetComponentInChildren<TextMeshProUGUI>();
		button = GetComponentInChildren<Button>();
	}

	private void Update()
	{
		if ((bool)MapLoaderBehaviour.CurrentMap)
		{
			text.text = "Enter " + MapLoaderBehaviour.CurrentMap.name;
			text.color = Color.white;
			button.interactable = true;
		}
		else
		{
			text.text = "Select a chamber";
			text.color = Color.white * 0.5f;
			button.interactable = false;
		}
	}
}

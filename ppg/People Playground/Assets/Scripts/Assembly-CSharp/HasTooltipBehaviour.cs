using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HasTooltipBehaviour : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public TextMeshProUGUI TooltipText;

	[Multiline]
	public string Text;

	public void OnPointerExit(PointerEventData eventData)
	{
		if (base.gameObject.activeSelf && (bool)TooltipText)
		{
			TooltipText.gameObject.SetActive(value: false);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (base.gameObject.activeSelf && (bool)TooltipText)
		{
			TooltipText.gameObject.SetActive(value: true);
			TooltipText.text = AppendPunctuation(Text);
		}
	}

	private string AppendPunctuation(string input)
	{
		string[] obj = new string[11]
		{
			".", "!", "¡", "?", "~", ";", ":", "\"", "'", ")",
			"]"
		};
		input = input.Trim();
		string[] array = obj;
		foreach (string value in array)
		{
			if (input.EndsWith(value))
			{
				return input;
			}
		}
		return input + ".";
	}
}

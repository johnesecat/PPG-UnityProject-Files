using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TextMeshProHyperlink : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerExitHandler, IPointerEnterHandler
{
	[Serializable]
	public struct LinkTarget
	{
		public string Name;

		public string Target;

		public UnityEvent Event;
	}

	public LinkTarget[] Targets;

	private TextMeshProUGUI text;

	private TMP_LinkInfo[] links;

	private bool hovering;

	public void OnPointerClick(PointerEventData eventData)
	{
		Debug.Log("wtf");
		if (!TryGetLink(Input.mousePosition, out var linkInfo))
		{
			return;
		}
		string id = linkInfo.GetLinkID();
		LinkTarget linkTarget = Targets.FirstOrDefault((LinkTarget t) => t.Name == id);
		if (linkTarget.Name == id)
		{
			linkTarget.Event?.Invoke();
			if (linkTarget.Target != null)
			{
				Application.OpenURL(linkTarget.Target);
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		hovering = true;
		TryGetLink(Input.mousePosition, out var _);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		hovering = false;
		TryGetLink(Input.mousePosition, out var _);
	}

	private void Awake()
	{
		text = GetComponent<TextMeshProUGUI>();
		links = GetComponent<TextMeshProUGUI>().textInfo.linkInfo;
	}

	private bool TryGetLink(Vector2 pos, out TMP_LinkInfo linkInfo)
	{
		linkInfo = default(TMP_LinkInfo);
		int num = TMP_TextUtilities.FindIntersectingLink(text, pos, Camera.main);
		if (num >= 0 && num < links.Length)
		{
			linkInfo = links[num];
			return true;
		}
		return false;
	}
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolBarGenerator : MonoBehaviour
{
	public TextMeshProUGUI ToolTipTextMesh;

	public Transform ToolContainer;

	public Transform PowerContainer;

	private void Start()
	{
		GenerateButtons();
		ToolLibrary.OnCollectionChange.AddListener(GenerateButtons);
	}

	private void GenerateButtons()
	{
		foreach (Transform item in ToolContainer)
		{
			Object.Destroy(item.gameObject);
		}
		foreach (Transform item2 in PowerContainer)
		{
			Object.Destroy(item2.gameObject);
		}
		GenerateButtonsFor(ToolLibrary.Instance.Tools, ToolContainer);
		GenerateButtonsFor(ToolLibrary.Instance.Powers, PowerContainer);
	}

	private void GenerateButtonsFor(List<ToolLibrary.Tool> list, Transform container)
	{
		foreach (ToolLibrary.Tool item in list)
		{
			if (string.IsNullOrEmpty(item.Parent))
			{
				CreateButton(item, container, list);
			}
		}
	}

	private GameObject CreateButton(ToolLibrary.Tool tool, Transform container, List<ToolLibrary.Tool> allEntries)
	{
		GameObject gameObject = new GameObject(tool.Name);
		gameObject.transform.SetParent(container);
		gameObject.transform.localScale = Vector3.one;
		Image image = gameObject.AddComponent<Image>();
		image.sprite = tool.Icon;
		image.preserveAspect = true;
		UiHoverEventEmitter uiHoverEventEmitter = gameObject.AddComponent<UiHoverEventEmitter>();
		Button button = gameObject.AddComponent<Button>();
		button.targetGraphic = image;
		ColorBlock colors = button.colors;
		colors.fadeDuration = 0.1f;
		button.colors = colors;
		ToolControllerBehaviour toolController = Object.FindObjectOfType<ToolControllerBehaviour>();
		button.onClick.AddListener(delegate
		{
			toolController.SetToolByTypeName(tool.Type);
		});
		HasTooltipBehaviour hasTooltipBehaviour = gameObject.AddComponent<HasTooltipBehaviour>();
		hasTooltipBehaviour.Text = "<b>" + tool.Name + "</b>\n" + tool.Description;
		hasTooltipBehaviour.TooltipText = ToolTipTextMesh;
		LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
		layoutElement.preferredHeight = 55f;
		layoutElement.preferredWidth = 55f;
		Transform childContainer = null;
		foreach (ToolLibrary.Tool allEntry in allEntries)
		{
			if (!(allEntry == tool) && !(allEntry.Parent != tool.Name))
			{
				if (!childContainer)
				{
					childContainer = CreateChildContainerFor(gameObject).transform;
				}
				CreateButton(allEntry, childContainer.transform, allEntries).GetComponent<Button>().onClick.AddListener(delegate
				{
					childContainer.gameObject.SetActive(value: false);
				});
			}
		}
		if ((bool)childContainer)
		{
			uiHoverEventEmitter.OnMouseEnter.AddListener(delegate
			{
				childContainer.gameObject.SetActive(value: true);
			});
			uiHoverEventEmitter.OnMouseExit.AddListener(delegate
			{
				childContainer.gameObject.SetActive(value: false);
			});
		}
		return gameObject;
	}

	private GameObject CreateChildContainerFor(GameObject obj)
	{
		GameObject obj2 = new GameObject("child container");
		obj2.transform.SetParent(obj.transform);
		obj2.transform.localScale = Vector3.one;
		RectTransform rectTransform = obj2.AddComponent<RectTransform>();
		rectTransform.anchorMin = new Vector2(0f, 0.5f);
		rectTransform.anchorMax = new Vector2(0f, 0.5f);
		rectTransform.pivot = new Vector2(1f, 0.5f);
		rectTransform.anchoredPosition = new Vector2(0f, 0f);
		obj2.AddComponent<Image>().color = Color.black;
		ContentSizeFitter contentSizeFitter = obj2.AddComponent<ContentSizeFitter>();
		contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
		HorizontalLayoutGroup horizontalLayoutGroup = obj2.AddComponent<HorizontalLayoutGroup>();
		horizontalLayoutGroup.spacing = 15f;
		horizontalLayoutGroup.padding = new RectOffset(10, 10, 10, 10);
		horizontalLayoutGroup.childControlWidth = true;
		horizontalLayoutGroup.childControlHeight = true;
		horizontalLayoutGroup.childForceExpandWidth = true;
		horizontalLayoutGroup.childForceExpandHeight = true;
		horizontalLayoutGroup.childScaleWidth = true;
		horizontalLayoutGroup.childScaleHeight = true;
		obj2.SetActive(value: false);
		return obj2;
	}
}

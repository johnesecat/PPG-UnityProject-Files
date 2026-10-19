using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContextMenuInteractableConditionalBehaviour : MonoBehaviour
{
	private Button button;

	private HasTooltipBehaviour tooltip;

	private TextMeshProUGUI text;

	public ContextMenuBehaviour Component;

	public string Property = "enabled";

	private PropertyInfo foundProperty;

	private void Awake()
	{
		button = GetComponent<Button>();
		tooltip = GetComponent<HasTooltipBehaviour>();
		text = GetComponentInChildren<TextMeshProUGUI>();
		foundProperty = Component.GetType().GetProperty(Property);
		if (foundProperty == null)
		{
			Debug.LogWarning("Property " + Property + " couldn't be found");
		}
	}

	private void Update()
	{
		try
		{
			bool flag = (bool)foundProperty.GetValue(Component, null);
			button.interactable = flag;
			text.color = (flag ? Color.white : new Color(1f, 1f, 1f, 0.2f));
			tooltip.enabled = flag;
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message);
		}
	}
}

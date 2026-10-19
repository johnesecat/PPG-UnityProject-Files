using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class ControlSchemeEditorBehaviour : MonoBehaviour
{
	public GameObject ControlPrefab;

	public GameObject HeaderPrefab;

	public Transform Container;

	internal static bool ShouldReinitialise;

	private void Awake()
	{
		LoadInputSystem();
		InitialiseControlScheme();
	}

	private void Update()
	{
		if (ShouldReinitialise)
		{
			ShouldReinitialise = false;
			LoadInputSystem();
			InitialiseControlScheme();
		}
	}

	public void InitialiseControlScheme()
	{
		Debug.Log("Initialising control scheme menu");
		foreach (Transform item in Container)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		foreach (ActionCategory category in Enum.GetValues(typeof(ActionCategory)))
		{
			if (category == ActionCategory.MapEditor)
			{
				continue;
			}
			IEnumerable<KeyValuePair<string, InputAction>> enumerable = InputSystem.Actions.Where((KeyValuePair<string, InputAction> action) => action.Value.Category == category && !action.Value.InvisibleInMenu);
			if (!enumerable.Any())
			{
				continue;
			}
			UnityEngine.Object.Instantiate(HeaderPrefab, Container).GetComponentInChildren<TextMeshProUGUI>().text = Regex.Replace(category.ToString(), "(\\B[A-Z])", " $1");
			foreach (KeyValuePair<string, InputAction> item2 in enumerable)
			{
				string key = item2.Key;
				InputAction action2 = item2.Value;
				CreateControlObject(key, in action2);
			}
		}
	}

	private void CreateControlObject(string key, in InputAction action)
	{
		ActionControlBehaviour component = UnityEngine.Object.Instantiate(ControlPrefab, Container).GetComponent<ActionControlBehaviour>();
		component.Action.text = action.Name;
		TriggerEditorBehaviour component2 = component.Trigger.GetComponent<TriggerEditorBehaviour>();
		component2.InputActionKey = key;
		component2.InputAction = action;
	}

	[ContextMenu("Save settings")]
	internal void SaveInputSystem()
	{
		InputSystem.Save();
	}

	[ContextMenu("Load settings")]
	internal void LoadInputSystem()
	{
		InputSystem.Load();
	}
}

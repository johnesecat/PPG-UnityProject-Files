using TMPro;
using UnityEngine;

public class DialogBoxManager : MonoBehaviour
{
	public GameObject DialogBoxPrefab;

	public static DialogBoxManager Main;

	private void Awake()
	{
		Main = this;
	}

	public static DialogBox Dialog(string title, params DialogButton[] buttons)
	{
		DialogBox component = Object.Instantiate(Main.DialogBoxPrefab, Main.transform).GetComponent<DialogBox>();
		component.Title = title;
		component.Buttons = buttons;
		return component;
	}

	public static DialogBox Notification(string message)
	{
		DialogBox component = Object.Instantiate(Main.DialogBoxPrefab, Main.transform).GetComponent<DialogBox>();
		component.Title = message;
		component.Buttons = new DialogButton[1]
		{
			new DialogButton("OK", true)
		};
		return component;
	}

	public static DialogBox TextEntry(string title, string placeholder, params DialogButton[] buttons)
	{
		DialogBox component = Object.Instantiate(Main.DialogBoxPrefab, Main.transform).GetComponent<DialogBox>();
		component.Title = title;
		component.ShowTextBox = true;
		component.Buttons = buttons;
		(component.InputField.placeholder as TextMeshProUGUI).text = placeholder;
		return component;
	}

	public static DialogBox KeyEntry(string title, KeyCode initial, params DialogButton[] buttons)
	{
		DialogBox component = Object.Instantiate(Main.DialogBoxPrefab, Main.transform).GetComponent<DialogBox>();
		component.Title = title;
		component.ShowKeyBox = true;
		component.Buttons = buttons;
		KeyTriggerEditor keyField = component.KeyField;
		keyField.SetKey = initial;
		keyField.EditOnAwake = true;
		return component;
	}
}

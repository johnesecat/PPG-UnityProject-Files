using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour
{
	public TextMeshProUGUI TextMesh;

	public TMP_InputField InputField;

	public KeyTriggerEditor KeyField;

	public Transform DialogButtonHolder;

	public GameObject DialogButtonPrefab;

	[Space]
	public string Title;

	public DialogButton[] Buttons;

	public bool ShowTextBox;

	public bool ShowKeyBox;

	public static int OpenDialogboxCount { get; internal set; }

	public static bool IsAnyDialogboxOpen => OpenDialogboxCount > 0;

	public string EnteredText
	{
		get
		{
			if (ShowTextBox)
			{
				return InputField.text;
			}
			return "";
		}
		set
		{
			if (ShowTextBox)
			{
				InputField.text = value;
				return;
			}
			throw new Exception("Attempt to set Entered Text in an invalid dialog box");
		}
	}

	public KeyCode InputKey
	{
		get
		{
			if (ShowKeyBox)
			{
				return KeyField.SetKey;
			}
			return KeyCode.None;
		}
	}

	private void Start()
	{
		InputField.gameObject.SetActive(ShowTextBox);
		KeyField.gameObject.SetActive(ShowKeyBox);
		TextMesh.text = Title;
		DialogButton[] buttons = Buttons;
		for (int i = 0; i < buttons.Length; i++)
		{
			DialogButton dialogButton = buttons[i];
			GameObject obj = UnityEngine.Object.Instantiate(DialogButtonPrefab, DialogButtonHolder);
			obj.GetComponentInChildren<TextMeshProUGUI>().text = dialogButton.Label;
			Button component = obj.GetComponent<Button>();
			UnityAction[] actions = dialogButton.Actions;
			foreach (UnityAction call in actions)
			{
				component.onClick.AddListener(call);
			}
			if (dialogButton.ClosesDialogBox)
			{
				component.onClick.AddListener(Close);
			}
		}
		UISoundBehaviour.Refresh();
		OpenDialogboxCount++;
	}

	public void Close()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void SetTitle(string title)
	{
		Title = title;
		TextMesh.text = Title;
	}

	public void SetHeight(float h)
	{
		RectTransform component = GetComponent<RectTransform>();
		component.sizeDelta = new Vector2(component.sizeDelta.x, h);
	}

	public void SetWidth(float w)
	{
		RectTransform component = GetComponent<RectTransform>();
		component.sizeDelta = new Vector2(w, component.sizeDelta.y);
	}

	private void OnDestroy()
	{
		OpenDialogboxCount--;
	}
}

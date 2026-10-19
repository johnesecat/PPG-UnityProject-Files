using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TriggerEditorBehaviour : MonoBehaviour
{
	public string InputActionKey;

	public InputAction InputAction;

	public Button UnbindButton;

	public TextMeshProUGUI Text;

	private Button button;

	public Image Image;

	public static bool IsBeingEdited { get; private set; }

	private void Awake()
	{
		button = GetComponent<Button>();
	}

	private void Start()
	{
		SetText();
	}

	public void StartWaiting()
	{
		if (!IsBeingEdited)
		{
			IsBeingEdited = true;
			button.interactable = false;
			StartCoroutine(WaitForUserInput());
		}
	}

	private IEnumerator WaitForUserInput()
	{
		KeyCode oldKey = InputAction.Key;
		Vector3 oldPos = Input.mousePosition;
		Text.text = "Press any button...";
		Cursor.visible = false;
		IsBeingEdited = true;
		yield return new WaitUntil(() => !Input.anyKey);
		yield return new WaitUntil(() => HasReceivedUserInput() || (oldPos - Input.mousePosition).magnitude > 200f);
		if ((oldPos - Input.mousePosition).magnitude <= 200f)
		{
			SetText();
			yield return new WaitUntil(() => !Input.anyKey);
		}
		else
		{
			InputAction.Key = oldKey;
			SetText();
		}
		SaveStateChanges();
		IsBeingEdited = false;
		button.interactable = true;
		Cursor.visible = true;
		InputSystem.Save();
	}

	private void Update()
	{
		if (base.gameObject.activeInHierarchy)
		{
			Image.color = (InputSystem.Actions.Any((KeyValuePair<string, InputAction> c) => InputActionKey != c.Key && AreConflicting(c.Value)) ? (Color.red * 0.25f) : (Color.black * 0.25f));
		}
	}

	public void Unbind()
	{
		InputAction.Key = KeyCode.None;
		InputAction.SecondaryKey = KeyCode.None;
		SaveStateChanges();
		InputSystem.Save();
		SetText();
	}

	private void SaveStateChanges()
	{
		InputSystem.Actions[InputActionKey] = InputAction;
	}

	private bool AreConflicting(InputAction b)
	{
		if (!InputAction.Universe.UniverseMatches(b.Universe))
		{
			return false;
		}
		bool num = InputAction.Key == b.Key && InputAction.Key != KeyCode.None;
		bool flag = InputAction.SecondaryKey == b.SecondaryKey && InputAction.SecondaryKey != KeyCode.None;
		bool flag2 = (InputAction.SecondaryKey == b.Key && b.Key != 0) || (b.SecondaryKey == InputAction.Key && InputAction.Key != KeyCode.None);
		return num || flag || flag2;
	}

	private void SetText()
	{
		Text.text = ((InputAction.SecondaryKey == KeyCode.None) ? "" : (InputAction.SecondaryKey.ToString() + " + ")) + InputAction.Key;
		UnbindButton.gameObject.SetActive(InputAction.Key != KeyCode.None);
	}

	private bool HasReceivedUserInput()
	{
		InputAction.SecondaryKey = KeyCode.None;
		IList values = Enum.GetValues(typeof(KeyCode));
		for (int i = 0; i < values.Count; i++)
		{
			KeyCode keyCode = (KeyCode)values[i];
			if (Input.GetKey(keyCode))
			{
				InputAction.SecondaryKey = keyCode;
			}
		}
		IList values2 = Enum.GetValues(typeof(KeyCode));
		for (int j = 0; j < values2.Count; j++)
		{
			KeyCode key = (KeyCode)values2[j];
			if (Input.GetKeyUp(key))
			{
				InputAction.Key = key;
				return true;
			}
		}
		return false;
	}
}

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyTriggerEditor : MonoBehaviour
{
	public KeyCode SetKey;

	public TextMeshProUGUI Text;

	private Button button;

	private bool isWaiting;

	public bool EditOnAwake;

	private void Awake()
	{
		button = GetComponent<Button>();
	}

	public void StartWaiting()
	{
		if (!isWaiting)
		{
			isWaiting = true;
			button.interactable = false;
			StartCoroutine(WaitForUserInput());
		}
	}

	private void Start()
	{
		SetText();
		if (EditOnAwake)
		{
			StartWaiting();
		}
	}

	private void SetText()
	{
		Text.text = SetKey.ToString();
	}

	private bool HasReceivedUserInput()
	{
		foreach (KeyCode value in Enum.GetValues(typeof(KeyCode)))
		{
			if (Input.GetKeyUp(value))
			{
				SetKey = value;
				return true;
			}
		}
		return false;
	}

	private IEnumerator WaitForUserInput()
	{
		KeyCode oldKey = SetKey;
		Vector3 oldPos = Input.mousePosition;
		Text.text = "Press any button...";
		Cursor.visible = false;
		yield return new WaitUntil(() => !Input.anyKey);
		yield return new WaitUntil(() => HasReceivedUserInput() || (oldPos - Input.mousePosition).magnitude > 200f);
		if ((oldPos - Input.mousePosition).magnitude <= 200f)
		{
			SetText();
			yield return new WaitUntil(() => !Input.anyKey);
		}
		else
		{
			SetKey = oldKey;
			SetText();
		}
		isWaiting = false;
		button.interactable = true;
		Cursor.visible = true;
		InputSystem.Save();
	}
}

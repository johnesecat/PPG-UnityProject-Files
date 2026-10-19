using System;
using UnityEngine;

[Serializable]
public struct InputAction
{
	public string Name;

	public KeyCode Key;

	public KeyCode SecondaryKey;

	public bool InvisibleInMenu;

	public ActionCategory Category;

	public ActionUniverse Universe;

	private bool IsInCurrentUniverse => InputSystem.IsInUniverse(Universe);

	public InputAction(string name, ActionCategory category, KeyCode key, KeyCode secondary = KeyCode.None)
	{
		Name = name;
		Key = key;
		SecondaryKey = secondary;
		Category = category;
		InvisibleInMenu = false;
		Universe = ActionUniverse.Game;
	}

	public bool IsHeld()
	{
		if (!IsInCurrentUniverse)
		{
			return false;
		}
		bool key = Input.GetKey(Key);
		bool flag = SecondaryKey == KeyCode.None || Input.GetKey(SecondaryKey);
		return key && flag;
	}

	public bool IsReleased()
	{
		if (!IsInCurrentUniverse)
		{
			return false;
		}
		bool keyUp = Input.GetKeyUp(Key);
		bool flag = SecondaryKey == KeyCode.None || Input.GetKey(SecondaryKey);
		return keyUp && flag;
	}

	public bool IsPressed()
	{
		if (!IsInCurrentUniverse)
		{
			return false;
		}
		bool keyDown = Input.GetKeyDown(Key);
		bool flag = SecondaryKey == KeyCode.None || Input.GetKey(SecondaryKey);
		return keyDown && flag;
	}

	public string GetDisplayText()
	{
		if (SecondaryKey != 0)
		{
			return KeyToString(Key) + "+" + KeyToString(SecondaryKey);
		}
		return KeyToString(Key);
	}

	private static string KeyToString(KeyCode key)
	{
		switch (key)
		{
		case KeyCode.Mouse0:
			return "Left Mouse";
		case KeyCode.Mouse1:
			return "Right Mouse";
		case KeyCode.Mouse2:
			return "Middle Mouse";
		default:
			return key.ToString();
		}
	}

	[Obsolete]
	public void SimulateDown()
	{
	}

	[Obsolete]
	public void SimulateUp()
	{
	}
}

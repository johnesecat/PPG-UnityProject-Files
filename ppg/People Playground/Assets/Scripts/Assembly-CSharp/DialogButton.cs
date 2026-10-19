using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct DialogButton
{
	public string Label;

	public bool ClosesDialogBox;

	[Space]
	public UnityAction[] Actions;

	public DialogButton(string label, bool closesDialogBox, params UnityAction[] action)
	{
		Label = label;
		ClosesDialogBox = closesDialogBox;
		Actions = action;
	}
}

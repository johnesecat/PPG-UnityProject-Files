using System;
using System.Collections.Generic;
using UnityEngine.Events;

[Serializable]
public struct ContextMenuButton
{
	public Func<string> LabelGetter;

	public Func<bool> Condition;

	public List<UnityAction> Actions;

	public string Description;

	public string LabelWhenMultipleAreSelected;

	public readonly string Identity;

	public ContextMenuButton(string identity, string label, string desc, params UnityAction[] actions)
	{
		Identity = identity;
		LabelGetter = () => label;
		LabelWhenMultipleAreSelected = desc;
		Actions = new List<UnityAction>(actions);
		Description = desc;
		Condition = () => true;
	}

	public ContextMenuButton(string identity, Func<string> label, string desc, params UnityAction[] actions)
	{
		Identity = identity;
		LabelGetter = label;
		LabelWhenMultipleAreSelected = desc;
		Actions = new List<UnityAction>(actions);
		Description = desc;
		Condition = () => true;
	}

	public ContextMenuButton(Func<bool> condition, string identity, Func<string> label, string desc, params UnityAction[] actions)
	{
		Identity = identity;
		LabelGetter = label;
		LabelWhenMultipleAreSelected = desc;
		Actions = new List<UnityAction>(actions);
		Description = desc;
		Condition = condition;
	}

	public ContextMenuButton(Func<bool> condition, string identity, string label, string desc, params UnityAction[] actions)
	{
		Identity = identity;
		LabelGetter = () => label;
		LabelWhenMultipleAreSelected = desc;
		Actions = new List<UnityAction>(actions);
		Description = desc;
		Condition = condition;
	}
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class SelectionController
{
	private readonly List<PhysicalBehaviour> rawSelection = new List<PhysicalBehaviour>();

	private PhysicalBehaviour hover;

	public static SelectionController Main { get; private set; }

	public PhysicalBehaviour CurrentlyUnderMouse => hover;

	public ReadOnlyCollection<PhysicalBehaviour> SelectedObjects => rawSelection.AsReadOnly();

	public event EventHandler SelectionChanged;

	public SelectionController()
	{
		Main = this;
	}

	public void UserDeleteSelection()
	{
		if (SelectedObjects.Count == 0)
		{
			return;
		}
		HashSet<string> hashSet = new HashSet<string>();
		foreach (PhysicalBehaviour selectedObject in SelectedObjects)
		{
			if ((bool)selectedObject && selectedObject.Deletable)
			{
				selectedObject.transform.root.gameObject.SendMessage("OnUserDelete", SendMessageOptions.DontRequireReceiver);
				UnityEngine.Object.Destroy(selectedObject.transform.root.gameObject);
				if (!hashSet.Contains(selectedObject.transform.root.gameObject.name))
				{
					hashSet.Add(selectedObject.transform.root.gameObject.name);
				}
			}
		}
		foreach (string item in hashSet)
		{
			NotificationControllerBehaviour.Show("<b>" + item.ToUpper() + "</b> removed");
		}
	}

	public void ClearSelection()
	{
		for (int i = 0; i < rawSelection.Count; i++)
		{
			PhysicalBehaviour physicalBehaviour = rawSelection[i];
			if ((bool)physicalBehaviour)
			{
				ModAPI.InvokeItemDeselected(this, physicalBehaviour);
			}
		}
		rawSelection.Clear();
		RefreshOutlines();
		this.SelectionChanged?.Invoke(this, EventArgs.Empty);
	}

	public void Select(PhysicalBehaviour physicalBehaviour, bool multiple = false)
	{
		if (!physicalBehaviour.Selectable)
		{
			return;
		}
		if (!multiple)
		{
			for (int i = 0; i < rawSelection.Count; i++)
			{
				PhysicalBehaviour physicalBehaviour2 = rawSelection[i];
				if ((bool)physicalBehaviour2)
				{
					ModAPI.InvokeItemDeselected(this, physicalBehaviour2);
				}
			}
			rawSelection.Clear();
		}
		if (!rawSelection.Contains(physicalBehaviour))
		{
			rawSelection.Add(physicalBehaviour);
			ModAPI.InvokeItemSelected(this, physicalBehaviour);
		}
		this.SelectionChanged?.Invoke(this, EventArgs.Empty);
		RefreshOutlines();
	}

	public void Select(IEnumerable<PhysicalBehaviour> physicalBehaviours, bool multiple = false)
	{
		if (!multiple)
		{
			rawSelection.Clear();
		}
		foreach (PhysicalBehaviour physicalBehaviour in physicalBehaviours)
		{
			if (physicalBehaviour.Selectable && !rawSelection.Contains(physicalBehaviour))
			{
				rawSelection.Add(physicalBehaviour);
				if ((bool)physicalBehaviour)
				{
					ModAPI.InvokeItemSelected(this, physicalBehaviour);
				}
			}
		}
		RefreshOutlines();
		this.SelectionChanged?.Invoke(this, EventArgs.Empty);
	}

	public void Deselect(PhysicalBehaviour physicalBehaviour)
	{
		if ((bool)physicalBehaviour)
		{
			ModAPI.InvokeItemDeselected(this, physicalBehaviour);
		}
		rawSelection.Remove(physicalBehaviour);
		RefreshOutlines();
		this.SelectionChanged?.Invoke(this, EventArgs.Empty);
	}

	public void RefreshOutlines()
	{
		ToolBehaviour currentTool = Global.main.ToolControllerBehaviour.CurrentTool;
		bool flag = (bool)currentTool && currentTool.HideHeldOutline;
		PhysicalBehaviour physicalBehaviour = (currentTool ? currentTool.ActiveSingleSelected : null);
		for (int i = 0; i < Global.main.PhysicalObjectsInWorld.Count; i++)
		{
			PhysicalBehaviour physicalBehaviour2 = Global.main.PhysicalObjectsInWorld[i];
			bool flag2 = physicalBehaviour == physicalBehaviour2;
			bool flag3 = !currentTool.ShowOutlineOnlyOnHover && rawSelection.Contains(physicalBehaviour2);
			physicalBehaviour2.ShowOutline = (hover == physicalBehaviour2 || flag3) && (!flag || !flag2) && !Global.main.UILock;
		}
	}

	public void SetHovering(PhysicalBehaviour currentlyHovering)
	{
		hover = currentlyHovering;
		RefreshOutlines();
	}
}

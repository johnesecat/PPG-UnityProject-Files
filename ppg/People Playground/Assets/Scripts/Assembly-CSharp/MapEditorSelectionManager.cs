using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MapEditorSelectionManager : MonoBehaviour
{
	public static MapEditorSelectionManager Instance;

	[NonSerialized]
	public List<MapEditorSelectable> CurrentlySelected = new List<MapEditorSelectable>();

	[NonSerialized]
	public List<MapEditorSelectable> HoveringOver = new List<MapEditorSelectable>();

	private int lastIndexSelected;

	private int hoveringHash;

	private IEnumerable<MapEditorSelectable> sortedSelectables = new List<MapEditorSelectable>();

	public UnityEvent<MapEditorSelectable> OnSelect = new UnityEvent<MapEditorSelectable>();

	public UnityEvent<MapEditorSelectable> OnDeselect = new UnityEvent<MapEditorSelectable>();

	public MapEditorSelectable TopMostHovering => HoveringOver.FirstOrDefault();

	private void Awake()
	{
		Instance = this;
	}

	public void UpdateSortedSelectables()
	{
		sortedSelectables = MapEditorSelectable.Selectables.OrderBy((MapEditorSelectable s) => s.Depth);
	}

	private void Update()
	{
		DetermineHovering(sortedSelectables);
		if (MapEditorGlobal.Instance.UiLock)
		{
			return;
		}
		if (InputSystem.Down("drag"))
		{
			foreach (MapEditorSelectable item in CurrentlySelected)
			{
				item.selectionAge++;
			}
		}
		if (InputSystem.Up("drag") && InputSystem.Held("selectMultiple") && !InputSystem.Down("context"))
		{
			foreach (MapEditorSelectable item2 in HoveringOver)
			{
				if (item2.selectionAge > 1 && CurrentlySelected.Contains(item2))
				{
					Deselect(item2);
				}
			}
		}
		if (InputSystem.Down("drag") || InputSystem.Down("context"))
		{
			if (!InputSystem.Held("selectMultiple") && (!InputSystem.Down("context") || CurrentlySelected.Count <= 1))
			{
				ClearSelection();
			}
			using (List<MapEditorSelectable>.Enumerator enumerator = HoveringOver.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					MapEditorSelectable current2 = enumerator.Current;
					if (!CurrentlySelected.Contains(current2))
					{
						Select(current2);
						current2.selectionAge++;
					}
				}
			}
		}
		if (InputSystem.Down("context"))
		{
			MapEditorGlobal.Instance.ContextMenu.Show(Input.mousePosition);
		}
		if (MapEditorGlobal.Instance.ContextMenu.IsShown && InputSystem.Down("drag"))
		{
			GameObject currentSelectedGameObject = MapEditorGlobal.Instance.EventSystem.currentSelectedGameObject;
			if (!currentSelectedGameObject || !currentSelectedGameObject.transform.IsChildOf(MapEditorGlobal.Instance.ContextMenu.transform))
			{
				MapEditorGlobal.Instance.ContextMenu.Hide();
			}
		}
	}

	public void Select(MapEditorSelectable selectable)
	{
		if (CurrentlySelected.Contains(selectable))
		{
			throw new Exception("Can't select a selected object again...");
		}
		CurrentlySelected.Add(selectable);
		selectable.BroadcastMessage("OnSelect", SendMessageOptions.DontRequireReceiver);
		OnSelect?.Invoke(selectable);
	}

	public void Deselect(MapEditorSelectable selectable)
	{
		if (CurrentlySelected.Remove(selectable))
		{
			selectable.BroadcastMessage("OnDeselect", SendMessageOptions.DontRequireReceiver);
		}
		OnDeselect?.Invoke(selectable);
	}

	public void ClearSelection()
	{
		foreach (MapEditorSelectable selectable in MapEditorSelectable.Selectables)
		{
			Deselect(selectable);
		}
	}

	private void DetermineHovering(IEnumerable<MapEditorSelectable> sorted)
	{
		HoveringOver.Clear();
		hoveringHash = 0;
		foreach (MapEditorSelectable item in sorted)
		{
			if (item.MouseOverlaps())
			{
				HoveringOver.Add(item);
				hoveringHash += item.GetHashCode();
			}
		}
	}
}

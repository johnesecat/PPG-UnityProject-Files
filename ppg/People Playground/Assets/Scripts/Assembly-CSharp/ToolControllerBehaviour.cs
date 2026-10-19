using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolControllerBehaviour : MonoBehaviour
{
	public class ToolChangeEventArgs : EventArgs
	{
		public int Index;

		public ToolChangeEventArgs(int index)
		{
			Index = index;
		}
	}

	public ToolBehaviour CurrentTool;

	public ContextMenuBehaviour contextMenu;

	public LayerMask SelectionLayer;

	private bool isCurrentlyHolding;

	private PhysicalBehaviour currentlyHovering;

	private Type[] toolImplementations;

	private int parentCycleIndex;

	private int childCycleIndex;

	private int childCount;

	private bool isSelecting;

	private Vector2 selectionDragStart;

	private readonly SelectionController selectionController = new SelectionController();

	private readonly Collider2D[] buffer = new Collider2D[16];

	private static string[] toolNamesForKeyInput;

	public event EventHandler<ToolChangeEventArgs> ToolChanged;

	private void Start()
	{
		SetImplementationArray();
		ToolLibrary.OnCollectionChange.AddListener(SetImplementationArray);
		SetTool(0);
		if (toolNamesForKeyInput == null)
		{
			toolNamesForKeyInput = new string[11];
			for (int i = 0; i < 11; i++)
			{
				toolNamesForKeyInput[i] = "tool" + i;
			}
		}
	}

	private void SetImplementationArray()
	{
		Type[] types = Assembly.GetAssembly(GetType()).GetTypes();
		Type toolType = typeof(ToolBehaviour);
		toolImplementations = types.Where((Type t) => toolType.IsAssignableFrom(t)).Concat(ToolLibrary.ModdedTypes).ToArray();
	}

	public void SetTool(int index, ToolTab tab = ToolTab.Tools)
	{
		if ((bool)CurrentTool)
		{
			CurrentTool.OnToolUnchosen();
			isCurrentlyHolding = false;
			CurrentTool.OnDeselect();
			selectionController.RefreshOutlines();
			CurrentTool.ActiveSingleSelected = null;
		}
		List<ToolLibrary.Tool> relevantCollection = GetRelevantCollection(tab);
		if (index < 0 || index >= relevantCollection.Count)
		{
			Debug.LogError("Attempt to set out of bounds tool: " + index);
			return;
		}
		ToolLibrary.Tool entry = relevantCollection[index];
		Type type = toolImplementations.FirstOrDefault((Type f) => f.Name == entry.Type);
		if (type == null)
		{
			Debug.LogError("Attempt to set invalid tool: " + entry.Type);
			return;
		}
		Component component = GetComponents(type).FirstOrDefault((Component f) => f.GetType().Name == entry.Type);
		if (!component)
		{
			CurrentTool = base.gameObject.AddComponent(type) as ToolBehaviour;
		}
		else
		{
			CurrentTool = component as ToolBehaviour;
		}
		if (CurrentTool == null)
		{
			Debug.LogError("Attempt to set invalid tool: " + entry.Type);
			return;
		}
		CurrentTool.Rigidbody = GetComponent<Rigidbody2D>();
		CurrentTool.OnToolChosen();
		this.ToolChanged?.Invoke(this, new ToolChangeEventArgs(index));
	}

	private static List<ToolLibrary.Tool> GetRelevantCollection(ToolTab tab)
	{
		if (tab != 0)
		{
			return ToolLibrary.Instance.Powers;
		}
		return ToolLibrary.Instance.Tools;
	}

	public void SetToolByTypeName(string typeName)
	{
		for (int i = 0; i < ToolLibrary.Instance.Tools.Count; i++)
		{
			if (typeName == ToolLibrary.Instance.Tools[i].Type)
			{
				SetTool(i);
				return;
			}
		}
		for (int j = 0; j < ToolLibrary.Instance.Powers.Count; j++)
		{
			if (typeName == ToolLibrary.Instance.Powers[j].Type)
			{
				SetTool(j, ToolTab.Powers);
				break;
			}
		}
	}

	private void Update()
	{
		currentlyHovering = null;
		if (!Global.main.GetPausedMenu() && !DialogBox.IsAnyDialogboxOpen && !Global.ActiveUiBlock)
		{
			HandleToolSelectionInput();
			HandleIndirectInteraction();
			DetermineHovering();
			HandleTools();
			HandleContextMenu();
			UpdateSelectionBox();
		}
	}

	private void FixedUpdate()
	{
		if (InputSystem.Held("drag"))
		{
			CurrentTool.OnFixedHold();
		}
	}

	private void HandleIndirectInteraction()
	{
		if (InputSystem.Down("activateDirect") && !CurrentTool.ActiveSingleSelected)
		{
			PhysicalBehaviour currentlyUnderMouse = SelectionController.Main.CurrentlyUnderMouse;
			if ((bool)currentlyUnderMouse)
			{
				ActivationPropagation activationPropagation = new ActivationPropagation(direct: true, 0, currentlyUnderMouse.gameObject);
				activationPropagation.Target = currentlyUnderMouse.gameObject;
				Utils.Activations.SendOnce(activationPropagation, currentlyUnderMouse);
				currentlyUnderMouse.BroadcastMessage("Use", activationPropagation, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void HandleToolSelectionInput()
	{
		List<ToolLibrary.Tool> relevantCollection = GetRelevantCollection(ToolWheelBehaviour.Instance.CurrentTab);
		int num = 1;
		for (int i = 0; i < relevantCollection.Count; i++)
		{
			ToolLibrary.Tool tool = relevantCollection[i];
			if (!string.IsNullOrEmpty(tool.Parent))
			{
				continue;
			}
			if (num >= 0 && num < toolNamesForKeyInput.Length)
			{
				string text = toolNamesForKeyInput[num];
				if (InputSystem.Has(text) && InputSystem.Down(text))
				{
					if (parentCycleIndex != num || childCount <= 0)
					{
						SetToolByTypeName(tool.Type);
						childCycleIndex = -1;
						parentCycleIndex = num;
						childCount = 0;
						for (int j = 0; j < relevantCollection.Count; j++)
						{
							if (relevantCollection[j].Parent == tool.Name)
							{
								childCount++;
							}
						}
						UISoundBehaviour.Main.Scroll();
						NotificationControllerBehaviour.Show(tool.Name + " selected");
						break;
					}
					childCycleIndex++;
					if (childCycleIndex >= childCount)
					{
						childCycleIndex = -1;
						SetToolByTypeName(tool.Type);
						UISoundBehaviour.Main.Scroll();
						NotificationControllerBehaviour.Show(tool.Name + " selected");
						break;
					}
					int num2 = childCount - 1;
					for (int k = 0; k < relevantCollection.Count; k++)
					{
						ToolLibrary.Tool tool2 = relevantCollection[k];
						if (tool2.Parent == tool.Name)
						{
							if (num2 == childCycleIndex)
							{
								SetToolByTypeName(tool2.Type);
								UISoundBehaviour.Main.Scroll();
								NotificationControllerBehaviour.Show(tool2.Name + " selected");
								return;
							}
							num2--;
						}
					}
				}
			}
			num++;
		}
	}

	private void HandleContextMenu()
	{
		if (InputSystem.Down("context"))
		{
			contextMenu.Show(Input.mousePosition);
		}
	}

	private void HandleTools()
	{
		if (InputSystem.Down("drag") && !Global.main.UILock)
		{
			if ((bool)currentlyHovering)
			{
				if (InputSystem.Held("fast") && selectionController.SelectedObjects.Contains(currentlyHovering))
				{
					selectionController.Deselect(currentlyHovering);
					if (currentlyHovering == CurrentTool.ActiveSingleSelected)
					{
						CurrentTool.OnDeselect();
						selectionController.RefreshOutlines();
						if (selectionController.SelectedObjects.Count > 0)
						{
							CurrentTool.ActiveSingleSelected = selectionController.SelectedObjects[0];
						}
					}
				}
				else
				{
					selectionController.Select(currentlyHovering, InputSystem.Held("fast") || selectionController.SelectedObjects.Contains(currentlyHovering));
					if ((bool)CurrentTool.ActiveSingleSelected)
					{
						CurrentTool.OnDeselect();
					}
					selectionController.RefreshOutlines();
					CurrentTool.ActiveSingleSelected = currentlyHovering;
				}
			}
			else
			{
				EscapeAndDeselect();
				if (!CurrentTool.UsesEmptyDrag)
				{
					StartSelectionDrag();
				}
			}
			isCurrentlyHolding = true;
			CurrentTool.OnSelect();
			selectionController.RefreshOutlines();
		}
		if (InputSystem.Down("context") && (bool)currentlyHovering)
		{
			selectionController.Select(currentlyHovering, InputSystem.Held("fast") || selectionController.SelectedObjects.Contains(currentlyHovering));
		}
		if (InputSystem.Held("drag") && isCurrentlyHolding)
		{
			CurrentTool.OnHold();
		}
		if (InputSystem.Up("drag") && (bool)Global.main && (bool)Global.main.eventSystem)
		{
			GameObject currentSelectedGameObject = Global.main.eventSystem.currentSelectedGameObject;
			if (!currentSelectedGameObject || !currentSelectedGameObject.transform.IsChildOf(contextMenu.transform))
			{
				EscapeAndDeselect();
			}
			StopSelectionDrag();
		}
	}

	private void DetermineHovering()
	{
		if ((bool)CurrentTool && (bool)CurrentTool.ActiveSingleSelected && currentlyHovering == CurrentTool.ActiveSingleSelected)
		{
			currentlyHovering = null;
		}
		else
		{
			PhysicalBehaviour physicalBehaviour = null;
			float num = float.MaxValue;
			for (int i = 0; i < Physics2D.OverlapPointNonAlloc(Global.main.MousePosition, buffer, SelectionLayer, -0.9f, 0.9f); i++)
			{
				if (buffer[i].TryGetComponent<PhysicalBehaviour>(out var component))
				{
					float objectAreaByMass = component.ObjectAreaByMass;
					if (objectAreaByMass < num)
					{
						num = objectAreaByMass;
						physicalBehaviour = component;
					}
				}
			}
			currentlyHovering = physicalBehaviour;
		}
		if (selectionController.CurrentlyUnderMouse != currentlyHovering)
		{
			if ((bool)currentlyHovering && currentlyHovering.Selectable)
			{
				selectionController.SetHovering(currentlyHovering);
			}
			else if (selectionController.CurrentlyUnderMouse != null)
			{
				selectionController.SetHovering(null);
			}
		}
	}

	private void EscapeAndDeselect()
	{
		CurrentTool.OnDeselect();
		CurrentTool.ActiveSingleSelected = null;
		isCurrentlyHolding = false;
		contextMenu.Hide();
		if (!selectionController.SelectedObjects.Contains(currentlyHovering) && !InputSystem.Held("fast"))
		{
			if ((bool)EventSystem.current && EventSystem.current.IsPointerOverGameObject() && (bool)EventSystem.current.currentSelectedGameObject && (bool)EventSystem.current.currentSelectedGameObject.GetComponentInParent<NoDeselectUi>())
			{
				return;
			}
			selectionController.ClearSelection();
		}
		selectionController.RefreshOutlines();
	}

	private void UpdateSelectionBox()
	{
		if (!isSelecting)
		{
			return;
		}
		Vector2 vector = (Vector2)Input.mousePosition - selectionDragStart;
		Vector3 position = SelectionBoxBehaviour.Main.transform.position;
		Vector2 zero = Vector2.zero;
		if (vector.sqrMagnitude > 200f)
		{
			SelectionBoxBehaviour.Main.Show();
			if (vector.x < 0f)
			{
				position.x = Input.mousePosition.x;
				zero.x = Mathf.Abs(position.x - selectionDragStart.x);
			}
			else
			{
				position.x = selectionDragStart.x;
				zero.x = Mathf.Abs(vector.x);
			}
			if (vector.y > 0f)
			{
				position.y = Input.mousePosition.y;
				zero.y = Mathf.Abs(position.y - selectionDragStart.y);
			}
			else
			{
				position.y = selectionDragStart.y;
				zero.y = Mathf.Abs(vector.y);
			}
			SelectionBoxBehaviour.Main.transform.position = position;
			float num = Mathf.Lerp((float)Screen.width / Global.main.CanvasScaler.referenceResolution.x, (float)Screen.height / Global.main.CanvasScaler.referenceResolution.y, Global.main.CanvasScaler.matchWidthOrHeight);
			zero.x /= num;
			zero.y /= num;
			SelectionBoxBehaviour.Main.SetSize(zero.x, zero.y);
			Vector3 vector2 = Global.main.camera.ScreenToWorldPoint(selectionDragStart);
			SelectionBoxBehaviour.Main.SetSizeDisplay((Global.main.MousePosition.x - vector2.x) * (220f / 267f), (Global.main.MousePosition.y - vector2.y) * (220f / 267f));
		}
		else
		{
			SelectionBoxBehaviour.Main.Hide();
		}
	}

	private void StartSelectionDrag()
	{
		if (!isSelecting)
		{
			isSelecting = true;
			selectionDragStart = Input.mousePosition;
			SelectionBoxBehaviour.Main.transform.position = selectionDragStart;
		}
	}

	private void StopSelectionDrag()
	{
		if (!isSelecting)
		{
			return;
		}
		isSelecting = false;
		SelectionBoxBehaviour.Main.Hide();
		Vector3 vector = Global.main.camera.ScreenToWorldPoint(selectionDragStart);
		if (!((selectionDragStart - (Vector2)Input.mousePosition).sqrMagnitude < 200f))
		{
			Debug.DrawLine(vector, Global.main.MousePosition, Color.green, 5f);
			if (!InputSystem.Held("fast"))
			{
				selectionController.ClearSelection();
			}
			Collider2D[] source = Physics2D.OverlapAreaAll(vector, Global.main.MousePosition, SelectionLayer);
			selectionController.Select(from c in source
				where Global.main.PhysicalObjectsInWorldByTransform.ContainsKey(c.transform)
				select c.GetComponent<PhysicalBehaviour>(), InputSystem.Held("fast"));
		}
	}
}

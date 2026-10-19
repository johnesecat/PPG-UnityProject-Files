using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ContextMenuBehaviour : MonoBehaviour
{
	internal static ContextMenuBehaviour Instance;

	private RectTransform rectTransform;

	public SpawnableOutlineBehaviour SpawnableOutline;

	private CanvasScaler canvasScaler;

	private bool saveNextFrame;

	private GameObject[] previousSelectedObjects;

	public GameObject ButtonPrefab;

	public TextMeshProUGUI ToolTipTextMesh;

	public GameObject Separator;

	public Transform ButtonParent;

	private Hover priorSelectedToolBehaviour;

	private readonly List<GameObject> dynamicButtons = new List<GameObject>();

	public bool isOpen { get; set; }

	public bool HasObject
	{
		get
		{
			if (SelectionController.Main.SelectedObjects.Any((PhysicalBehaviour c) => c))
			{
				return priorSelectedToolBehaviour == null;
			}
			return false;
		}
	}

	public bool HasSingleObject
	{
		get
		{
			if (SelectionController.Main.SelectedObjects.Count == 1)
			{
				return priorSelectedToolBehaviour == null;
			}
			return false;
		}
	}

	public bool CanDelete
	{
		get
		{
			if (!HasObject)
			{
				return priorSelectedToolBehaviour != null;
			}
			return true;
		}
	}

	public bool HasCopied => ClipboardControllerBehaviour.Main.ObjectStateInClipboard != null;

	public bool isFollowingObject
	{
		get
		{
			if (!isOpen || !HasObject)
			{
				return false;
			}
			if (!Global.main.CameraControlBehaviour.CurrentlyFollowing.Any())
			{
				return false;
			}
			return Global.main.CameraControlBehaviour.CurrentlyFollowing.Intersect(SelectionController.Main.SelectedObjects).Count() == SelectionController.Main.SelectedObjects.Count;
		}
	}

	public bool isFrozen
	{
		get
		{
			if (!isOpen || !HasObject)
			{
				return false;
			}
			foreach (PhysicalBehaviour selectedObject in SelectionController.Main.SelectedObjects)
			{
				if ((bool)selectedObject.GetComponent<FreezeBehaviour>())
				{
					return true;
				}
			}
			return false;
		}
	}

	public bool isNoCollide
	{
		get
		{
			if (!isOpen || !HasObject)
			{
				return false;
			}
			foreach (PhysicalBehaviour selectedObject in SelectionController.Main.SelectedObjects)
			{
				if (selectedObject.gameObject.layer != 10)
				{
					return false;
				}
			}
			return true;
		}
	}

	public bool isWeightless
	{
		get
		{
			if (!isOpen || !HasObject)
			{
				return false;
			}
			foreach (PhysicalBehaviour selectedObject in SelectionController.Main.SelectedObjects)
			{
				if (!selectedObject.IsWeightless)
				{
					return false;
				}
			}
			return true;
		}
	}

	public bool isOnFire
	{
		get
		{
			if (!isOpen || !HasObject)
			{
				return false;
			}
			foreach (PhysicalBehaviour selectedObject in SelectionController.Main.SelectedObjects)
			{
				PhysicalBehaviour component = selectedObject.GetComponent<PhysicalBehaviour>();
				if ((bool)component && !component.OnFire)
				{
					return false;
				}
			}
			return true;
		}
	}

	public bool ShouldStopResizing
	{
		get
		{
			if (!isResizing)
			{
				if ((bool)Global.main.ResizeHandles && Global.main.ResizeHandles.HasTargets)
				{
					return !HasObject;
				}
				return false;
			}
			return true;
		}
	}

	public bool ShouldShowResizeButton
	{
		get
		{
			if (!Global.main.ResizeHandles || !Global.main.ResizeHandles.HasTargets)
			{
				return HasObject;
			}
			return true;
		}
	}

	public bool ShouldHideLayer
	{
		get
		{
			if ((bool)LayerEditorController.Instance)
			{
				return LayerEditorController.Instance.IsOpen;
			}
			return false;
		}
	}

	public bool ShouldShowLayerButton
	{
		get
		{
			if ((bool)LayerEditorController.Instance)
			{
				return HasObject;
			}
			return false;
		}
	}

	public bool isResizing
	{
		get
		{
			if (HasObject && (bool)Global.main.ResizeHandles && Global.main.ResizeHandles.HasTargets)
			{
				return ((IEnumerable<PhysicalBehaviour>)SelectionController.Main.SelectedObjects).All((Func<PhysicalBehaviour, bool>)((IEnumerable<PhysicalBehaviour>)Global.main.ResizeHandles.Targets).Contains);
			}
			return false;
		}
	}

	[Obsolete]
	public bool CanAmalgamate => false;

	public void Show(Vector2 screenPosition)
	{
		if (!Global.main.GetPausedMenu())
		{
			Separator.SetActive(value: false);
			ClearDynamicButtons();
			CreateDynamicButtons();
			base.transform.position = Vector3.zero;
			base.gameObject.SetActive(value: true);
			rectTransform.ForceUpdateRectTransforms();
			LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
			rectTransform.ForceUpdateRectTransforms();
			ClampToScreen(screenPosition);
			isOpen = true;
			if (UserPreferenceManager.Current.DeleteWireByContextMenu)
			{
				priorSelectedToolBehaviour = Hover.CurrentlyHovering;
			}
			else
			{
				priorSelectedToolBehaviour = null;
			}
		}
	}

	private void CreateDynamicButtons()
	{
		IEnumerable<ContextMenuButton> buttons = SelectionController.Main.SelectedObjects.SelectMany((PhysicalBehaviour o) => o.ContextMenuOptions.Buttons);
		List<ContextMenuButton> list = MergeButtons(buttons);
		if (list.Count == 0)
		{
			return;
		}
		foreach (ContextMenuButton item in list)
		{
			CreateButton(item);
		}
		Separator.SetActive(value: true);
		UISoundBehaviour.Refresh();
	}

	private List<ContextMenuButton> MergeButtons(IEnumerable<ContextMenuButton> buttons)
	{
		Dictionary<string, ContextMenuButton> dictionary = new Dictionary<string, ContextMenuButton>();
		foreach (ContextMenuButton b in buttons)
		{
			if (!b.Condition())
			{
				continue;
			}
			if (dictionary.TryGetValue(b.Identity, out var value))
			{
				value.Actions.AddRange(b.Actions);
				value.LabelGetter = () => b.LabelWhenMultipleAreSelected;
				dictionary[b.Identity] = value;
			}
			else
			{
				dictionary.Add(b.Identity, new ContextMenuButton(b.Identity, b.LabelGetter, b.Description, b.Actions.ToArray()));
			}
		}
		return dictionary.Values.ToList();
	}

	private void CreateButton(ContextMenuButton button)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(ButtonPrefab, ButtonParent);
		gameObject.GetComponentInChildren<TextUpdaterBehaviour>().StringFunction = button.LabelGetter;
		HasTooltipBehaviour componentInChildren = gameObject.GetComponentInChildren<HasTooltipBehaviour>();
		componentInChildren.TooltipText = ToolTipTextMesh;
		componentInChildren.Text = button.Description;
		foreach (UnityAction action in button.Actions)
		{
			gameObject.GetComponentInChildren<Button>().onClick.AddListener(action);
		}
		gameObject.GetComponentInChildren<Button>().onClick.AddListener(Hide);
		dynamicButtons.Add(gameObject);
	}

	private void ClearDynamicButtons()
	{
		foreach (GameObject dynamicButton in dynamicButtons)
		{
			UnityEngine.Object.Destroy(dynamicButton);
		}
		dynamicButtons.Clear();
	}

	private void ClampToScreen(Vector2 screenPosition)
	{
		base.transform.position = screenPosition;
		Vector2 vector = rectTransform.sizeDelta * 2f / (canvasScaler.referenceResolution.x / Mathf.Lerp(Screen.width, Screen.height, canvasScaler.matchWidthOrHeight));
		screenPosition.x = Mathf.Clamp(screenPosition.x, 0f, (float)Screen.width - vector.x);
		screenPosition.y = Mathf.Clamp(screenPosition.y, vector.y, Screen.height);
		base.transform.position = screenPosition;
	}

	public void Hide()
	{
		if (isOpen)
		{
			isOpen = false;
			base.gameObject.SetActive(value: false);
		}
	}

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		canvasScaler = UnityEngine.Object.FindObjectOfType<CanvasScaler>();
		Instance = this;
	}

	private void Start()
	{
		base.gameObject.SetActive(value: false);
	}

	private void Update()
	{
		base.transform.localScale = Vector3.one * 1.8f;
		if (isOpen)
		{
			ClampToScreen(base.transform.position);
		}
	}

	public void DeleteAction()
	{
		if (isOpen)
		{
			if ((bool)priorSelectedToolBehaviour)
			{
				priorSelectedToolBehaviour.OnUserDelete();
				UnityEngine.Object.Destroy(priorSelectedToolBehaviour);
			}
			else
			{
				SelectionController.Main.UserDeleteSelection();
				Hide();
			}
		}
	}

	public void CopyAction()
	{
		if (isOpen && HasObject)
		{
			Vector3 newOrigin = Camera.main.ScreenToWorldPoint(base.transform.position);
			ClipboardControllerBehaviour.Main.ObjectStateInClipboard = ObjectStateConverter.Convert(SelectionController.Main.SelectedObjects.Select((PhysicalBehaviour c) => c.gameObject).ToArray(), newOrigin);
			Hide();
		}
	}

	public void PasteAction()
	{
		if (isOpen && HasCopied)
		{
			Vector3 newOrigin = Camera.main.ScreenToWorldPoint(base.transform.position);
			UndoControllerBehaviour.RegisterAction(new PasteLoadAction(ObjectStateConverter.Convert(ClipboardControllerBehaviour.Main.ObjectStateInClipboard, newOrigin), "Paste"));
			Hide();
		}
	}

	public void FollowAction()
	{
		if (!isOpen || !HasObject)
		{
			return;
		}
		bool flag = isFollowingObject;
		foreach (PhysicalBehaviour selectedObject in SelectionController.Main.SelectedObjects)
		{
			Global.main.CameraControlBehaviour.CurrentlyFollowing.Remove(selectedObject);
		}
		if (!flag)
		{
			Global.main.CameraControlBehaviour.CurrentlyFollowing.AddRange(SelectionController.Main.SelectedObjects);
		}
		Hide();
	}

	public void SaveAction()
	{
		DialogBox dialog;
		if (isOpen && HasObject)
		{
			ContraptionSavePoint.Main.NameToSave = "";
			ContraptionSavePoint.Main.GameObjectsToSave = SelectionController.Main.SelectedObjects.Select((PhysicalBehaviour c) => c.gameObject).ToArray();
			ContraptionSavePoint.Main.NewOrigin = Camera.main.ScreenToWorldPoint(base.transform.position);
			dialog = null;
			dialog = DialogBoxManager.TextEntry("Enter contraption name", "", new DialogButton("Cancel", true), new DialogButton("Save", true, askForSave));
			Hide();
		}
		void askForSave()
		{
			if (string.IsNullOrWhiteSpace(dialog.EnteredText) || dialog.EnteredText.IndexOfAny(Path.GetInvalidFileNameChars()) != -1 || dialog.EnteredText.Contains("."))
			{
				UISoundBehaviour.Main.Error();
				DialogBoxManager.Notification("\"" + dialog.EnteredText + "\" contains invalid characters");
			}
			else
			{
				string lower = dialog.EnteredText.ToLower();
				if (ContraptionSerialiser.GetAllContraptions().Any((ContraptionMetaData c) => c.Name.ToLower() == lower))
				{
					UISoundBehaviour.Main.Warning();
					DialogBoxManager.Dialog("This will overwrite an existing contraption with the same name.", new DialogButton("Continue", true, save), new DialogButton("Cancel", true));
				}
				else
				{
					save();
				}
			}
		}
		void save()
		{
			ContraptionSavePoint.Main.NameToSave = dialog.EnteredText;
			ContraptionSavePoint.Main.DoSave();
		}
	}

	[Obsolete]
	public void LoadAction()
	{
	}

	public void ActivateAction()
	{
		if (!isOpen || !HasObject)
		{
			return;
		}
		foreach (PhysicalBehaviour selectedObject in SelectionController.Main.SelectedObjects)
		{
			if ((bool)selectedObject)
			{
				selectedObject.BroadcastMessage("Use", new ActivationPropagation(direct: true, 0, selectedObject.gameObject), SendMessageOptions.DontRequireReceiver);
			}
		}
		Hide();
	}

	public void FreezeAction()
	{
		if (!isOpen || !HasObject)
		{
			return;
		}
		bool flag = !isFrozen;
		foreach (PhysicalBehaviour selectedObject in SelectionController.Main.SelectedObjects)
		{
			if (!selectedObject)
			{
				continue;
			}
			if (!flag)
			{
				FreezeBehaviour component = selectedObject.gameObject.GetComponent<FreezeBehaviour>();
				if ((bool)component)
				{
					UnityEngine.Object.Destroy(component);
					selectedObject.gameObject.BroadcastMessage("OnUserUnfreeze", SendMessageOptions.DontRequireReceiver);
				}
			}
			else
			{
				if (selectedObject.PlaySliderSound && (bool)selectedObject.slidingAudioSource)
				{
					selectedObject.slidingAudioSource.Stop();
				}
				selectedObject.gameObject.AddComponent<FreezeBehaviour>();
				selectedObject.gameObject.BroadcastMessage("OnUserFreeze", SendMessageOptions.DontRequireReceiver);
			}
		}
		Hide();
	}

	public void NoCollideAction()
	{
		if (!isOpen || !HasObject)
		{
			return;
		}
		bool flag = isNoCollide;
		foreach (PhysicalBehaviour selectedObject in SelectionController.Main.SelectedObjects)
		{
			if ((bool)selectedObject)
			{
				selectedObject.gameObject.SetLayer(flag ? 9 : 10);
				LayerSerialisationBehaviour layerSerialisationBehaviour = selectedObject.GetComponent<LayerSerialisationBehaviour>();
				if (!layerSerialisationBehaviour)
				{
					layerSerialisationBehaviour = selectedObject.gameObject.AddComponent<LayerSerialisationBehaviour>();
				}
				layerSerialisationBehaviour.Layer = selectedObject.gameObject.layer;
			}
		}
		Hide();
	}

	public void WeightlessAction()
	{
		if (!isOpen || !HasObject)
		{
			return;
		}
		bool flag = isWeightless;
		foreach (PhysicalBehaviour selectedObject in SelectionController.Main.SelectedObjects)
		{
			if (flag)
			{
				selectedObject.MakeWeightful();
			}
			else
			{
				selectedObject.MakeWeightless();
			}
		}
		Hide();
	}

	public void IgniteAction()
	{
		if (!isOpen || !HasObject)
		{
			return;
		}
		bool flag = !isOnFire;
		foreach (PhysicalBehaviour selectedObject in SelectionController.Main.SelectedObjects)
		{
			if (!(selectedObject.Properties.Flammability <= float.Epsilon) && (bool)selectedObject)
			{
				if (flag)
				{
					selectedObject.Ignite(ignoreFlammability: true, Global.main.MousePosition);
					selectedObject.BurnIntensity = 1f;
				}
				else
				{
					selectedObject.Extinguish();
					selectedObject.BurnIntensity = 0f;
				}
			}
		}
		Hide();
	}

	public void ResizeAction()
	{
		if (!isOpen)
		{
			return;
		}
		ResizeHandles resizeHandles = Global.main.ResizeHandles;
		if (ShouldStopResizing)
		{
			resizeHandles.Targets = Array.Empty<PhysicalBehaviour>();
		}
		else
		{
			if (SelectionController.Main.SelectedObjects.Count((PhysicalBehaviour w) => w.Resizable) <= 1)
			{
				PhysicalBehaviour physicalBehaviour = SelectionController.Main.SelectedObjects[0];
				if (!physicalBehaviour.Resizable)
				{
					return;
				}
				resizeHandles.HandleDistanceMultiplier = new Vector2(physicalBehaviour.InitialBounds.extents.x, physicalBehaviour.InitialBounds.extents.y);
			}
			else
			{
				resizeHandles.HandleDistanceMultiplier = Vector2.one;
			}
			resizeHandles.Targets = SelectionController.Main.SelectedObjects.Where((PhysicalBehaviour t) => t.Resizable && !DragTool.IsAnyParentSelected(t)).ToArray();
			resizeHandles.gameObject.SetActive(value: true);
		}
		resizeHandles.ResetHandles();
		Hide();
	}

	public void EditLayerAction()
	{
		if (isOpen)
		{
			if (LayerEditorController.Instance.IsOpen)
			{
				LayerEditorController.Instance.Hide();
			}
			else
			{
				LayerEditorController.Instance.Targets.Clear();
				LayerEditorController.Instance.Targets.AddRange(SelectionController.Main.SelectedObjects);
				LayerEditorController.Instance.Show();
			}
			Hide();
		}
	}

	[Obsolete]
	public void AmalgamateAction()
	{
		PhysicalObjectGrouper.Group(SelectionController.Main.SelectedObjects);
	}
}

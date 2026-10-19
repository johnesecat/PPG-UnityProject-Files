using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class MapEditorManipulator : MonoBehaviour
{
	private const string MoveGizmoGroup = "Move Gizmo";

	private const string ResizeGizmoGroup = "Resize Gizmo";

	[BoxGroup("Move Gizmo")]
	public GameObject MoveGizmo;

	[BoxGroup("Move Gizmo")]
	public ScreenUIEventEmitter AllAxisMoveGizmo;

	[BoxGroup("Move Gizmo")]
	public ScreenUIEventEmitter VerticalMoveGizmo;

	[BoxGroup("Move Gizmo")]
	public ScreenUIEventEmitter HorizontalMoveGizmo;

	[BoxGroup("Resize Gizmo")]
	public GameObject ResizeGizmo;

	[BoxGroup("Resize Gizmo")]
	public ScreenUIEventEmitter TopResizeHandle;

	[BoxGroup("Resize Gizmo")]
	public ScreenUIEventEmitter RightResizeHandle;

	[BoxGroup("Resize Gizmo")]
	public ScreenUIEventEmitter LeftResizeHandle;

	[BoxGroup("Resize Gizmo")]
	public ScreenUIEventEmitter BottomResizeHandle;

	public Vector2 CenterOfSelection;

	private Vector2 previousMousePos;

	private RectTransform catalogRect;

	private const float resizeHandleSize = 0.01f;

	private void Awake()
	{
		MapEditorSelectionManager.Instance.OnSelect.AddListener(OnSelect);
		MapEditorSelectionManager.Instance.OnDeselect.AddListener(OnDeselect);
	}

	private void Start()
	{
		MoveGizmo.SetActive(value: false);
		catalogRect = MapEditorGlobal.Instance.Catalog.GetComponent<RectTransform>();
	}

	private void OnRenderObject()
	{
		CalculateSelectionTransform();
		if (InputSystem.Down("drag"))
		{
			previousMousePos = MapEditorGlobal.Instance.WorldMousePos;
		}
		HandleTranslation();
		HandleResizing();
		previousMousePos = MapEditorGlobal.Instance.WorldMousePos;
	}

	private void HandleTranslation()
	{
		if (!MoveGizmo.activeSelf)
		{
			return;
		}
		List<MapEditorSelectable> currentlySelected = MapEditorSelectionManager.Instance.CurrentlySelected;
		Vector3 vector = MapEditorGlobal.Instance.WorldMousePos - previousMousePos;
		Space space = ((InputSystem.Held("localSpaceTransform") && currentlySelected.Count == 1) ? Space.Self : Space.World);
		switch (space)
		{
		case Space.World:
			MoveGizmo.transform.rotation = Quaternion.identity;
			break;
		case Space.Self:
			MoveGizmo.transform.rotation = currentlySelected[0].transform.rotation;
			break;
		}
		if (AllAxisMoveGizmo.IsBeingDragged)
		{
			foreach (MapEditorSelectable item in currentlySelected)
			{
				if ((bool)item)
				{
					item.transform.Translate(vector, Space.World);
				}
			}
			return;
		}
		if (VerticalMoveGizmo.IsBeingDragged)
		{
			if (space == Space.Self)
			{
				vector = Vector3.ProjectOnPlane(vector, currentlySelected[0].transform.right);
			}
			foreach (MapEditorSelectable item2 in currentlySelected)
			{
				if ((bool)item2)
				{
					item2.transform.Translate(0f, vector.y, 0f, space);
				}
			}
		}
		if (!HorizontalMoveGizmo.IsBeingDragged)
		{
			return;
		}
		if (space == Space.Self)
		{
			vector = Vector3.ProjectOnPlane(vector, currentlySelected[0].transform.up);
		}
		foreach (MapEditorSelectable item3 in currentlySelected)
		{
			if ((bool)item3)
			{
				item3.transform.Translate(vector.x, 0f, 0f, space);
			}
		}
	}

	private void HandleResizing()
	{
		if (!ResizeGizmo.activeSelf)
		{
			return;
		}
		List<MapEditorSelectable> currentlySelected = MapEditorSelectionManager.Instance.CurrentlySelected;
		if (currentlySelected.Count == 1)
		{
			HandleResizeSingleObject(currentlySelected[0]);
		}
		else if (currentlySelected.Count > 1)
		{
			ResizeGizmo.transform.position = CenterOfSelection;
			float num = float.MaxValue;
			float num2 = float.MinValue;
			float num3 = float.MaxValue;
			float num4 = float.MinValue;
			foreach (MapEditorSelectable item in currentlySelected)
			{
				Vector3 vector = item.transform.TransformPoint(new Vector2(item.LocalBounds.min.x, item.LocalBounds.max.y));
				Vector3 vector2 = item.transform.TransformPoint(new Vector2(item.LocalBounds.max.x, item.LocalBounds.max.y));
				Vector3 vector3 = item.transform.TransformPoint(new Vector2(item.LocalBounds.min.x, item.LocalBounds.min.y));
				Vector3 vector4 = item.transform.TransformPoint(new Vector2(item.LocalBounds.max.x, item.LocalBounds.min.y));
				num = Mathf.Min(num, Mathf.Min(Mathf.Min(vector.x, vector2.x), Mathf.Min(vector3.x, vector4.x)));
				num3 = Mathf.Min(num3, Mathf.Min(Mathf.Min(vector.y, vector2.y), Mathf.Min(vector3.y, vector4.y)));
				num2 = Mathf.Max(num2, Mathf.Max(Mathf.Max(vector.x, vector2.x), Mathf.Max(vector3.x, vector4.x)));
				num4 = Mathf.Max(num4, Mathf.Max(Mathf.Max(vector.y, vector2.y), Mathf.Max(vector3.y, vector4.y)));
			}
			float num5 = Mathf.Abs(num4 - num3);
			float num6 = Mathf.Abs(num2 - num);
			if (float.IsNaN(num5) || float.IsInfinity(num5) || float.IsNaN(num6) || float.IsInfinity(num6))
			{
				return;
			}
			Vector3 vector5 = new Vector3(num, num4);
			Vector3 vector6 = new Vector3(num2, num4);
			Vector3 vector7 = new Vector3(num, num3);
			Vector3 vector8 = new Vector3(num2, num3);
			TopResizeHandle.transform.SetPositionAndRotation((vector6 + vector5) / 2f, Quaternion.identity);
			RightResizeHandle.transform.SetPositionAndRotation((vector6 + vector8) / 2f, Quaternion.identity);
			BottomResizeHandle.transform.SetPositionAndRotation((vector7 + vector8) / 2f, Quaternion.identity);
			LeftResizeHandle.transform.SetPositionAndRotation((vector7 + vector5) / 2f, Quaternion.identity);
			float num7 = MapEditorGlobal.Instance.Camera.orthographicSize * 0.01f;
			TopResizeHandle.transform.localScale = new Vector3(num6, num7);
			BottomResizeHandle.transform.localScale = new Vector3(num6, num7);
			RightResizeHandle.transform.localScale = new Vector3(num7, num5);
			LeftResizeHandle.transform.localScale = new Vector3(num7, num5);
			Vector3 vector9 = MapEditorGlobal.Instance.WorldMousePos - previousMousePos;
			float magnitude = vector9.magnitude;
			float num8 = (vector9.x + vector9.y) / 2f;
			if (!RightResizeHandle.IsBeingDragged && !TopResizeHandle.IsBeingDragged && !BottomResizeHandle.IsBeingDragged && !LeftResizeHandle.IsBeingDragged)
			{
				return;
			}
			foreach (MapEditorSelectable item2 in currentlySelected)
			{
				Vector3 localScale = item2.transform.localScale;
				localScale.x += magnitude / item2.LocalBounds.size.x * 2f;
				localScale.y += magnitude / item2.LocalBounds.size.y * 2f;
				localScale.x = Mathf.Clamp(localScale.x, 0.1f, 1000f);
				localScale.y = Mathf.Clamp(localScale.y, 0.1f, 1000f);
				item2.transform.localScale = localScale;
				Vector3 vector10 = default(Vector3);
				Vector2 vector11 = (Vector2)item2.transform.position - CenterOfSelection;
				vector10.x += num8 * vector11.x / 2f;
				vector10.y += num8 * vector11.y / 2f;
				item2.transform.position += vector10;
			}
		}
		else
		{
			ResizeGizmo.SetActive(value: false);
		}
	}

	private void HandleResizeSingleObject(MapEditorSelectable selected)
	{
		ResizeGizmo.transform.position = selected.transform.position;
		Vector3 vector = selected.transform.TransformPoint(new Vector2(selected.LocalBounds.min.x, selected.LocalBounds.max.y));
		Vector3 vector2 = selected.transform.TransformPoint(new Vector2(selected.LocalBounds.max.x, selected.LocalBounds.max.y));
		Vector3 vector3 = selected.transform.TransformPoint(new Vector2(selected.LocalBounds.min.x, selected.LocalBounds.min.y));
		Vector3 vector4 = selected.transform.TransformPoint(new Vector2(selected.LocalBounds.max.x, selected.LocalBounds.min.y));
		float magnitude = (vector - vector3).magnitude;
		float magnitude2 = (vector3 - vector4).magnitude;
		TopResizeHandle.transform.SetPositionAndRotation((vector2 + vector) / 2f, selected.transform.rotation);
		RightResizeHandle.transform.SetPositionAndRotation((vector2 + vector4) / 2f, selected.transform.rotation);
		BottomResizeHandle.transform.SetPositionAndRotation((vector3 + vector4) / 2f, selected.transform.rotation);
		LeftResizeHandle.transform.SetPositionAndRotation((vector + vector3) / 2f, selected.transform.rotation);
		float num = MapEditorGlobal.Instance.Camera.orthographicSize * 0.01f;
		TopResizeHandle.transform.localScale = new Vector3(magnitude2, num);
		BottomResizeHandle.transform.localScale = new Vector3(magnitude2, num);
		RightResizeHandle.transform.localScale = new Vector3(num, magnitude);
		LeftResizeHandle.transform.localScale = new Vector3(num, magnitude);
		Vector3 vector5 = MapEditorGlobal.Instance.WorldMousePos - previousMousePos;
		if (RightResizeHandle.IsBeingDragged)
		{
			Vector3 localScale = selected.transform.localScale;
			localScale.x += vector5.x / selected.LocalBounds.size.x;
			localScale.x = Mathf.Clamp(localScale.x, 0.1f, 1000f);
			selected.transform.localScale = localScale;
			selected.transform.position += Vector3.right * vector5.x / 2f;
		}
		else if (TopResizeHandle.IsBeingDragged)
		{
			Vector3 localScale2 = selected.transform.localScale;
			localScale2.y += vector5.y / selected.LocalBounds.size.y;
			localScale2.y = Mathf.Clamp(localScale2.y, 0.1f, 1000f);
			selected.transform.localScale = localScale2;
			selected.transform.position += Vector3.up * vector5.y / 2f;
		}
		else if (LeftResizeHandle.IsBeingDragged)
		{
			Vector3 localScale3 = selected.transform.localScale;
			localScale3.x -= vector5.x / selected.LocalBounds.size.x;
			localScale3.x = Mathf.Clamp(localScale3.x, 0.1f, 1000f);
			selected.transform.localScale = localScale3;
			selected.transform.position += Vector3.right * vector5.x / 2f;
		}
		else if (BottomResizeHandle.IsBeingDragged)
		{
			Vector3 localScale4 = selected.transform.localScale;
			localScale4.y -= vector5.y / selected.LocalBounds.size.y;
			localScale4.y = Mathf.Clamp(localScale4.y, 0.1f, 1000f);
			selected.transform.localScale = localScale4;
			selected.transform.position += Vector3.up * vector5.y / 2f;
		}
	}

	private void CalculateSelectionTransform()
	{
		float num = float.MaxValue;
		float num2 = float.MinValue;
		float num3 = float.MaxValue;
		float num4 = float.MinValue;
		foreach (MapEditorSelectable item in MapEditorSelectionManager.Instance.CurrentlySelected)
		{
			if ((bool)item)
			{
				Vector3 position = item.transform.position;
				num = Mathf.Min(position.x, num);
				num2 = Mathf.Max(position.x, num2);
				num3 = Mathf.Min(position.y, num3);
				num4 = Mathf.Max(position.y, num4);
			}
		}
		CenterOfSelection = new Vector2((num + num2) * 0.5f, (num3 + num4) * 0.5f);
		Camera camera = MapEditorGlobal.Instance.Camera;
		float num5 = 170.66667f * ((float)camera.pixelHeight / 1080f);
		Bounds worldSpaceBounds = catalogRect.GetWorldSpaceBounds();
		Vector3 vector = camera.ScreenToWorldPoint(new Vector3((catalogRect.gameObject.activeInHierarchy ? worldSpaceBounds.size.x : 0f) + num5, 0f + num5, 0f));
		Vector3 vector2 = camera.ScreenToWorldPoint(new Vector3((float)camera.pixelWidth - num5, (float)camera.pixelHeight - num5));
		float x = Mathf.Clamp(CenterOfSelection.x, vector.x, vector2.x);
		float y = Mathf.Clamp(CenterOfSelection.y, vector.y, vector2.y);
		if (MoveGizmo.activeSelf)
		{
			MoveGizmo.transform.position = new Vector3(x, y, 0f);
		}
	}

	private void OnSelect(MapEditorSelectable selectable)
	{
		if (!MoveGizmo.activeSelf)
		{
			MoveGizmo.SetActive(value: true);
		}
		if (!ResizeGizmo.activeSelf)
		{
			ResizeGizmo.SetActive(value: true);
		}
	}

	private void OnDeselect(MapEditorSelectable selectable)
	{
		if ((bool)MapEditorSelectionManager.Instance && MapEditorSelectionManager.Instance.CurrentlySelected.Count == 0)
		{
			if ((bool)MoveGizmo && MoveGizmo.activeSelf)
			{
				MoveGizmo.SetActive(value: false);
			}
			if ((bool)ResizeGizmo && ResizeGizmo.activeSelf)
			{
				ResizeGizmo.SetActive(value: false);
			}
		}
	}
}

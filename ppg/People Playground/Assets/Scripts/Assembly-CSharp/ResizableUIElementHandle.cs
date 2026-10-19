using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResizableUIElementHandle : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
	public Texture2D CursorSprite;

	public Image Graphic;

	public Color Default = Color.white;

	public Color Hover = Color.white;

	public Color Active = Color.white;

	public RectTransform ToResize;

	public RectTransform.Axis Axis;

	[MinMaxSlider(16f, 2688f)]
	public Vector2 SizeRange = new Vector2(300f, 800f);

	public float Step = 1f;

	public UnityEvent OnEndResize;

	private bool isBeingDragged;

	private float currentDraggingSize;

	private float initialSize;

	private Vector3 initialMousePos;

	private CanvasScaler canvasScaler;

	private Camera cam;

	private void Start()
	{
		cam = Camera.main;
		Graphic.color = Default;
		canvasScaler = Object.FindObjectOfType<CanvasScaler>();
	}

	public void OnDeselect(BaseEventData eventData)
	{
		StopResizing();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			if ((bool)EventSystem.current && !EventSystem.current.alreadySelecting)
			{
				EventSystem.current.SetSelectedGameObject(base.gameObject);
			}
			isBeingDragged = true;
			switch (Axis)
			{
			case RectTransform.Axis.Horizontal:
				currentDraggingSize = ToResize.sizeDelta.x;
				break;
			case RectTransform.Axis.Vertical:
				currentDraggingSize = ToResize.sizeDelta.y;
				break;
			}
			initialSize = currentDraggingSize;
			initialMousePos = Input.mousePosition;
			Graphic.color = Active;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		Graphic.color = Hover;
		Cursor.SetCursor(CursorSprite, new Vector2(12f, 5f), CursorMode.Auto);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if ((bool)EventSystem.current && EventSystem.current.currentSelectedGameObject == base.gameObject)
		{
			Graphic.color = Active;
			Cursor.SetCursor(CursorSprite, new Vector2(12f, 5f), CursorMode.Auto);
		}
		else
		{
			Graphic.color = Default;
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if ((bool)EventSystem.current && !EventSystem.current.alreadySelecting)
		{
			EventSystem.current.SetSelectedGameObject(null);
		}
		StopResizing();
	}

	private void StopResizing()
	{
		isBeingDragged = false;
		Graphic.color = Default;
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		OnEndResize?.Invoke();
	}

	private void Update()
	{
		if (!isBeingDragged)
		{
			return;
		}
		if (!ToResize.gameObject.activeInHierarchy || !base.gameObject.activeInHierarchy)
		{
			StopResizing();
			return;
		}
		float num = ((Axis == RectTransform.Axis.Horizontal) ? cam.pixelWidth : cam.pixelHeight);
		Vector3 vector = Utils.Snap((Input.mousePosition - initialMousePos) * canvasScaler.scaleFactor / num * canvasScaler.referenceResolution, Step);
		switch (Axis)
		{
		case RectTransform.Axis.Horizontal:
			currentDraggingSize = initialSize + vector.x;
			break;
		case RectTransform.Axis.Vertical:
			currentDraggingSize = initialSize + vector.y;
			break;
		}
		currentDraggingSize = Mathf.Clamp(currentDraggingSize, SizeRange.x, SizeRange.y);
		ToResize.SetSizeWithCurrentAnchors(Axis, currentDraggingSize);
	}

	private void OnDisable()
	{
		StopResizing();
	}
}

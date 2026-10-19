using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("UI/Extensions/UI Window Base")]
	public class UIWindowBase : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
	{
		private bool _isDragging;

		public static bool ResetCoords;

		private Vector3 m_originalCoods = Vector3.zero;

		private Canvas m_canvas;

		private RectTransform m_canvasRectTransform;

		[Tooltip("Number of pixels of the window that must stay inside the canvas view.")]
		public int KeepWindowInCanvas = 5;

		[Tooltip("The transform that is moved when dragging, can be left empty in which case its own transform is used.")]
		public RectTransform RootTransform;

		private void Start()
		{
			if (RootTransform == null)
			{
				RootTransform = GetComponent<RectTransform>();
			}
			m_originalCoods = RootTransform.position;
			m_canvas = GetComponentInParent<Canvas>();
			m_canvasRectTransform = m_canvas.GetComponent<RectTransform>();
		}

		private void Update()
		{
			if (ResetCoords)
			{
				resetCoordinatePosition();
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (_isDragging)
			{
				Vector3 vector = ScreenToCanvas(eventData.position) - ScreenToCanvas(eventData.position - eventData.delta);
				RootTransform.localPosition += vector;
			}
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (!(eventData.pointerCurrentRaycast.gameObject == null) && eventData.pointerCurrentRaycast.gameObject.name == base.name)
			{
				_isDragging = true;
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			_isDragging = false;
		}

		private void resetCoordinatePosition()
		{
			RootTransform.position = m_originalCoods;
			ResetCoords = false;
		}

		private Vector3 ScreenToCanvas(Vector3 screenPosition)
		{
			Vector2 sizeDelta = m_canvasRectTransform.sizeDelta;
			Vector3 result;
			Vector2 vector;
			Vector2 vector2;
			if (m_canvas.renderMode == RenderMode.ScreenSpaceOverlay || (m_canvas.renderMode == RenderMode.ScreenSpaceCamera && m_canvas.worldCamera == null))
			{
				result = screenPosition;
				vector = Vector2.zero;
				vector2 = sizeDelta;
			}
			else
			{
				Ray ray = m_canvas.worldCamera.ScreenPointToRay(screenPosition);
				if (!new Plane(m_canvasRectTransform.forward, m_canvasRectTransform.position).Raycast(ray, out var enter))
				{
					throw new Exception("Is it practically possible?");
				}
				Vector3 position = ray.origin + ray.direction * enter;
				result = m_canvasRectTransform.InverseTransformPoint(position);
				vector = -Vector2.Scale(sizeDelta, m_canvasRectTransform.pivot);
				vector2 = Vector2.Scale(sizeDelta, Vector2.one - m_canvasRectTransform.pivot);
			}
			result.x = Mathf.Clamp(result.x, vector.x + (float)KeepWindowInCanvas, vector2.x - (float)KeepWindowInCanvas);
			result.y = Mathf.Clamp(result.y, vector.y + (float)KeepWindowInCanvas, vector2.y - (float)KeepWindowInCanvas);
			return result;
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/UI Magnetic Infinite Scroll")]
	[RequireComponent(typeof(ScrollRect))]
	public class UI_MagneticInfiniteScroll : UI_InfiniteScroll, IDragHandler, IEventSystemHandler, IEndDragHandler, IScrollHandler
	{
		[Tooltip("The pointer to the pivot, the visual element for centering objects.")]
		[SerializeField]
		private RectTransform pivot;

		[Tooltip("The maximum speed that allows you to activate the magnet to center on the pivot")]
		[SerializeField]
		private float maxSpeedForMagnetic = 10f;

		[SerializeField]
		[Tooltip("The index of the object which must be initially centered")]
		private int indexStart;

		[SerializeField]
		[Tooltip("The time to decelerate and aim to the pivot")]
		private float timeForDeceleration = 0.05f;

		private float _pastPositionMouseSpeed;

		private float _initMovementDirection;

		private float _pastPosition;

		private float _currentSpeed;

		private float _stopValue;

		private readonly float _waitForContentSet = 0.1f;

		private float _currentTime;

		private int _nearestIndex;

		private bool _useMagnetic = true;

		private bool _isStopping;

		private bool _isMovement;

		public List<RectTransform> Items { get; }

		public event Action<GameObject> OnNewSelect;

		protected override void Awake()
		{
			base.Awake();
			StartCoroutine(SetInitContent());
		}

		private void Update()
		{
			if (_scrollRect == null || !_scrollRect.content || !pivot || !_useMagnetic || !_isMovement || items == null)
			{
				return;
			}
			float rightAxis = GetRightAxis(_scrollRect.content.anchoredPosition);
			_currentSpeed = Mathf.Abs(rightAxis - _pastPosition);
			_pastPosition = rightAxis;
			if (Mathf.Abs(_currentSpeed) > maxSpeedForMagnetic)
			{
				return;
			}
			if (_isStopping)
			{
				Vector2 anchoredPosition = _scrollRect.content.anchoredPosition;
				_currentTime += Time.deltaTime;
				float t = _currentTime / timeForDeceleration;
				float num = Mathf.Lerp(GetRightAxis(anchoredPosition), _stopValue, t);
				_scrollRect.content.anchoredPosition = (_isVertical ? new Vector2(anchoredPosition.x, num) : new Vector2(num, anchoredPosition.y));
				if (num == GetRightAxis(anchoredPosition) && _nearestIndex > 0 && _nearestIndex < items.Count)
				{
					_isStopping = false;
					_isMovement = false;
					RectTransform rectTransform = items[_nearestIndex];
					if (rectTransform != null && this.OnNewSelect != null)
					{
						this.OnNewSelect(rectTransform.gameObject);
					}
				}
				return;
			}
			float num2 = float.PositiveInfinity * (0f - _initMovementDirection);
			for (int i = 0; i < items.Count; i++)
			{
				RectTransform rectTransform2 = items[i];
				if (!(rectTransform2 == null))
				{
					float num3 = GetRightAxis(rectTransform2.position) - GetRightAxis(pivot.position);
					if ((_initMovementDirection <= 0f && num3 < num2 && num3 > 0f) || (_initMovementDirection > 0f && num3 > num2 && num3 < 0f))
					{
						num2 = num3;
						_nearestIndex = i;
					}
				}
			}
			_isStopping = true;
			_stopValue = GetAnchoredPositionForPivot(_nearestIndex);
			_scrollRect.StopMovement();
		}

		public override void SetNewItems(ref List<Transform> newItems)
		{
			foreach (Transform newItem in newItems)
			{
				RectTransform component = newItem.GetComponent<RectTransform>();
				if ((bool)component && (bool)pivot)
				{
					component.sizeDelta = pivot.sizeDelta;
				}
			}
			base.SetNewItems(ref newItems);
		}

		public void SetContentInPivot(int index)
		{
			float anchoredPositionForPivot = GetAnchoredPositionForPivot(index);
			Vector2 anchoredPosition = _scrollRect.content.anchoredPosition;
			if ((bool)_scrollRect.content)
			{
				_scrollRect.content.anchoredPosition = (_isVertical ? new Vector2(anchoredPosition.x, anchoredPositionForPivot) : new Vector2(anchoredPositionForPivot, anchoredPosition.y));
				_pastPosition = GetRightAxis(_scrollRect.content.anchoredPosition);
			}
		}

		private IEnumerator SetInitContent()
		{
			yield return new WaitForSeconds(_waitForContentSet);
			SetContentInPivot(indexStart);
		}

		private float GetAnchoredPositionForPivot(int index)
		{
			if (!pivot || items == null || items.Count < 0)
			{
				return 0f;
			}
			index = Mathf.Clamp(index, 0, items.Count - 1);
			float rightAxis = GetRightAxis(items[index].anchoredPosition);
			return GetRightAxis(pivot.anchoredPosition) - rightAxis;
		}

		private void FinishPrepareMovement()
		{
			_isMovement = true;
			_useMagnetic = true;
			_isStopping = false;
			_currentTime = 0f;
		}

		private float GetRightAxis(Vector2 vector)
		{
			if (!_isVertical)
			{
				return vector.x;
			}
			return vector.y;
		}

		public void OnDrag(PointerEventData eventData)
		{
			float rightAxis = GetRightAxis(UIExtensionsInputManager.MousePosition);
			_initMovementDirection = Mathf.Sign(rightAxis - _pastPositionMouseSpeed);
			_pastPositionMouseSpeed = rightAxis;
			_useMagnetic = false;
			_isStopping = false;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			FinishPrepareMovement();
		}

		public void OnScroll(PointerEventData eventData)
		{
			_initMovementDirection = 0f - UIExtensionsInputManager.MouseScrollDelta.y;
			FinishPrepareMovement();
		}
	}
}

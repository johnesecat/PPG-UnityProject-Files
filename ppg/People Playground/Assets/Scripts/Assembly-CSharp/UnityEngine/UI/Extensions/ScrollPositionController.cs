using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	public class ScrollPositionController : UIBehaviour, IBeginDragHandler, IEventSystemHandler, IEndDragHandler, IDragHandler
	{
		private enum ScrollDirection
		{
			Vertical,
			Horizontal
		}

		private enum MovementType
		{
			Unrestricted,
			Elastic,
			Clamped
		}

		[Serializable]
		private struct Snap
		{
			public bool Enable;

			public float VelocityThreshold;

			public float Duration;
		}

		private class AutoScrollState
		{
			public bool Enable;

			public bool Elastic;

			public float Duration;

			public float StartTime;

			public float EndScrollPosition;

			public void Reset()
			{
				Enable = false;
				Elastic = false;
				Duration = 0f;
				StartTime = 0f;
				EndScrollPosition = 0f;
			}
		}

		[SerializeField]
		private RectTransform viewport;

		[SerializeField]
		private ScrollDirection directionOfRecognize;

		[SerializeField]
		private MovementType movementType = MovementType.Elastic;

		[SerializeField]
		private float elasticity = 0.1f;

		[SerializeField]
		private float scrollSensitivity = 1f;

		[SerializeField]
		private bool inertia = true;

		[SerializeField]
		[Tooltip("Only used when inertia is enabled")]
		private float decelerationRate = 0.03f;

		[SerializeField]
		[Tooltip("Only used when inertia is enabled")]
		private Snap snap = new Snap
		{
			Enable = true,
			VelocityThreshold = 0.5f,
			Duration = 0.3f
		};

		[SerializeField]
		private int dataCount;

		private readonly AutoScrollState autoScrollState = new AutoScrollState();

		private Action<float> onUpdatePosition;

		private Action<int> onItemSelected;

		private Vector2 pointerStartLocalPosition;

		private float dragStartScrollPosition;

		private float prevScrollPosition;

		private float currentScrollPosition;

		private bool dragging;

		private float velocity;

		public void OnUpdatePosition(Action<float> onUpdatePosition)
		{
			this.onUpdatePosition = onUpdatePosition;
		}

		public void OnItemSelected(Action<int> onItemSelected)
		{
			this.onItemSelected = onItemSelected;
		}

		public void SetDataCount(int dataCount)
		{
			this.dataCount = dataCount;
		}

		public void ScrollTo(int index, float duration)
		{
			autoScrollState.Reset();
			autoScrollState.Enable = true;
			autoScrollState.Duration = duration;
			autoScrollState.StartTime = Time.unscaledTime;
			autoScrollState.EndScrollPosition = CalculateDestinationIndex(index);
			velocity = 0f;
			dragStartScrollPosition = currentScrollPosition;
			ItemSelected(Mathf.RoundToInt(GetCircularPosition(autoScrollState.EndScrollPosition, dataCount)));
		}

		public void JumpTo(int index)
		{
			autoScrollState.Reset();
			velocity = 0f;
			dragging = false;
			index = CalculateDestinationIndex(index);
			ItemSelected(index);
			UpdatePosition(index);
		}

		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				pointerStartLocalPosition = Vector2.zero;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, eventData.position, eventData.pressEventCamera, out pointerStartLocalPosition);
				dragStartScrollPosition = currentScrollPosition;
				dragging = true;
				autoScrollState.Reset();
			}
		}

		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left && dragging && RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, eventData.position, eventData.pressEventCamera, out var localPoint))
			{
				Vector2 vector = localPoint - pointerStartLocalPosition;
				float num = ((directionOfRecognize == ScrollDirection.Horizontal) ? (0f - vector.x) : vector.y) / GetViewportSize() * scrollSensitivity + dragStartScrollPosition;
				float num2 = CalculateOffset(num);
				num += num2;
				if (movementType == MovementType.Elastic && num2 != 0f)
				{
					num -= RubberDelta(num2, scrollSensitivity);
				}
				UpdatePosition(num);
			}
		}

		void IEndDragHandler.OnEndDrag(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				dragging = false;
			}
		}

		private float GetViewportSize()
		{
			if (directionOfRecognize != ScrollDirection.Horizontal)
			{
				return viewport.rect.size.y;
			}
			return viewport.rect.size.x;
		}

		private float CalculateOffset(float position)
		{
			if (movementType == MovementType.Unrestricted)
			{
				return 0f;
			}
			if (position < 0f)
			{
				return 0f - position;
			}
			if (position > (float)(dataCount - 1))
			{
				return (float)(dataCount - 1) - position;
			}
			return 0f;
		}

		private void UpdatePosition(float position)
		{
			currentScrollPosition = position;
			if (onUpdatePosition != null)
			{
				onUpdatePosition(currentScrollPosition);
			}
		}

		private void ItemSelected(int index)
		{
			if (onItemSelected != null)
			{
				onItemSelected(index);
			}
		}

		private float RubberDelta(float overStretching, float viewSize)
		{
			return (1f - 1f / (Mathf.Abs(overStretching) * 0.55f / viewSize + 1f)) * viewSize * Mathf.Sign(overStretching);
		}

		private void Update()
		{
			float unscaledDeltaTime = Time.unscaledDeltaTime;
			float num = CalculateOffset(currentScrollPosition);
			if (autoScrollState.Enable)
			{
				float num2 = 0f;
				if (autoScrollState.Elastic)
				{
					float currentVelocity = velocity;
					num2 = Mathf.SmoothDamp(currentScrollPosition, currentScrollPosition + num, ref currentVelocity, elasticity, float.PositiveInfinity, unscaledDeltaTime);
					velocity = currentVelocity;
					if (Mathf.Abs(velocity) < 0.01f)
					{
						num2 = Mathf.Clamp(Mathf.RoundToInt(num2), 0, dataCount - 1);
						velocity = 0f;
						autoScrollState.Reset();
					}
				}
				else
				{
					float num3 = Mathf.Clamp01((Time.unscaledTime - autoScrollState.StartTime) / Mathf.Max(autoScrollState.Duration, float.Epsilon));
					num2 = Mathf.Lerp(dragStartScrollPosition, autoScrollState.EndScrollPosition, EaseInOutCubic(0f, 1f, num3));
					if (Mathf.Approximately(num3, 1f))
					{
						autoScrollState.Reset();
					}
				}
				UpdatePosition(num2);
			}
			else if (!dragging && (!Mathf.Approximately(num, 0f) || !Mathf.Approximately(velocity, 0f)))
			{
				float num4 = currentScrollPosition;
				if (movementType == MovementType.Elastic && !Mathf.Approximately(num, 0f))
				{
					autoScrollState.Reset();
					autoScrollState.Enable = true;
					autoScrollState.Elastic = true;
					ItemSelected(Mathf.Clamp(Mathf.RoundToInt(num4), 0, dataCount - 1));
				}
				else if (inertia)
				{
					velocity *= Mathf.Pow(decelerationRate, unscaledDeltaTime);
					if (Mathf.Abs(velocity) < 0.001f)
					{
						velocity = 0f;
					}
					num4 += velocity * unscaledDeltaTime;
					if (snap.Enable && Mathf.Abs(velocity) < snap.VelocityThreshold)
					{
						ScrollTo(Mathf.RoundToInt(currentScrollPosition), snap.Duration);
					}
				}
				else
				{
					velocity = 0f;
				}
				if (!Mathf.Approximately(velocity, 0f))
				{
					if (movementType == MovementType.Clamped)
					{
						num = CalculateOffset(num4);
						num4 += num;
						if (Mathf.Approximately(num4, 0f) || Mathf.Approximately(num4, (float)dataCount - 1f))
						{
							velocity = 0f;
							ItemSelected(Mathf.RoundToInt(num4));
						}
					}
					UpdatePosition(num4);
				}
			}
			if (!autoScrollState.Enable && dragging && inertia)
			{
				float b = (currentScrollPosition - prevScrollPosition) / unscaledDeltaTime;
				velocity = Mathf.Lerp(velocity, b, unscaledDeltaTime * 10f);
			}
			if (currentScrollPosition != prevScrollPosition)
			{
				prevScrollPosition = currentScrollPosition;
			}
		}

		private int CalculateDestinationIndex(int index)
		{
			if (movementType != 0)
			{
				return Mathf.Clamp(index, 0, dataCount - 1);
			}
			return CalculateClosestIndex(index);
		}

		private int CalculateClosestIndex(int index)
		{
			float num = GetCircularPosition(index, dataCount) - GetCircularPosition(currentScrollPosition, dataCount);
			if (Mathf.Abs(num) > (float)dataCount * 0.5f)
			{
				num = Mathf.Sign(0f - num) * ((float)dataCount - Mathf.Abs(num));
			}
			return Mathf.RoundToInt(num + currentScrollPosition);
		}

		private float GetCircularPosition(float position, int length)
		{
			if (!(position < 0f))
			{
				return position % (float)length;
			}
			return (float)(length - 1) + (position + 1f) % (float)length;
		}

		private float EaseInOutCubic(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end * 0.5f * value * value * value + start;
			}
			value -= 2f;
			return end * 0.5f * (value * value * value + 2f) + start;
		}
	}
}

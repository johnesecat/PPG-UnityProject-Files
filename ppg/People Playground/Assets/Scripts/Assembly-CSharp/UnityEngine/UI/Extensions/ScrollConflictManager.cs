using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(ScrollRect))]
	[AddComponentMenu("UI/Extensions/Scrollrect Conflict Manager")]
	public class ScrollConflictManager : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IEndDragHandler, IDragHandler
	{
		[Tooltip("The parent ScrollRect control hosting this ScrollSnap")]
		public ScrollRect ParentScrollRect;

		[Tooltip("The parent ScrollSnap control hosting this Scroll Snap.\nIf left empty, it will use the ScrollSnap of the ParentScrollRect")]
		private ScrollSnapBase ParentScrollSnap;

		private ScrollRect _myScrollRect;

		private IBeginDragHandler[] _beginDragHandlers;

		private IEndDragHandler[] _endDragHandlers;

		private IDragHandler[] _dragHandlers;

		private bool scrollOther;

		private bool scrollOtherHorizontally;

		private void Awake()
		{
			if ((bool)ParentScrollRect)
			{
				InitialiseConflictManager();
			}
		}

		private void InitialiseConflictManager()
		{
			_myScrollRect = GetComponent<ScrollRect>();
			scrollOtherHorizontally = _myScrollRect.vertical;
			if (scrollOtherHorizontally)
			{
				if (_myScrollRect.horizontal)
				{
					Debug.LogError("You have added the SecondScrollRect to a scroll view that already has both directions selected");
				}
				if (!ParentScrollRect.horizontal)
				{
					Debug.LogError("The other scroll rect does not support scrolling horizontally");
				}
			}
			else if (!ParentScrollRect.vertical)
			{
				Debug.LogError("The other scroll rect does not support scrolling vertically");
			}
			if ((bool)ParentScrollRect && !ParentScrollSnap)
			{
				ParentScrollSnap = ParentScrollRect.GetComponent<ScrollSnapBase>();
			}
		}

		private void Start()
		{
			if ((bool)ParentScrollRect)
			{
				AssignScrollRectHandlers();
			}
		}

		private void AssignScrollRectHandlers()
		{
			_beginDragHandlers = ParentScrollRect.GetComponents<IBeginDragHandler>();
			_dragHandlers = ParentScrollRect.GetComponents<IDragHandler>();
			_endDragHandlers = ParentScrollRect.GetComponents<IEndDragHandler>();
		}

		public void SetParentScrollRect(ScrollRect parentScrollRect)
		{
			ParentScrollRect = parentScrollRect;
			InitialiseConflictManager();
			AssignScrollRectHandlers();
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			float num = Mathf.Abs(eventData.position.x - eventData.pressPosition.x);
			float num2 = Mathf.Abs(eventData.position.y - eventData.pressPosition.y);
			if (scrollOtherHorizontally)
			{
				if (!(num > num2))
				{
					return;
				}
				scrollOther = true;
				_myScrollRect.enabled = false;
				int i = 0;
				for (int num3 = _beginDragHandlers.Length; i < num3; i++)
				{
					_beginDragHandlers[i].OnBeginDrag(eventData);
					if ((bool)ParentScrollSnap)
					{
						ParentScrollSnap.OnBeginDrag(eventData);
					}
				}
			}
			else
			{
				if (!(num2 > num))
				{
					return;
				}
				scrollOther = true;
				_myScrollRect.enabled = false;
				int j = 0;
				for (int num4 = _beginDragHandlers.Length; j < num4; j++)
				{
					_beginDragHandlers[j].OnBeginDrag(eventData);
					if ((bool)ParentScrollSnap)
					{
						ParentScrollSnap.OnBeginDrag(eventData);
					}
				}
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (!scrollOther)
			{
				return;
			}
			_myScrollRect.enabled = true;
			scrollOther = false;
			int i = 0;
			for (int num = _endDragHandlers.Length; i < num; i++)
			{
				_endDragHandlers[i].OnEndDrag(eventData);
				if ((bool)ParentScrollSnap)
				{
					ParentScrollSnap.OnEndDrag(eventData);
				}
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (!scrollOther)
			{
				return;
			}
			int i = 0;
			for (int num = _endDragHandlers.Length; i < num; i++)
			{
				_dragHandlers[i].OnDrag(eventData);
				if ((bool)ParentScrollSnap)
				{
					ParentScrollSnap.OnDrag(eventData);
				}
			}
		}
	}
}

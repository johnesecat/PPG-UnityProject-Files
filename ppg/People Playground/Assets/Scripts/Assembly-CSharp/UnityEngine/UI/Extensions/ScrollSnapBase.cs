using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	public class ScrollSnapBase : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler, IScrollSnap, IPointerClickHandler
	{
		[Serializable]
		public class SelectionChangeStartEvent : UnityEvent
		{
		}

		[Serializable]
		public class SelectionPageChangedEvent : UnityEvent<int>
		{
		}

		[Serializable]
		public class SelectionChangeEndEvent : UnityEvent<int>
		{
		}

		internal Rect panelDimensions;

		internal RectTransform _screensContainer;

		internal bool _isVertical;

		internal int _screens = 1;

		internal float _scrollStartPosition;

		internal float _childSize;

		private float _childPos;

		private float _maskSize;

		internal Vector2 _childAnchorPoint;

		internal ScrollRect _scroll_rect;

		internal Vector3 _lerp_target;

		internal bool _lerp;

		internal bool _pointerDown;

		internal bool _settled = true;

		internal Vector3 _startPosition;

		[Tooltip("The currently active page")]
		internal int _currentPage;

		internal int _previousPage;

		internal int _halfNoVisibleItems;

		internal bool _isInfinite;

		internal int _infiniteWindow;

		internal float _infiniteOffset;

		private int _bottomItem;

		private int _topItem;

		internal bool _startEventCalled;

		internal bool _endEventCalled;

		internal bool _suspendEvents;

		[Tooltip("The screen / page to start the control on\n*Note, this is a 0 indexed array")]
		[SerializeField]
		public int StartingScreen;

		[Tooltip("The distance between two pages based on page height, by default pages are next to each other")]
		[SerializeField]
		[Range(0f, 8f)]
		public float PageStep = 1f;

		[Tooltip("The gameobject that contains toggles which suggest pagination. (optional)")]
		public GameObject Pagination;

		[Tooltip("Button to go to the previous page. (optional)")]
		public GameObject PrevButton;

		[Tooltip("Button to go to the next page. (optional)")]
		public GameObject NextButton;

		[Tooltip("Transition speed between pages. (optional)")]
		public float transitionSpeed = 7.5f;

		[Tooltip("Hard Swipe forces to swiping to the next / previous page (optional)")]
		public bool UseHardSwipe;

		[Tooltip("Fast Swipe makes swiping page next / previous (optional)")]
		public bool UseFastSwipe;

		[Tooltip("Swipe Delta Threshold looks at the speed of input to decide if a swipe will be initiated (optional)")]
		public bool UseSwipeDeltaThreshold;

		[Tooltip("Offset for how far a swipe has to travel to initiate a page change (optional)")]
		public int FastSwipeThreshold = 100;

		[Tooltip("Speed at which the ScrollRect will keep scrolling before slowing down and stopping (optional)")]
		public int SwipeVelocityThreshold = 100;

		[Tooltip("Threshold for swipe speed to initiate a swipe, below threshold will return to closest page (optional)")]
		public float SwipeDeltaThreshold = 5f;

		[Tooltip("Use time scale instead of unscaled time (optional)")]
		public bool UseTimeScale = true;

		[Tooltip("The visible bounds area, controls which items are visible/enabled. *Note Should use a RectMask. (optional)")]
		public RectTransform MaskArea;

		[Tooltip("Pixel size to buffer around Mask Area. (optional)")]
		public float MaskBuffer = 1f;

		[Tooltip("By default the container will lerp to the start when enabled in the scene, this option overrides this and forces it to simply jump without lerping")]
		public bool JumpOnEnable;

		[Tooltip("By default the container will return to the original starting page when enabled, this option overrides this behaviour and stays on the current selection")]
		public bool RestartOnEnable;

		[Tooltip("(Experimental)\nBy default, child array objects will use the parent transform\nHowever you can disable this for some interesting effects")]
		public bool UseParentTransform = true;

		[Tooltip("Scroll Snap children. (optional)\nEither place objects in the scene as children OR\nPrefabs in this array, NOT BOTH")]
		public GameObject[] ChildObjects;

		[SerializeField]
		[Tooltip("Event fires when a user starts to change the selection")]
		private SelectionChangeStartEvent m_OnSelectionChangeStartEvent = new SelectionChangeStartEvent();

		[SerializeField]
		[Tooltip("Event fires as the page changes, while dragging or jumping")]
		private SelectionPageChangedEvent m_OnSelectionPageChangedEvent = new SelectionPageChangedEvent();

		[SerializeField]
		[Tooltip("Event fires when the page settles after a user has dragged")]
		private SelectionChangeEndEvent m_OnSelectionChangeEndEvent = new SelectionChangeEndEvent();

		public int CurrentPage
		{
			get
			{
				return _currentPage;
			}
			internal set
			{
				if (_isInfinite)
				{
					float num = (float)value / (float)_screensContainer.childCount;
					if (num < 0f)
					{
						_infiniteWindow = (int)Math.Floor(num);
					}
					else
					{
						_infiniteWindow = value / _screensContainer.childCount;
					}
					_infiniteWindow = ((value < 0) ? (-_infiniteWindow) : _infiniteWindow);
					value %= _screensContainer.childCount;
					if (value < 0)
					{
						value = _screensContainer.childCount + value;
					}
					else if (value > _screensContainer.childCount - 1)
					{
						value -= _screensContainer.childCount;
					}
				}
				if ((value != _currentPage && value >= 0 && value < _screensContainer.childCount) || (value == 0 && _screensContainer.childCount == 0))
				{
					_previousPage = _currentPage;
					_currentPage = value;
					if ((bool)MaskArea)
					{
						UpdateVisible();
					}
					if (!_lerp)
					{
						ScreenChange();
					}
					OnCurrentScreenChange(_currentPage);
				}
			}
		}

		public SelectionChangeStartEvent OnSelectionChangeStartEvent
		{
			get
			{
				return m_OnSelectionChangeStartEvent;
			}
			set
			{
				m_OnSelectionChangeStartEvent = value;
			}
		}

		public SelectionPageChangedEvent OnSelectionPageChangedEvent
		{
			get
			{
				return m_OnSelectionPageChangedEvent;
			}
			set
			{
				m_OnSelectionPageChangedEvent = value;
			}
		}

		public SelectionChangeEndEvent OnSelectionChangeEndEvent
		{
			get
			{
				return m_OnSelectionChangeEndEvent;
			}
			set
			{
				m_OnSelectionChangeEndEvent = value;
			}
		}

		private void Awake()
		{
			if (_scroll_rect == null)
			{
				_scroll_rect = base.gameObject.GetComponent<ScrollRect>();
			}
			if ((bool)_scroll_rect.horizontalScrollbar && _scroll_rect.horizontal)
			{
				_scroll_rect.horizontalScrollbar.gameObject.AddComponent<ScrollSnapScrollbarHelper>().ss = this;
			}
			if ((bool)_scroll_rect.verticalScrollbar && _scroll_rect.vertical)
			{
				_scroll_rect.verticalScrollbar.gameObject.AddComponent<ScrollSnapScrollbarHelper>().ss = this;
			}
			panelDimensions = base.gameObject.GetComponent<RectTransform>().rect;
			if (StartingScreen < 0)
			{
				StartingScreen = 0;
			}
			_screensContainer = _scroll_rect.content;
			InitialiseChildObjects();
			if ((bool)NextButton)
			{
				NextButton.GetComponent<Button>().onClick.AddListener(delegate
				{
					NextScreen();
				});
			}
			if ((bool)PrevButton)
			{
				PrevButton.GetComponent<Button>().onClick.AddListener(delegate
				{
					PreviousScreen();
				});
			}
			_isInfinite = GetComponent<UI_InfiniteScroll>() != null;
		}

		internal void InitialiseChildObjects()
		{
			if (ChildObjects != null && ChildObjects.Length != 0)
			{
				if (_screensContainer.transform.childCount > 0)
				{
					Debug.LogError("ScrollRect Content has children, this is not supported when using managed Child Objects\n Either remove the ScrollRect Content children or clear the ChildObjects array");
					return;
				}
				InitialiseChildObjectsFromArray();
				if (GetComponent<UI_InfiniteScroll>() != null)
				{
					GetComponent<UI_InfiniteScroll>().Init();
				}
			}
			else
			{
				InitialiseChildObjectsFromScene();
			}
		}

		internal void InitialiseChildObjectsFromScene()
		{
			int childCount = _screensContainer.childCount;
			ChildObjects = new GameObject[childCount];
			for (int i = 0; i < childCount; i++)
			{
				ChildObjects[i] = _screensContainer.transform.GetChild(i).gameObject;
				if ((bool)MaskArea && ChildObjects[i].activeSelf)
				{
					ChildObjects[i].SetActive(value: false);
				}
			}
		}

		internal void InitialiseChildObjectsFromArray()
		{
			int num = ChildObjects.Length;
			for (int i = 0; i < num; i++)
			{
				GameObject gameObject = Object.Instantiate(ChildObjects[i]);
				if (UseParentTransform)
				{
					RectTransform component = gameObject.GetComponent<RectTransform>();
					component.rotation = _screensContainer.rotation;
					component.localScale = _screensContainer.localScale;
					component.position = _screensContainer.position;
				}
				gameObject.transform.SetParent(_screensContainer.transform);
				ChildObjects[i] = gameObject;
				if ((bool)MaskArea && ChildObjects[i].activeSelf)
				{
					ChildObjects[i].SetActive(value: false);
				}
			}
		}

		internal void UpdateVisible()
		{
			if (!MaskArea || ChildObjects == null || ChildObjects.Length < 1 || _screensContainer.childCount < 1)
			{
				return;
			}
			_maskSize = (_isVertical ? MaskArea.rect.height : MaskArea.rect.width);
			_halfNoVisibleItems = (int)Math.Round(_maskSize / (_childSize * MaskBuffer), MidpointRounding.AwayFromZero) / 2;
			_bottomItem = (_topItem = 0);
			for (int num = _halfNoVisibleItems + 1; num > 0; num--)
			{
				_bottomItem = ((_currentPage - num >= 0) ? num : 0);
				if (_bottomItem > 0)
				{
					break;
				}
			}
			for (int num2 = _halfNoVisibleItems + 1; num2 > 0; num2--)
			{
				_topItem = ((_screensContainer.childCount - _currentPage - num2 >= 0) ? num2 : 0);
				if (_topItem > 0)
				{
					break;
				}
			}
			for (int i = CurrentPage - _bottomItem; i < CurrentPage + _topItem; i++)
			{
				try
				{
					ChildObjects[i].SetActive(value: true);
				}
				catch
				{
					Debug.Log("Failed to setactive child [" + i + "]");
				}
			}
			if (_currentPage > _halfNoVisibleItems)
			{
				ChildObjects[CurrentPage - _bottomItem].SetActive(value: false);
			}
			if (_screensContainer.childCount - _currentPage > _topItem)
			{
				ChildObjects[CurrentPage + _topItem].SetActive(value: false);
			}
		}

		public void NextScreen()
		{
			if (_currentPage < _screens - 1 || _isInfinite)
			{
				if (!_lerp)
				{
					StartScreenChange();
				}
				_lerp = true;
				if (_isInfinite)
				{
					CurrentPage = GetPageforPosition(_screensContainer.anchoredPosition) + 1;
				}
				else
				{
					CurrentPage = _currentPage + 1;
				}
				GetPositionforPage(_currentPage, ref _lerp_target);
				ScreenChange();
			}
		}

		public void PreviousScreen()
		{
			if (_currentPage > 0 || _isInfinite)
			{
				if (!_lerp)
				{
					StartScreenChange();
				}
				_lerp = true;
				if (_isInfinite)
				{
					CurrentPage = GetPageforPosition(_screensContainer.anchoredPosition) - 1;
				}
				else
				{
					CurrentPage = _currentPage - 1;
				}
				GetPositionforPage(_currentPage, ref _lerp_target);
				ScreenChange();
			}
		}

		public void GoToScreen(int screenIndex, bool pagination = false)
		{
			if (screenIndex <= _screens - 1 && screenIndex >= 0)
			{
				if (!_lerp || pagination)
				{
					StartScreenChange();
				}
				_lerp = true;
				CurrentPage = screenIndex;
				GetPositionforPage(_currentPage, ref _lerp_target);
				ScreenChange();
			}
		}

		internal int GetPageforPosition(Vector3 pos)
		{
			if (!_isVertical)
			{
				return (int)Math.Round((_scrollStartPosition - pos.x) / _childSize);
			}
			return (int)Math.Round((_scrollStartPosition - pos.y) / _childSize);
		}

		internal bool IsRectSettledOnaPage(Vector3 pos)
		{
			if (!_isVertical)
			{
				return 0f - (pos.x - _scrollStartPosition) / _childSize == (float)(-(int)Math.Round((pos.x - _scrollStartPosition) / _childSize));
			}
			return 0f - (pos.y - _scrollStartPosition) / _childSize == (float)(-(int)Math.Round((pos.y - _scrollStartPosition) / _childSize));
		}

		internal void GetPositionforPage(int page, ref Vector3 target)
		{
			_childPos = (0f - _childSize) * (float)page;
			if (_isVertical)
			{
				_infiniteOffset = ((_screensContainer.anchoredPosition.y < 0f) ? ((0f - _screensContainer.sizeDelta.y) * (float)_infiniteWindow) : (_screensContainer.sizeDelta.y * (float)_infiniteWindow));
				_infiniteOffset = ((_infiniteOffset == 0f) ? 0f : ((_infiniteOffset < 0f) ? (_infiniteOffset - _childSize * (float)_infiniteWindow) : (_infiniteOffset + _childSize * (float)_infiniteWindow)));
				target.y = _childPos + _scrollStartPosition + _infiniteOffset;
			}
			else
			{
				_infiniteOffset = ((_screensContainer.anchoredPosition.x < 0f) ? ((0f - _screensContainer.sizeDelta.x) * (float)_infiniteWindow) : (_screensContainer.sizeDelta.x * (float)_infiniteWindow));
				_infiniteOffset = ((_infiniteOffset == 0f) ? 0f : ((_infiniteOffset < 0f) ? (_infiniteOffset - _childSize * (float)_infiniteWindow) : (_infiniteOffset + _childSize * (float)_infiniteWindow)));
				target.x = _childPos + _scrollStartPosition + _infiniteOffset;
			}
		}

		internal void ScrollToClosestElement()
		{
			_lerp = true;
			CurrentPage = GetPageforPosition(_screensContainer.anchoredPosition);
			GetPositionforPage(_currentPage, ref _lerp_target);
			OnCurrentScreenChange(_currentPage);
		}

		internal void OnCurrentScreenChange(int currentScreen)
		{
			ChangeBulletsInfo(currentScreen);
			ToggleNavigationButtons(currentScreen);
		}

		private void ChangeBulletsInfo(int targetScreen)
		{
			if ((bool)Pagination)
			{
				for (int i = 0; i < Pagination.transform.childCount; i++)
				{
					Pagination.transform.GetChild(i).GetComponent<Toggle>().isOn = ((targetScreen == i) ? true : false);
				}
			}
		}

		private void ToggleNavigationButtons(int targetScreen)
		{
			if (!_isInfinite)
			{
				if ((bool)PrevButton)
				{
					PrevButton.GetComponent<Button>().interactable = targetScreen > 0;
				}
				if ((bool)NextButton)
				{
					NextButton.GetComponent<Button>().interactable = targetScreen < _screensContainer.transform.childCount - 1;
				}
			}
		}

		private void OnValidate()
		{
			if (_scroll_rect == null)
			{
				_scroll_rect = GetComponent<ScrollRect>();
			}
			if (!_scroll_rect.horizontal && !_scroll_rect.vertical)
			{
				Debug.LogError("ScrollRect has to have a direction, please select either Horizontal OR Vertical with the appropriate control.");
			}
			if (_scroll_rect.horizontal && _scroll_rect.vertical)
			{
				Debug.LogError("ScrollRect has to be unidirectional, only use either Horizontal or Vertical on the ScrollRect, NOT both.");
			}
			RectTransform content = base.gameObject.GetComponent<ScrollRect>().content;
			if (content != null)
			{
				int childCount = content.childCount;
				if (childCount != 0 || ChildObjects != null)
				{
					int num = ((ChildObjects == null || ChildObjects.Length == 0) ? childCount : ChildObjects.Length);
					if (StartingScreen > num - 1)
					{
						StartingScreen = num - 1;
					}
					if (StartingScreen < 0)
					{
						StartingScreen = 0;
					}
				}
			}
			if (MaskBuffer <= 0f)
			{
				MaskBuffer = 1f;
			}
			if (PageStep < 0f)
			{
				PageStep = 0f;
			}
			if (PageStep > 8f)
			{
				PageStep = 9f;
			}
			UI_InfiniteScroll component = GetComponent<UI_InfiniteScroll>();
			if (ChildObjects != null && ChildObjects.Length != 0 && component != null && !component.InitByUser)
			{
				Debug.LogError("[" + base.gameObject.name + "]When using procedural children with a ScrollSnap (Adding Prefab ChildObjects) and the Infinite Scroll component\nYou must set the 'InitByUser' option to true, to enable late initialising");
			}
		}

		public void StartScreenChange()
		{
			if (!_startEventCalled)
			{
				_suspendEvents = true;
				_startEventCalled = true;
				_endEventCalled = false;
				OnSelectionChangeStartEvent.Invoke();
			}
		}

		internal void ScreenChange()
		{
			OnSelectionPageChangedEvent.Invoke(_currentPage);
		}

		internal void EndScreenChange()
		{
			if (!_endEventCalled)
			{
				_suspendEvents = false;
				_endEventCalled = true;
				_startEventCalled = false;
				_settled = true;
				OnSelectionChangeEndEvent.Invoke(_currentPage);
			}
		}

		public Transform CurrentPageObject()
		{
			return _screensContainer.GetChild(CurrentPage);
		}

		public void CurrentPageObject(out Transform returnObject)
		{
			returnObject = _screensContainer.GetChild(CurrentPage);
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			_pointerDown = true;
			_settled = false;
			StartScreenChange();
			_startPosition = _screensContainer.anchoredPosition;
		}

		public void OnDrag(PointerEventData eventData)
		{
			_lerp = false;
		}

		public virtual void OnEndDrag(PointerEventData eventData)
		{
		}

		int IScrollSnap.CurrentPage()
		{
			return CurrentPage = GetPageforPosition(_screensContainer.anchoredPosition);
		}

		public void SetLerp(bool value)
		{
			_lerp = value;
		}

		public void ChangePage(int page)
		{
			GoToScreen(page);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			_ = _screensContainer.anchoredPosition;
		}
	}
}

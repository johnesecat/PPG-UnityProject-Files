using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/UI Infinite Scroll")]
	public class UI_InfiniteScroll : MonoBehaviour
	{
		[Tooltip("If false, will Init automatically, otherwise you need to call Init() method")]
		public bool InitByUser;

		protected ScrollRect _scrollRect;

		private ContentSizeFitter _contentSizeFitter;

		private VerticalLayoutGroup _verticalLayoutGroup;

		private HorizontalLayoutGroup _horizontalLayoutGroup;

		private GridLayoutGroup _gridLayoutGroup;

		protected bool _isVertical;

		protected bool _isHorizontal;

		private float _disableMarginX;

		private float _disableMarginY;

		private bool _hasDisabledGridComponents;

		protected List<RectTransform> items = new List<RectTransform>();

		private Vector2 _newAnchoredPosition = Vector2.zero;

		private float _threshold = 100f;

		private int _itemCount;

		private float _recordOffsetX;

		private float _recordOffsetY;

		protected virtual void Awake()
		{
			if (!InitByUser)
			{
				Init();
			}
		}

		public virtual void SetNewItems(ref List<Transform> newItems)
		{
			if (!(_scrollRect != null) || (_scrollRect.content == null && newItems == null))
			{
				return;
			}
			if (items != null)
			{
				items.Clear();
			}
			for (int num = _scrollRect.content.childCount - 1; num >= 0; num--)
			{
				Transform child = _scrollRect.content.GetChild(num);
				child.SetParent(null);
				Object.DestroyImmediate(child.gameObject);
			}
			foreach (Transform newItem in newItems)
			{
				newItem.SetParent(_scrollRect.content);
			}
			SetItems();
		}

		private void SetItems()
		{
			for (int i = 0; i < _scrollRect.content.childCount; i++)
			{
				items.Add(_scrollRect.content.GetChild(i).GetComponent<RectTransform>());
			}
			_itemCount = _scrollRect.content.childCount;
		}

		public void Init()
		{
			if (GetComponent<ScrollRect>() != null)
			{
				_scrollRect = GetComponent<ScrollRect>();
				_scrollRect.onValueChanged.AddListener(OnScroll);
				_scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
				if (_scrollRect.content.GetComponent<VerticalLayoutGroup>() != null)
				{
					_verticalLayoutGroup = _scrollRect.content.GetComponent<VerticalLayoutGroup>();
				}
				if (_scrollRect.content.GetComponent<HorizontalLayoutGroup>() != null)
				{
					_horizontalLayoutGroup = _scrollRect.content.GetComponent<HorizontalLayoutGroup>();
				}
				if (_scrollRect.content.GetComponent<GridLayoutGroup>() != null)
				{
					_gridLayoutGroup = _scrollRect.content.GetComponent<GridLayoutGroup>();
				}
				if (_scrollRect.content.GetComponent<ContentSizeFitter>() != null)
				{
					_contentSizeFitter = _scrollRect.content.GetComponent<ContentSizeFitter>();
				}
				_isHorizontal = _scrollRect.horizontal;
				_isVertical = _scrollRect.vertical;
				if (_isHorizontal && _isVertical)
				{
					Debug.LogError("UI_InfiniteScroll doesn't support scrolling in both directions, please choose one direction (horizontal or vertical)");
				}
				SetItems();
			}
			else
			{
				Debug.LogError("UI_InfiniteScroll => No ScrollRect component found");
			}
		}

		private void DisableGridComponents()
		{
			if (_isVertical)
			{
				_recordOffsetY = items[1].GetComponent<RectTransform>().anchoredPosition.y - items[0].GetComponent<RectTransform>().anchoredPosition.y;
				if (_recordOffsetY < 0f)
				{
					_recordOffsetY *= -1f;
				}
				_disableMarginY = _recordOffsetY * (float)_itemCount / 2f;
			}
			if (_isHorizontal)
			{
				_recordOffsetX = items[1].GetComponent<RectTransform>().anchoredPosition.x - items[0].GetComponent<RectTransform>().anchoredPosition.x;
				if (_recordOffsetX < 0f)
				{
					_recordOffsetX *= -1f;
				}
				_disableMarginX = _recordOffsetX * (float)_itemCount / 2f;
			}
			if ((bool)_verticalLayoutGroup)
			{
				_verticalLayoutGroup.enabled = false;
			}
			if ((bool)_horizontalLayoutGroup)
			{
				_horizontalLayoutGroup.enabled = false;
			}
			if ((bool)_contentSizeFitter)
			{
				_contentSizeFitter.enabled = false;
			}
			if ((bool)_gridLayoutGroup)
			{
				_gridLayoutGroup.enabled = false;
			}
			_hasDisabledGridComponents = true;
		}

		public void OnScroll(Vector2 pos)
		{
			if (!_hasDisabledGridComponents)
			{
				DisableGridComponents();
			}
			for (int i = 0; i < items.Count; i++)
			{
				if (_isHorizontal)
				{
					if (_scrollRect.transform.InverseTransformPoint(items[i].gameObject.transform.position).x > _disableMarginX + _threshold)
					{
						_newAnchoredPosition = items[i].anchoredPosition;
						_newAnchoredPosition.x -= (float)_itemCount * _recordOffsetX;
						items[i].anchoredPosition = _newAnchoredPosition;
						_scrollRect.content.GetChild(_itemCount - 1).transform.SetAsFirstSibling();
					}
					else if (_scrollRect.transform.InverseTransformPoint(items[i].gameObject.transform.position).x < 0f - _disableMarginX)
					{
						_newAnchoredPosition = items[i].anchoredPosition;
						_newAnchoredPosition.x += (float)_itemCount * _recordOffsetX;
						items[i].anchoredPosition = _newAnchoredPosition;
						_scrollRect.content.GetChild(0).transform.SetAsLastSibling();
					}
				}
				if (_isVertical)
				{
					if (_scrollRect.transform.InverseTransformPoint(items[i].gameObject.transform.position).y > _disableMarginY + _threshold)
					{
						_newAnchoredPosition = items[i].anchoredPosition;
						_newAnchoredPosition.y -= (float)_itemCount * _recordOffsetY;
						items[i].anchoredPosition = _newAnchoredPosition;
						_scrollRect.content.GetChild(_itemCount - 1).transform.SetAsFirstSibling();
					}
					else if (_scrollRect.transform.InverseTransformPoint(items[i].gameObject.transform.position).y < 0f - _disableMarginY)
					{
						_newAnchoredPosition = items[i].anchoredPosition;
						_newAnchoredPosition.y += (float)_itemCount * _recordOffsetY;
						items[i].anchoredPosition = _newAnchoredPosition;
						_scrollRect.content.GetChild(0).transform.SetAsLastSibling();
					}
				}
			}
		}
	}
}

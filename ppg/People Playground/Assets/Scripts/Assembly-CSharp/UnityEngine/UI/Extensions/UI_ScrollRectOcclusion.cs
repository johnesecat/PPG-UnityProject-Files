using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/UI Scrollrect Occlusion")]
	public class UI_ScrollRectOcclusion : MonoBehaviour
	{
		public bool InitByUser;

		private bool _initialised;

		private ScrollRect _scrollRect;

		private ContentSizeFitter _contentSizeFitter;

		private VerticalLayoutGroup _verticalLayoutGroup;

		private HorizontalLayoutGroup _horizontalLayoutGroup;

		private GridLayoutGroup _gridLayoutGroup;

		private bool _isVertical;

		private bool _isHorizontal;

		private float _disableMarginX;

		private float _disableMarginY;

		private bool _hasDisabledGridComponents;

		private List<RectTransform> _items = new List<RectTransform>();

		private bool _reset;

		private void Awake()
		{
			if (!InitByUser)
			{
				Init();
			}
		}

		public void Init()
		{
			if (_initialised)
			{
				Debug.LogError("Control already initialized\nYou have to enable the InitByUser setting on the control in order to use Init() when running");
			}
			else if (GetComponent<ScrollRect>() != null)
			{
				_initialised = true;
				_scrollRect = GetComponent<ScrollRect>();
				_scrollRect.onValueChanged.AddListener(OnScroll);
				_isHorizontal = _scrollRect.horizontal;
				_isVertical = _scrollRect.vertical;
				for (int i = 0; i < _scrollRect.content.childCount; i++)
				{
					_items.Add(_scrollRect.content.GetChild(i).GetComponent<RectTransform>());
				}
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
			}
			else
			{
				Debug.LogError("UI_ScrollRectOcclusion => No ScrollRect component found");
			}
		}

		private void ToggleGridComponents(bool toggle)
		{
			if (_isVertical)
			{
				_disableMarginY = _scrollRect.GetComponent<RectTransform>().rect.height / 2f + _items[0].sizeDelta.y;
			}
			if (_isHorizontal)
			{
				_disableMarginX = _scrollRect.GetComponent<RectTransform>().rect.width / 2f + _items[0].sizeDelta.x;
			}
			if ((bool)_verticalLayoutGroup)
			{
				_verticalLayoutGroup.enabled = toggle;
			}
			if ((bool)_horizontalLayoutGroup)
			{
				_horizontalLayoutGroup.enabled = toggle;
			}
			if ((bool)_contentSizeFitter)
			{
				_contentSizeFitter.enabled = toggle;
			}
			if ((bool)_gridLayoutGroup)
			{
				_gridLayoutGroup.enabled = toggle;
			}
			_hasDisabledGridComponents = !toggle;
		}

		public void OnScroll(Vector2 pos)
		{
			if (_reset)
			{
				return;
			}
			if (!_hasDisabledGridComponents)
			{
				ToggleGridComponents(toggle: false);
			}
			for (int i = 0; i < _items.Count; i++)
			{
				if (_isVertical && _isHorizontal)
				{
					if (_scrollRect.transform.InverseTransformPoint(_items[i].position).y < 0f - _disableMarginY || _scrollRect.transform.InverseTransformPoint(_items[i].position).y > _disableMarginY || _scrollRect.transform.InverseTransformPoint(_items[i].position).x < 0f - _disableMarginX || _scrollRect.transform.InverseTransformPoint(_items[i].position).x > _disableMarginX)
					{
						_items[i].gameObject.SetActive(value: false);
					}
					else
					{
						_items[i].gameObject.SetActive(value: true);
					}
					continue;
				}
				if (_isVertical)
				{
					if (_scrollRect.transform.InverseTransformPoint(_items[i].position).y < 0f - _disableMarginY || _scrollRect.transform.InverseTransformPoint(_items[i].position).y > _disableMarginY)
					{
						_items[i].gameObject.SetActive(value: false);
					}
					else
					{
						_items[i].gameObject.SetActive(value: true);
					}
				}
				if (_isHorizontal)
				{
					if (_scrollRect.transform.InverseTransformPoint(_items[i].position).x < 0f - _disableMarginX || _scrollRect.transform.InverseTransformPoint(_items[i].position).x > _disableMarginX)
					{
						_items[i].gameObject.SetActive(value: false);
					}
					else
					{
						_items[i].gameObject.SetActive(value: true);
					}
				}
			}
		}

		public void SetDirty()
		{
			_reset = true;
		}

		private void LateUpdate()
		{
			if (_reset)
			{
				_reset = false;
				_items.Clear();
				for (int i = 0; i < _scrollRect.content.childCount; i++)
				{
					_items.Add(_scrollRect.content.GetChild(i).GetComponent<RectTransform>());
					_items[i].gameObject.SetActive(value: true);
				}
				ToggleGridComponents(toggle: true);
			}
		}
	}
}

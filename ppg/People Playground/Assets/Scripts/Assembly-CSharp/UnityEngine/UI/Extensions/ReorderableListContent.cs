using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[DisallowMultipleComponent]
	public class ReorderableListContent : MonoBehaviour
	{
		private List<Transform> _cachedChildren;

		private List<ReorderableListElement> _cachedListElement;

		private ReorderableListElement _ele;

		private ReorderableList _extList;

		private RectTransform _rect;

		private bool _started;

		private void OnEnable()
		{
			if ((bool)_rect)
			{
				StartCoroutine(RefreshChildren());
			}
		}

		public void OnTransformChildrenChanged()
		{
			if (base.isActiveAndEnabled)
			{
				StartCoroutine(RefreshChildren());
			}
		}

		public void Init(ReorderableList extList)
		{
			if (_started)
			{
				StopCoroutine(RefreshChildren());
			}
			_extList = extList;
			_rect = GetComponent<RectTransform>();
			_cachedChildren = new List<Transform>();
			_cachedListElement = new List<ReorderableListElement>();
			StartCoroutine(RefreshChildren());
			_started = true;
		}

		private IEnumerator RefreshChildren()
		{
			for (int i = 0; i < _rect.childCount; i++)
			{
				if (!_cachedChildren.Contains(_rect.GetChild(i)))
				{
					_ele = _rect.GetChild(i).gameObject.GetComponent<ReorderableListElement>() ?? _rect.GetChild(i).gameObject.AddComponent<ReorderableListElement>();
					_ele.Init(_extList);
					_cachedChildren.Add(_rect.GetChild(i));
					_cachedListElement.Add(_ele);
				}
			}
			yield return 0;
			for (int num = _cachedChildren.Count - 1; num >= 0; num--)
			{
				if (_cachedChildren[num] == null)
				{
					_cachedChildren.RemoveAt(num);
					_cachedListElement.RemoveAt(num);
				}
			}
		}
	}
}

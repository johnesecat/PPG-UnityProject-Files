using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(ScrollRect))]
	[AddComponentMenu("UI/Extensions/ContentSnapScrollHorizontal")]
	public class ContentScrollSnapHorizontal : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IEndDragHandler
	{
		[Serializable]
		public class StartMovementEvent : UnityEvent
		{
		}

		[Serializable]
		public class CurrentItemChangeEvent : UnityEvent<int>
		{
		}

		[Serializable]
		public class FoundItemToSnapToEvent : UnityEvent<int>
		{
		}

		[Serializable]
		public class SnappedToItemEvent : UnityEvent<int>
		{
		}

		[Serializable]
		public struct MoveInfo
		{
			public enum IndexType
			{
				childIndex,
				positionIndex
			}

			[Tooltip("Child Index means the Index corresponds to the content item at that index in the hierarchy.\nPosition Index means the Index corresponds to the content item in that snap position.\nA higher Position Index in a Horizontal Scroll Snap means it would be further to the right.")]
			public IndexType indexType;

			[Tooltip("Zero based")]
			public int index;

			[Tooltip("If this is true the snap scroll will jump to the index, otherwise it will lerp there.")]
			public bool jump;

			[Tooltip("If jump is false this is the time it will take to lerp to the index")]
			public float duration;

			public MoveInfo(IndexType _indexType, int _index)
			{
				indexType = _indexType;
				index = _index;
				jump = true;
				duration = 0f;
			}

			public MoveInfo(IndexType _indexType, int _index, bool _jump, float _duration)
			{
				indexType = _indexType;
				index = _index;
				jump = _jump;
				duration = _duration;
			}
		}

		public bool ignoreInactiveItems = true;

		public MoveInfo startInfo = new MoveInfo(MoveInfo.IndexType.positionIndex, 0);

		public GameObject prevButton;

		public GameObject nextButton;

		public GameObject pagination;

		[Tooltip("The velocity below which the scroll rect will start to snap")]
		public int snappingVelocityThreshold = 50;

		[Header("Paging Info")]
		[Tooltip("Should the pagination & buttons jump or lerp to the items")]
		public bool jumpToItem;

		[Tooltip("The time it will take for the pagination or buttons to move between items")]
		public float lerpTime = 0.3f;

		[Header("Events")]
		[SerializeField]
		[Tooltip("Event is triggered whenever the scroll rect starts to move, even when triggered programmatically")]
		private StartMovementEvent m_StartMovementEvent = new StartMovementEvent();

		[SerializeField]
		[Tooltip("Event is triggered whenever the closest item to the center of the scrollrect changes")]
		private CurrentItemChangeEvent m_CurrentItemChangeEvent = new CurrentItemChangeEvent();

		[SerializeField]
		[Tooltip("Event is triggered when the ContentSnapScroll decides which item it is going to snap to. Returns the index of the closest position.")]
		private FoundItemToSnapToEvent m_FoundItemToSnapToEvent = new FoundItemToSnapToEvent();

		[SerializeField]
		[Tooltip("Event is triggered when we finally settle on an element. Returns the index of the item's position.")]
		private SnappedToItemEvent m_SnappedToItemEvent = new SnappedToItemEvent();

		private ScrollRect scrollRect;

		private RectTransform scrollRectTransform;

		private RectTransform contentTransform;

		private List<Vector3> contentPositions = new List<Vector3>();

		private Vector3 lerpTarget = Vector3.zero;

		private float totalScrollableWidth;

		private DrivenRectTransformTracker tracker;

		private float mLerpTime;

		private int _closestItem;

		private bool mSliding;

		private bool mLerping;

		public StartMovementEvent MovementStarted
		{
			get
			{
				return m_StartMovementEvent;
			}
			set
			{
				m_StartMovementEvent = value;
			}
		}

		public CurrentItemChangeEvent CurrentItemChanged
		{
			get
			{
				return m_CurrentItemChangeEvent;
			}
			set
			{
				m_CurrentItemChangeEvent = value;
			}
		}

		public FoundItemToSnapToEvent ItemFoundToSnap
		{
			get
			{
				return m_FoundItemToSnapToEvent;
			}
			set
			{
				m_FoundItemToSnapToEvent = value;
			}
		}

		public SnappedToItemEvent ItemSnappedTo
		{
			get
			{
				return m_SnappedToItemEvent;
			}
			set
			{
				m_SnappedToItemEvent = value;
			}
		}

		private bool ContentIsHorizonalLayoutGroup => contentTransform.GetComponent<HorizontalLayoutGroup>() != null;

		public bool Moving
		{
			get
			{
				if (!Sliding)
				{
					return Lerping;
				}
				return true;
			}
		}

		public bool Sliding => mSliding;

		public bool Lerping => mLerping;

		public int ClosestItemIndex => contentPositions.IndexOf(FindClosestFrom(contentTransform.localPosition));

		public int LerpTargetIndex => contentPositions.IndexOf(lerpTarget);

		private bool IsScrollRectAvailable
		{
			get
			{
				if ((bool)scrollRect && (bool)contentTransform && contentTransform.childCount > 0)
				{
					return true;
				}
				return false;
			}
		}

		private void Awake()
		{
			scrollRect = GetComponent<ScrollRect>();
			scrollRectTransform = (RectTransform)scrollRect.transform;
			contentTransform = scrollRect.content;
			if ((bool)nextButton)
			{
				nextButton.GetComponent<Button>().onClick.AddListener(delegate
				{
					NextItem();
				});
			}
			if ((bool)prevButton)
			{
				prevButton.GetComponent<Button>().onClick.AddListener(delegate
				{
					PreviousItem();
				});
			}
			if (IsScrollRectAvailable)
			{
				SetupDrivenTransforms();
				SetupSnapScroll();
				scrollRect.horizontalNormalizedPosition = 0f;
				_closestItem = 0;
				GoTo(startInfo);
			}
		}

		public void SetNewItems(ref List<Transform> newItems)
		{
			if (!scrollRect || !contentTransform)
			{
				return;
			}
			for (int num = scrollRect.content.childCount - 1; num >= 0; num--)
			{
				Transform child = contentTransform.GetChild(num);
				child.SetParent(null);
				Object.DestroyImmediate(child.gameObject);
			}
			foreach (Transform newItem in newItems)
			{
				GameObject gameObject = newItem.gameObject;
				if (gameObject.IsPrefab())
				{
					gameObject = Object.Instantiate(newItem.gameObject, contentTransform);
				}
				else
				{
					gameObject.transform.SetParent(contentTransform);
				}
			}
			SetupDrivenTransforms();
			SetupSnapScroll();
			scrollRect.horizontalNormalizedPosition = 0f;
			_closestItem = 0;
			GoTo(startInfo);
		}

		private void OnDisable()
		{
			tracker.Clear();
		}

		private void SetupDrivenTransforms()
		{
			tracker = default(DrivenRectTransformTracker);
			tracker.Clear();
			foreach (RectTransform item in contentTransform)
			{
				tracker.Add(this, item, DrivenTransformProperties.Anchors);
				item.anchorMax = new Vector2(0f, 1f);
				item.anchorMin = new Vector2(0f, 1f);
			}
		}

		private void SetupSnapScroll()
		{
			if (ContentIsHorizonalLayoutGroup)
			{
				SetupWithHorizontalLayoutGroup();
			}
			else
			{
				SetupWithCalculatedSpacing();
			}
		}

		private void SetupWithHorizontalLayoutGroup()
		{
			HorizontalLayoutGroup component = contentTransform.GetComponent<HorizontalLayoutGroup>();
			float num = 0f;
			int num2 = 0;
			for (int i = 0; i < contentTransform.childCount; i++)
			{
				if (!ignoreInactiveItems || contentTransform.GetChild(i).gameObject.activeInHierarchy)
				{
					num += ((RectTransform)contentTransform.GetChild(i)).sizeDelta.x;
					num2++;
				}
			}
			float num3 = (float)(num2 - 1) * component.spacing;
			float num4 = num + num3 + (float)component.padding.left + (float)component.padding.right;
			contentTransform.sizeDelta = new Vector2(num4, contentTransform.sizeDelta.y);
			float x = Mathf.Min(((RectTransform)contentTransform.GetChild(0)).sizeDelta.x, ((RectTransform)contentTransform.GetChild(contentTransform.childCount - 1)).sizeDelta.x);
			scrollRectTransform.sizeDelta = new Vector2(x, scrollRectTransform.sizeDelta.y);
			contentPositions = new List<Vector3>();
			float x2 = scrollRectTransform.sizeDelta.x;
			totalScrollableWidth = num4 - x2;
			float num5 = component.padding.left;
			int num6 = 0;
			for (int j = 0; j < contentTransform.childCount; j++)
			{
				if (!ignoreInactiveItems || contentTransform.GetChild(j).gameObject.activeInHierarchy)
				{
					float x3 = ((RectTransform)contentTransform.GetChild(j)).sizeDelta.x;
					float num7 = num5 + component.spacing * (float)num6 + (x3 - x2) / 2f;
					scrollRect.horizontalNormalizedPosition = num7 / totalScrollableWidth;
					contentPositions.Add(contentTransform.localPosition);
					num5 += x3;
					num6++;
				}
			}
		}

		private void SetupWithCalculatedSpacing()
		{
			List<RectTransform> list = new List<RectTransform>();
			for (int i = 0; i < contentTransform.childCount; i++)
			{
				if (ignoreInactiveItems && !contentTransform.GetChild(i).gameObject.activeInHierarchy)
				{
					continue;
				}
				RectTransform rectTransform = (RectTransform)contentTransform.GetChild(i);
				int index = list.Count;
				for (int j = 0; j < list.Count; j++)
				{
					if (DstFromTopLeftOfTransformToTopLeftOfParent(rectTransform).x < DstFromTopLeftOfTransformToTopLeftOfParent(list[j]).x)
					{
						index = j;
						break;
					}
				}
				list.Insert(index, rectTransform);
			}
			RectTransform rectTransform2 = list[list.Count - 1];
			float num = DstFromTopLeftOfTransformToTopLeftOfParent(rectTransform2).x + rectTransform2.sizeDelta.x;
			contentTransform.sizeDelta = new Vector2(num, contentTransform.sizeDelta.y);
			float x = Mathf.Min(list[0].sizeDelta.x, list[list.Count - 1].sizeDelta.x);
			scrollRectTransform.sizeDelta = new Vector2(x, scrollRectTransform.sizeDelta.y);
			contentPositions = new List<Vector3>();
			float x2 = scrollRectTransform.sizeDelta.x;
			totalScrollableWidth = num - x2;
			for (int k = 0; k < list.Count; k++)
			{
				float num2 = DstFromTopLeftOfTransformToTopLeftOfParent(list[k]).x + (list[k].sizeDelta.x - x2) / 2f;
				scrollRect.horizontalNormalizedPosition = num2 / totalScrollableWidth;
				contentPositions.Add(contentTransform.localPosition);
			}
		}

		public void GoTo(MoveInfo info)
		{
			if (!Moving && info.index != ClosestItemIndex)
			{
				MovementStarted.Invoke();
			}
			if (info.indexType == MoveInfo.IndexType.childIndex)
			{
				mLerpTime = info.duration;
				GoToChild(info.index, info.jump);
			}
			else if (info.indexType == MoveInfo.IndexType.positionIndex)
			{
				mLerpTime = info.duration;
				GoToContentPos(info.index, info.jump);
			}
		}

		private void GoToChild(int index, bool jump)
		{
			int num = Mathf.Clamp(index, 0, contentPositions.Count - 1);
			if (ContentIsHorizonalLayoutGroup)
			{
				lerpTarget = contentPositions[num];
				if (jump)
				{
					contentTransform.localPosition = lerpTarget;
					return;
				}
				StopMovement();
				StartCoroutine("LerpToContent");
				return;
			}
			int num2 = 0;
			Vector3 localPosition = contentTransform.localPosition;
			for (int i = 0; i < contentTransform.childCount; i++)
			{
				if (ignoreInactiveItems && !contentTransform.GetChild(i).gameObject.activeInHierarchy)
				{
					continue;
				}
				if (num2 == num)
				{
					RectTransform rectTransform = (RectTransform)contentTransform.GetChild(i);
					float num3 = DstFromTopLeftOfTransformToTopLeftOfParent(rectTransform).x + (rectTransform.sizeDelta.x - scrollRectTransform.sizeDelta.x) / 2f;
					scrollRect.horizontalNormalizedPosition = num3 / totalScrollableWidth;
					lerpTarget = contentTransform.localPosition;
					if (!jump)
					{
						contentTransform.localPosition = localPosition;
						StopMovement();
						StartCoroutine("LerpToContent");
					}
					break;
				}
				num2++;
			}
		}

		private void GoToContentPos(int index, bool jump)
		{
			int index2 = Mathf.Clamp(index, 0, contentPositions.Count - 1);
			lerpTarget = contentPositions[index2];
			if (jump)
			{
				contentTransform.localPosition = lerpTarget;
				return;
			}
			StopMovement();
			StartCoroutine("LerpToContent");
		}

		public void NextItem()
		{
			int index = ((!Sliding) ? (LerpTargetIndex + 1) : (ClosestItemIndex + 1));
			MoveInfo info = new MoveInfo(MoveInfo.IndexType.positionIndex, index, jumpToItem, lerpTime);
			GoTo(info);
		}

		public void PreviousItem()
		{
			int index = ((!Sliding) ? (LerpTargetIndex - 1) : (ClosestItemIndex - 1));
			MoveInfo info = new MoveInfo(MoveInfo.IndexType.positionIndex, index, jumpToItem, lerpTime);
			GoTo(info);
		}

		public void UpdateLayout()
		{
			SetupDrivenTransforms();
			SetupSnapScroll();
		}

		public void UpdateLayoutAndMoveTo(MoveInfo info)
		{
			SetupDrivenTransforms();
			SetupSnapScroll();
			GoTo(info);
		}

		public void OnBeginDrag(PointerEventData ped)
		{
			if (contentPositions.Count >= 2)
			{
				StopMovement();
				if (!Moving)
				{
					MovementStarted.Invoke();
				}
			}
		}

		public void OnEndDrag(PointerEventData ped)
		{
			if (contentPositions.Count > 1 && IsScrollRectAvailable)
			{
				StartCoroutine("SlideAndLerp");
			}
		}

		private void Update()
		{
			if (IsScrollRectAvailable && _closestItem != ClosestItemIndex)
			{
				CurrentItemChanged.Invoke(ClosestItemIndex);
				ChangePaginationInfo(ClosestItemIndex);
				_closestItem = ClosestItemIndex;
			}
		}

		private IEnumerator SlideAndLerp()
		{
			mSliding = true;
			while (Mathf.Abs(scrollRect.velocity.x) > (float)snappingVelocityThreshold)
			{
				yield return null;
			}
			lerpTarget = FindClosestFrom(contentTransform.localPosition);
			ItemFoundToSnap.Invoke(LerpTargetIndex);
			while (Vector3.Distance(contentTransform.localPosition, lerpTarget) > 1f)
			{
				contentTransform.localPosition = Vector3.Lerp(scrollRect.content.localPosition, lerpTarget, 7.5f * Time.deltaTime);
				yield return null;
			}
			mSliding = false;
			scrollRect.velocity = Vector2.zero;
			contentTransform.localPosition = lerpTarget;
			ItemSnappedTo.Invoke(LerpTargetIndex);
		}

		private IEnumerator LerpToContent()
		{
			ItemFoundToSnap.Invoke(LerpTargetIndex);
			mLerping = true;
			Vector3 originalContentPos = contentTransform.localPosition;
			float elapsedTime = 0f;
			while (elapsedTime < mLerpTime)
			{
				elapsedTime += Time.deltaTime;
				contentTransform.localPosition = Vector3.Lerp(originalContentPos, lerpTarget, elapsedTime / mLerpTime);
				yield return null;
			}
			ItemSnappedTo.Invoke(LerpTargetIndex);
			mLerping = false;
		}

		private void StopMovement()
		{
			scrollRect.velocity = Vector2.zero;
			StopCoroutine("SlideAndLerp");
			StopCoroutine("LerpToContent");
		}

		private void ChangePaginationInfo(int targetScreen)
		{
			if ((bool)pagination)
			{
				for (int i = 0; i < pagination.transform.childCount; i++)
				{
					pagination.transform.GetChild(i).GetComponent<Toggle>().isOn = targetScreen == i;
				}
			}
		}

		private Vector2 DstFromTopLeftOfTransformToTopLeftOfParent(RectTransform rt)
		{
			return new Vector2(rt.anchoredPosition.x - rt.sizeDelta.x * rt.pivot.x, rt.anchoredPosition.y + rt.sizeDelta.y * (1f - rt.pivot.y));
		}

		private Vector3 FindClosestFrom(Vector3 start)
		{
			Vector3 result = Vector3.zero;
			float num = float.PositiveInfinity;
			foreach (Vector3 contentPosition in contentPositions)
			{
				if (Vector3.Distance(start, contentPosition) < num)
				{
					num = Vector3.Distance(start, contentPosition);
					result = contentPosition;
				}
			}
			return result;
		}
	}
}

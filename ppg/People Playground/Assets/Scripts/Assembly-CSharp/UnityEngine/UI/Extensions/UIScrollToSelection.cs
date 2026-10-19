using System;
using System.Collections;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(ScrollRect))]
	[AddComponentMenu("UI/Extensions/UIScrollToSelection")]
	public class UIScrollToSelection : MonoBehaviour
	{
		[Flags]
		public enum Axis
		{
			NONE = 0x0,
			HORIZONTAL = 0x1,
			VERTICAL = 0x10,
			ANY = 0x11
		}

		[Flags]
		public enum MouseButton
		{
			NONE = 0x0,
			LEFT = 0x1,
			RIGHT = 0x10,
			MIDDLE = 0x100,
			ANY = 0x111
		}

		public enum ScrollMethod
		{
			MOVE_TOWARDS,
			LERP
		}

		[Header("[ References ]")]
		[SerializeField]
		[Tooltip("View (boundaries/mask) rect transform. Used to check if automatic scroll to selection is required.")]
		private RectTransform viewportRectTransform;

		[SerializeField]
		[Tooltip("Scroll rect used to reach selected element.")]
		private ScrollRect targetScrollRect;

		[Header("[ Scrolling ]")]
		[SerializeField]
		[Tooltip("Allow automatic scrolling only on these axes.")]
		private Axis scrollAxes = Axis.ANY;

		[SerializeField]
		[Tooltip("MOVE_TOWARDS: stiff movement, LERP: smoothed out movement")]
		private ScrollMethod usedScrollMethod;

		[SerializeField]
		private float scrollSpeed = 50f;

		[Space(5f)]
		[SerializeField]
		[Tooltip("Scroll speed used when element to select is out of \"JumpOffsetThreshold\" range")]
		private float endOfListJumpScrollSpeed = 150f;

		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("If next element to scroll to is located over this screen percentage, use \"EndOfListJumpScrollSpeed\" to reach this element faster.")]
		private float jumpOffsetThreshold = 1f;

		[Header("[ Input ]")]
		[SerializeField]
		private MouseButton cancelScrollMouseButtons = MouseButton.ANY;

		[SerializeField]
		private KeyCode[] cancelScrollKeys = new KeyCode[0];

		private Vector3[] viewRectCorners = new Vector3[4];

		private Vector3[] selectedElementCorners = new Vector3[4];

		private RectTransform scrollRectContentTransform;

		private GameObject lastCheckedSelection;

		private Coroutine animationCoroutine;

		public RectTransform ViewRectTransform
		{
			get
			{
				return viewportRectTransform;
			}
			set
			{
				viewportRectTransform = value;
			}
		}

		public ScrollRect TargetScrollRect
		{
			get
			{
				return targetScrollRect;
			}
			set
			{
				targetScrollRect = value;
			}
		}

		public Axis ScrollAxes => scrollAxes;

		public ScrollMethod UsedScrollMethod => usedScrollMethod;

		public float ScrollSpeed => scrollSpeed;

		public float EndOfListJumpScrollSpeed => endOfListJumpScrollSpeed;

		public float JumpOffsetThreshold => jumpOffsetThreshold;

		public MouseButton CancelScrollMouseButtons => cancelScrollMouseButtons;

		public KeyCode[] CancelScrollKeys => cancelScrollKeys;

		protected void Awake()
		{
			ValidateReferences();
		}

		protected void LateUpdate()
		{
			TryToScrollToSelection();
		}

		protected void Reset()
		{
			TargetScrollRect = base.gameObject.GetComponentInParent<ScrollRect>() ?? base.gameObject.GetComponentInChildren<ScrollRect>();
			ViewRectTransform = base.gameObject.GetComponent<RectTransform>();
		}

		private void ValidateReferences()
		{
			if (!targetScrollRect)
			{
				targetScrollRect = GetComponent<ScrollRect>();
			}
			if (!targetScrollRect)
			{
				Debug.LogError("[UIScrollToSelection] No ScrollRect found. Either attach this script to a ScrollRect or assign on in the 'Target Scroll Rect' property");
				base.gameObject.SetActive(value: false);
				return;
			}
			if (ViewRectTransform == null)
			{
				ViewRectTransform = TargetScrollRect.GetComponent<RectTransform>();
			}
			if (TargetScrollRect != null)
			{
				scrollRectContentTransform = TargetScrollRect.content;
			}
			if (EventSystem.current == null)
			{
				Debug.LogError("[UIScrollToSelection] Unity UI EventSystem not found. It is required to check current selected object.");
				base.gameObject.SetActive(value: false);
			}
		}

		private void TryToScrollToSelection()
		{
			GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
			if (!(currentSelectedGameObject == null) && currentSelectedGameObject.activeInHierarchy && !(currentSelectedGameObject == lastCheckedSelection) && currentSelectedGameObject.transform.IsChildOf(base.transform))
			{
				RectTransform component = currentSelectedGameObject.GetComponent<RectTransform>();
				ViewRectTransform.GetWorldCorners(viewRectCorners);
				component.GetWorldCorners(selectedElementCorners);
				ScrollToSelection(currentSelectedGameObject);
				lastCheckedSelection = currentSelectedGameObject;
			}
		}

		private void ScrollToSelection(GameObject selection)
		{
			if (!(selection == null))
			{
				Vector3[] array = viewRectCorners;
				Vector3[] array2 = selectedElementCorners;
				Vector2 zero = Vector2.zero;
				zero.x = ((array2[0].x < array[0].x) ? (array2[0].x - array[0].x) : 0f) + ((array2[2].x > array[2].x) ? (array2[2].x - array[2].x) : 0f);
				zero.y = ((array2[0].y < array[0].y) ? (array2[0].y - array[0].y) : 0f) + ((array2[1].y > array[1].y) ? (array2[1].y - array[1].y) : 0f);
				float speed = ScrollSpeed;
				if (Math.Abs(zero.x) / (float)Screen.width >= JumpOffsetThreshold || Math.Abs(zero.y) / (float)Screen.height >= JumpOffsetThreshold)
				{
					speed = EndOfListJumpScrollSpeed;
				}
				Vector2 targetPosition = (Vector2)scrollRectContentTransform.localPosition - zero;
				if (animationCoroutine != null)
				{
					StopCoroutine(animationCoroutine);
				}
				animationCoroutine = StartCoroutine(ScrollToPosition(targetPosition, speed));
			}
		}

		private IEnumerator ScrollToPosition(Vector2 targetPosition, float speed)
		{
			Vector3 localPosition = scrollRectContentTransform.localPosition;
			targetPosition.x = (((ScrollAxes | Axis.HORIZONTAL) == ScrollAxes) ? targetPosition.x : localPosition.x);
			targetPosition.y = (((ScrollAxes | Axis.VERTICAL) == ScrollAxes) ? targetPosition.y : localPosition.y);
			Vector2 currentPosition2D = localPosition;
			float horizontalSpeed = (float)Screen.width / Screen.dpi * speed;
			float verticalSpeed = (float)Screen.height / Screen.dpi * speed;
			while (currentPosition2D != targetPosition && !CheckIfScrollInterrupted())
			{
				currentPosition2D.x = MoveTowardsValue(currentPosition2D.x, targetPosition.x, horizontalSpeed, UsedScrollMethod);
				currentPosition2D.y = MoveTowardsValue(currentPosition2D.y, targetPosition.y, verticalSpeed, UsedScrollMethod);
				scrollRectContentTransform.localPosition = currentPosition2D;
				yield return null;
			}
			scrollRectContentTransform.localPosition = currentPosition2D;
		}

		private bool CheckIfScrollInterrupted()
		{
			bool flag = false;
			switch (CancelScrollMouseButtons)
			{
			case MouseButton.LEFT:
				flag |= Input.GetMouseButtonDown(0);
				break;
			case MouseButton.RIGHT:
				flag |= Input.GetMouseButtonDown(1);
				break;
			case MouseButton.MIDDLE:
				flag |= Input.GetMouseButtonDown(2);
				break;
			}
			if (flag)
			{
				return true;
			}
			for (int i = 0; i < CancelScrollKeys.Length; i++)
			{
				if (Input.GetKeyDown(CancelScrollKeys[i]))
				{
					return true;
				}
			}
			return false;
		}

		private float MoveTowardsValue(float from, float to, float delta, ScrollMethod method)
		{
			switch (method)
			{
			case ScrollMethod.MOVE_TOWARDS:
				return Mathf.MoveTowards(from, to, delta * Time.unscaledDeltaTime);
			case ScrollMethod.LERP:
				return Mathf.Lerp(from, to, delta * Time.unscaledDeltaTime);
			default:
				return from;
			}
		}
	}
}

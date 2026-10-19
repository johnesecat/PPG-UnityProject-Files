using System.Collections;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("UI/Extensions/Tooltip/Tooltip Trigger")]
	public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
	{
		public enum TooltipPositioningType
		{
			mousePosition,
			mousePositionAndFollow,
			transformPosition
		}

		[TextArea]
		public string text;

		[Tooltip("Defines where the tooltip will be placed and how that placement will occur. Transform position will always be used if this element wasn't selected via mouse")]
		public TooltipPositioningType tooltipPositioningType;

		private bool isChildOfOverlayCanvas;

		private bool hovered;

		public Vector3 offset;

		public bool WorldToScreenIsRequired
		{
			get
			{
				if (!isChildOfOverlayCanvas || ToolTip.Instance.guiMode != RenderMode.ScreenSpaceCamera)
				{
					if (!isChildOfOverlayCanvas)
					{
						return ToolTip.Instance.guiMode == RenderMode.ScreenSpaceOverlay;
					}
					return false;
				}
				return true;
			}
		}

		private void Start()
		{
			Canvas componentInParent = GetComponentInParent<Canvas>();
			if ((bool)componentInParent && componentInParent.renderMode == RenderMode.ScreenSpaceOverlay)
			{
				isChildOfOverlayCanvas = true;
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			switch (tooltipPositioningType)
			{
			case TooltipPositioningType.mousePosition:
				StartHover(UIExtensionsInputManager.MousePosition + offset, shouldCanvasUpdate: true);
				break;
			case TooltipPositioningType.mousePositionAndFollow:
				StartHover(UIExtensionsInputManager.MousePosition + offset, shouldCanvasUpdate: true);
				hovered = true;
				StartCoroutine(HoveredMouseFollowingLoop());
				break;
			case TooltipPositioningType.transformPosition:
				StartHover((WorldToScreenIsRequired ? ToolTip.Instance.GuiCamera.WorldToScreenPoint(base.transform.position) : base.transform.position) + offset, shouldCanvasUpdate: true);
				break;
			}
		}

		private IEnumerator HoveredMouseFollowingLoop()
		{
			while (hovered)
			{
				StartHover(UIExtensionsInputManager.MousePosition + offset);
				yield return null;
			}
		}

		public void OnSelect(BaseEventData eventData)
		{
			StartHover((WorldToScreenIsRequired ? ToolTip.Instance.GuiCamera.WorldToScreenPoint(base.transform.position) : base.transform.position) + offset, shouldCanvasUpdate: true);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			StopHover();
		}

		public void OnDeselect(BaseEventData eventData)
		{
			StopHover();
		}

		private void StartHover(Vector3 position, bool shouldCanvasUpdate = false)
		{
			ToolTip.Instance.SetTooltip(text, position, shouldCanvasUpdate);
		}

		private void StopHover()
		{
			hovered = false;
			ToolTip.Instance.HideTooltip();
		}
	}
}

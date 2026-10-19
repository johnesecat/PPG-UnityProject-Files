namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("UI/Extensions/Tooltip/Tooltip")]
	public class ToolTip : MonoBehaviour
	{
		private Text _text;

		private RectTransform _rectTransform;

		private RectTransform canvasRectTransform;

		[Tooltip("The canvas used by the tooltip as positioning and scaling reference. Should usually be the root Canvas of the hierarchy this component is in")]
		public Canvas canvas;

		[Tooltip("Sets if tooltip triggers will run ForceUpdateCanvases and refresh the tooltip's layout group (if any) when hovered, in order to prevent momentousness misplacement sometimes caused by ContentSizeFitters")]
		public bool tooltipTriggersCanForceCanvasUpdate;

		private LayoutGroup _layoutGroup;

		private bool _inside;

		private float width;

		private float height;

		public float YShift;

		public float xShift;

		[HideInInspector]
		public RenderMode guiMode;

		private Camera _guiCamera;

		private Vector3 screenLowerLeft;

		private Vector3 screenUpperRight;

		private Vector3 shiftingVector;

		private Vector3 baseTooltipPos;

		private Vector3 newTTPos;

		private Vector3 adjustedNewTTPos;

		private Vector3 adjustedTTLocalPos;

		private Vector3 shifterForBorders;

		private float borderTest;

		private static ToolTip instance;

		public Camera GuiCamera
		{
			get
			{
				if (!_guiCamera)
				{
					_guiCamera = Camera.main;
				}
				return _guiCamera;
			}
		}

		public static ToolTip Instance
		{
			get
			{
				if (instance == null)
				{
					instance = Object.FindObjectOfType<ToolTip>();
				}
				return instance;
			}
		}

		private void Reset()
		{
			canvas = GetComponentInParent<Canvas>();
			canvas = canvas.rootCanvas;
		}

		public void Awake()
		{
			instance = this;
			if (!canvas)
			{
				canvas = GetComponentInParent<Canvas>();
				canvas = canvas.rootCanvas;
			}
			_guiCamera = canvas.worldCamera;
			guiMode = canvas.renderMode;
			_rectTransform = GetComponent<RectTransform>();
			canvasRectTransform = canvas.GetComponent<RectTransform>();
			_layoutGroup = GetComponentInChildren<LayoutGroup>();
			_text = GetComponentInChildren<Text>();
			_inside = false;
			base.gameObject.SetActive(value: false);
		}

		public void SetTooltip(string ttext)
		{
			SetTooltip(ttext, base.transform.position);
		}

		public void SetTooltip(string ttext, Vector3 basePos, bool refreshCanvasesBeforeGetSize = false)
		{
			baseTooltipPos = basePos;
			if ((bool)_text)
			{
				_text.text = ttext;
			}
			else
			{
				Debug.LogWarning("[ToolTip] Couldn't set tooltip text, tooltip has no child Text component");
			}
			ContextualTooltipUpdate(refreshCanvasesBeforeGetSize);
		}

		public void HideTooltip()
		{
			base.gameObject.SetActive(value: false);
			_inside = false;
		}

		private void Update()
		{
			if (_inside)
			{
				ContextualTooltipUpdate();
			}
		}

		public void RefreshTooltipSize()
		{
			if (tooltipTriggersCanForceCanvasUpdate)
			{
				Canvas.ForceUpdateCanvases();
				if ((bool)_layoutGroup)
				{
					_layoutGroup.enabled = false;
					_layoutGroup.enabled = true;
				}
			}
		}

		public void ContextualTooltipUpdate(bool refreshCanvasesBeforeGettingSize = false)
		{
			switch (guiMode)
			{
			case RenderMode.ScreenSpaceCamera:
				OnScreenSpaceCamera(refreshCanvasesBeforeGettingSize);
				break;
			case RenderMode.ScreenSpaceOverlay:
				OnScreenSpaceOverlay(refreshCanvasesBeforeGettingSize);
				break;
			}
		}

		public void OnScreenSpaceCamera(bool refreshCanvasesBeforeGettingSize = false)
		{
			shiftingVector.x = xShift;
			shiftingVector.y = YShift;
			baseTooltipPos.z = canvas.planeDistance;
			newTTPos = GuiCamera.ScreenToViewportPoint(baseTooltipPos - shiftingVector);
			adjustedNewTTPos = GuiCamera.ViewportToWorldPoint(newTTPos);
			base.gameObject.SetActive(value: true);
			if (refreshCanvasesBeforeGettingSize)
			{
				RefreshTooltipSize();
			}
			width = base.transform.lossyScale.x * _rectTransform.sizeDelta[0];
			height = base.transform.lossyScale.y * _rectTransform.sizeDelta[1];
			RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRectTransform, Vector2.zero, GuiCamera, out screenLowerLeft);
			RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRectTransform, new Vector2(Screen.width, Screen.height), GuiCamera, out screenUpperRight);
			borderTest = adjustedNewTTPos.x + width / 2f;
			if (borderTest > screenUpperRight.x)
			{
				shifterForBorders.x = borderTest - screenUpperRight.x;
				adjustedNewTTPos.x -= shifterForBorders.x;
			}
			borderTest = adjustedNewTTPos.x - width / 2f;
			if (borderTest < screenLowerLeft.x)
			{
				shifterForBorders.x = screenLowerLeft.x - borderTest;
				adjustedNewTTPos.x += shifterForBorders.x;
			}
			borderTest = adjustedNewTTPos.y - height / 2f;
			if (borderTest < screenLowerLeft.y)
			{
				shifterForBorders.y = screenLowerLeft.y - borderTest;
				adjustedNewTTPos.y += shifterForBorders.y;
			}
			borderTest = adjustedNewTTPos.y + height / 2f;
			if (borderTest > screenUpperRight.y)
			{
				shifterForBorders.y = borderTest - screenUpperRight.y;
				adjustedNewTTPos.y -= shifterForBorders.y;
			}
			adjustedNewTTPos = base.transform.rotation * adjustedNewTTPos;
			base.transform.position = adjustedNewTTPos;
			adjustedTTLocalPos = base.transform.localPosition;
			adjustedTTLocalPos.z = 0f;
			base.transform.localPosition = adjustedTTLocalPos;
			_inside = true;
		}

		public void OnScreenSpaceOverlay(bool refreshCanvasesBeforeGettingSize = false)
		{
			shiftingVector.x = xShift;
			shiftingVector.y = YShift;
			newTTPos = (baseTooltipPos - shiftingVector) / canvas.scaleFactor;
			adjustedNewTTPos = newTTPos;
			base.gameObject.SetActive(value: true);
			if (refreshCanvasesBeforeGettingSize)
			{
				RefreshTooltipSize();
			}
			width = _rectTransform.sizeDelta[0];
			height = _rectTransform.sizeDelta[1];
			screenLowerLeft = Vector3.zero;
			screenUpperRight = canvasRectTransform.sizeDelta;
			borderTest = newTTPos.x + width / 2f;
			if (borderTest > screenUpperRight.x)
			{
				shifterForBorders.x = borderTest - screenUpperRight.x;
				adjustedNewTTPos.x -= shifterForBorders.x;
			}
			borderTest = adjustedNewTTPos.x - width / 2f;
			if (borderTest < screenLowerLeft.x)
			{
				shifterForBorders.x = screenLowerLeft.x - borderTest;
				adjustedNewTTPos.x += shifterForBorders.x;
			}
			borderTest = adjustedNewTTPos.y - height / 2f;
			if (borderTest < screenLowerLeft.y)
			{
				shifterForBorders.y = screenLowerLeft.y - borderTest;
				adjustedNewTTPos.y += shifterForBorders.y;
			}
			borderTest = adjustedNewTTPos.y + height / 2f;
			if (borderTest > screenUpperRight.y)
			{
				shifterForBorders.y = borderTest - screenUpperRight.y;
				adjustedNewTTPos.y -= shifterForBorders.y;
			}
			adjustedNewTTPos *= canvas.scaleFactor;
			base.transform.position = adjustedNewTTPos;
			_inside = true;
		}
	}
}

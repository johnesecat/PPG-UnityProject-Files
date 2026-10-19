namespace UnityEngine.UI.Extensions
{
	public static class UIExtensionMethods
	{
		public static Canvas GetParentCanvas(this RectTransform rt)
		{
			RectTransform rectTransform = rt;
			Canvas canvas = rt.GetComponent<Canvas>();
			int num = 0;
			while (canvas == null || num > 50)
			{
				canvas = rt.GetComponentInParent<Canvas>();
				if (canvas == null)
				{
					rectTransform = rectTransform.parent.GetComponent<RectTransform>();
					num++;
				}
			}
			return canvas;
		}

		public static Vector2 TransformInputBasedOnCanvasType(this Vector2 input, Canvas canvas)
		{
			if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
			{
				return canvas.GetEventCamera().ScreenToWorldPoint(input);
			}
			return input;
		}

		public static Vector3 TransformInputBasedOnCanvasType(this Vector2 input, RectTransform rt)
		{
			Canvas parentCanvas = rt.GetParentCanvas();
			if (input == Vector2.zero || parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
			{
				return input;
			}
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, input, parentCanvas.GetEventCamera(), out var localPoint);
			return parentCanvas.transform.TransformPoint(localPoint);
		}

		public static Camera GetEventCamera(this Canvas input)
		{
			if (!(input.worldCamera == null))
			{
				return input.worldCamera;
			}
			return Camera.main;
		}
	}
}

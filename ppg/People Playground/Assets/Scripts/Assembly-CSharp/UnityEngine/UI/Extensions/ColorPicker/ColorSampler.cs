using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions.ColorPicker
{
	public class ColorSampler : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IDragHandler
	{
		private Vector2 m_screenPos;

		[SerializeField]
		protected Button sampler;

		private RectTransform sampleRectTransform;

		[SerializeField]
		protected Outline samplerOutline;

		protected Texture2D screenCapture;

		public ColorChangedEvent oncolorSelected = new ColorChangedEvent();

		protected Color color;

		protected virtual void OnEnable()
		{
			screenCapture = ScreenCapture.CaptureScreenshotAsTexture();
			sampleRectTransform = sampler.GetComponent<RectTransform>();
			sampler.gameObject.SetActive(value: true);
			sampler.onClick.AddListener(SelectColor);
		}

		protected virtual void OnDisable()
		{
			Object.Destroy(screenCapture);
			sampler.gameObject.SetActive(value: false);
			sampler.onClick.RemoveListener(SelectColor);
		}

		protected virtual void Update()
		{
			if (!(screenCapture == null))
			{
				sampleRectTransform.position = m_screenPos;
				color = screenCapture.GetPixel((int)m_screenPos.x, (int)m_screenPos.y);
				HandleSamplerColoring();
			}
		}

		protected virtual void HandleSamplerColoring()
		{
			sampler.image.color = color;
			if ((bool)samplerOutline)
			{
				Color effectColor = Color.Lerp(Color.white, Color.black, (color.grayscale > 0.5f) ? 1 : 0);
				effectColor.a = samplerOutline.effectColor.a;
				samplerOutline.effectColor = effectColor;
			}
		}

		protected virtual void SelectColor()
		{
			if (oncolorSelected != null)
			{
				oncolorSelected.Invoke(color);
			}
			base.enabled = false;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			m_screenPos = eventData.position;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			m_screenPos = Vector2.zero;
		}

		public void OnDrag(PointerEventData eventData)
		{
			m_screenPos = eventData.position;
		}
	}
}

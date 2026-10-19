namespace UnityEngine.UI.Extensions
{
	public class CardExpanding2D : MonoBehaviour
	{
		[SerializeField]
		private float lerpSpeed = 8f;

		[SerializeField]
		private RectTransform buttonRect;

		private Vector2 closeButtonMin = Vector2.zero;

		private Vector2 closeButtonMax = Vector2.zero;

		[SerializeField]
		private Vector2 cardSize = Vector2.zero;

		[SerializeField]
		private Vector2 pageSize = Vector2.zero;

		private Vector2 cardCenter = Vector2.zero;

		private Vector2 pageCenter = Vector2.zero;

		private Vector2 cardMin = Vector2.zero;

		private Vector2 cardMax = Vector2.zero;

		private Vector2 pageMin = Vector2.zero;

		private Vector2 pageMax = Vector2.zero;

		private RectTransform rectTrans;

		private int animationActive = -1;

		private void Start()
		{
			rectTrans = GetComponent<RectTransform>();
			buttonRect.GetComponent<Image>().color = new Color32(228, 0, 0, 0);
			closeButtonMin = new Vector2(pageMin.x + pageSize.x - 64f, pageMin.y + pageSize.y - 64f);
			closeButtonMax = new Vector2(pageMax.x - 16f, pageMax.y - 16f);
			cardMin = new Vector2(cardCenter.x - cardSize.x * 0.5f, cardCenter.y - cardSize.y * 0.5f);
			cardMax = new Vector2(cardCenter.x + cardSize.x * 0.5f, cardCenter.y + cardSize.y * 0.5f);
			pageMin = new Vector2(pageCenter.x - pageSize.x * 0.5f, pageCenter.y - pageSize.y * 0.5f);
			pageMax = new Vector2(pageCenter.x + pageSize.x * 0.5f, pageCenter.y + pageSize.y * 0.5f);
		}

		private void Update()
		{
			if (animationActive == 1)
			{
				rectTrans.offsetMin = Vector2.Lerp(rectTrans.offsetMin, pageMin, Time.deltaTime * lerpSpeed);
				rectTrans.offsetMax = Vector2.Lerp(rectTrans.offsetMax, pageMax, Time.deltaTime * lerpSpeed);
				if (rectTrans.offsetMin.x < pageMin.x * 0.995f && rectTrans.offsetMin.y < pageMin.y * 0.995f && rectTrans.offsetMax.x > pageMax.x * 0.995f && rectTrans.offsetMax.y > pageMax.y * 0.995f)
				{
					rectTrans.offsetMin = pageMin;
					rectTrans.offsetMax = pageMax;
					buttonRect.GetComponent<Image>().color = Color32.Lerp(buttonRect.GetComponent<Image>().color, new Color32(228, 0, 0, 191), Time.deltaTime * lerpSpeed);
					if (Mathf.Abs(buttonRect.GetComponent<Image>().color.a - 191f) < 2f)
					{
						buttonRect.GetComponent<Image>().color = new Color32(228, 0, 0, 191);
						animationActive = 0;
						CardStack2D.canUseHorizontalAxis = true;
					}
				}
			}
			else if (animationActive == -1)
			{
				buttonRect.GetComponent<Image>().color = Color32.Lerp(buttonRect.GetComponent<Image>().color, new Color32(228, 0, 0, 0), Time.deltaTime * lerpSpeed * 1.25f);
				rectTrans.offsetMin = Vector2.Lerp(rectTrans.offsetMin, cardMin, Time.deltaTime * lerpSpeed);
				rectTrans.offsetMax = Vector2.Lerp(rectTrans.offsetMax, cardMax, Time.deltaTime * lerpSpeed);
				if (rectTrans.offsetMin.x > cardMin.x * 1.005f && rectTrans.offsetMin.y > cardMin.y * 1.005f && rectTrans.offsetMax.x < cardMax.x * 1.005f && rectTrans.offsetMax.y < cardMax.y * 1.005f)
				{
					rectTrans.offsetMin = cardMin;
					rectTrans.offsetMax = cardMax;
					buttonRect.offsetMin = Vector2.zero;
					buttonRect.offsetMax = Vector2.zero;
					animationActive = 0;
					CardStack2D.canUseHorizontalAxis = true;
				}
			}
		}

		public void ToggleCard()
		{
			CardStack2D.canUseHorizontalAxis = false;
			if (animationActive != 1)
			{
				animationActive = 1;
				cardCenter = base.transform.localPosition;
				buttonRect.offsetMin = closeButtonMin;
				buttonRect.offsetMax = closeButtonMax;
			}
			else if (animationActive != -1)
			{
				animationActive = -1;
			}
		}
	}
}

namespace UnityEngine.UI.Extensions
{
	[ExecuteInEditMode]
	public class CardExpanding3D : MonoBehaviour
	{
		[SerializeField]
		private float lerpSpeed = 12f;

		[SerializeField]
		private float cornerSize = 64f;

		[Header("Parts")]
		public RectTransform[] cardCorners;

		public RectTransform[] cardEdges;

		public RectTransform cardCenter;

		[Header("Card Info")]
		[Tooltip("Positions and sizes card to its current transform.")]
		public bool cardAutoSize = true;

		public Vector2 cardSize;

		public Vector2 cardPosition;

		[Range(1f, 96f)]
		public int cardSuperness = 4;

		[Header("Page Info")]
		[Tooltip("Positions and sizes the page to the top third of the screen.")]
		public bool pageAutoSize = true;

		public Vector2 pageSize;

		public Vector2 pagePosition;

		[Range(1f, 96f)]
		public int pageSuperness = 96;

		private int animationActive;

		private Vector2[] nextCornerPos = new Vector2[4];

		private Vector2[] nextEdgePos = new Vector2[4];

		private Vector2[] nextEdgeScale = new Vector2[4];

		private Vector2 nextCenterScale;

		private Vector2 nextPos;

		private int nextSuperness;

		private RectTransform rect;

		private Vector2 nextMin;

		private Vector2 nextMax;

		private void Start()
		{
			if (cardAutoSize)
			{
				cardSize = new Vector2(cardCorners[0].localScale.x * 2f + cardEdges[0].localScale.x, cardCorners[0].localScale.y * 2f + cardEdges[0].localScale.y);
				cardPosition = cardCenter.localPosition;
			}
			if (pageAutoSize)
			{
				pageSize = new Vector2(Screen.width, Screen.height / 3);
				pagePosition = new Vector2(0f, (float)(Screen.height / 2) - pageSize.y / 2f);
			}
			rect = GetComponent<RectTransform>();
		}

		private void Update()
		{
			if (animationActive != 1 && animationActive != -1)
			{
				return;
			}
			for (int i = 0; i < cardCorners.Length; i++)
			{
				cardCorners[i].localPosition = Vector3.Lerp(cardCorners[i].localPosition, nextCornerPos[i], Time.deltaTime * lerpSpeed);
				cardCorners[i].GetComponent<SuperellipsePoints>().superness = Mathf.Lerp(cardCorners[i].GetComponent<SuperellipsePoints>().superness, nextSuperness, Time.deltaTime * lerpSpeed);
				if (Mathf.Abs(cardCorners[i].GetComponent<SuperellipsePoints>().superness - (float)nextSuperness) <= 1f)
				{
					cardCorners[i].localPosition = nextCornerPos[i];
					cardEdges[i].localPosition = nextEdgePos[i];
					cardEdges[i].localScale = new Vector3(nextEdgeScale[i].x, nextEdgeScale[i].y, 1f);
					base.transform.localPosition = nextPos;
					cardCenter.localScale = new Vector3(nextCenterScale.x, nextCenterScale.y, 1f);
					cardCorners[i].GetComponent<SuperellipsePoints>().superness = nextSuperness;
					rect.offsetMin = nextMin;
					rect.offsetMax = nextMax;
				}
			}
			for (int j = 0; j < cardEdges.Length; j++)
			{
				cardEdges[j].localPosition = Vector3.Lerp(cardEdges[j].localPosition, nextEdgePos[j], Time.deltaTime * lerpSpeed);
				cardEdges[j].localScale = Vector3.Lerp(cardEdges[j].localScale, new Vector3(nextEdgeScale[j].x, nextEdgeScale[j].y, 1f), Time.deltaTime * lerpSpeed);
			}
			base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, nextPos, Time.deltaTime * lerpSpeed);
			cardCenter.localScale = Vector3.Lerp(cardCenter.localScale, new Vector3(nextCenterScale.x, nextCenterScale.y, 1f), Time.deltaTime * lerpSpeed);
			rect.offsetMin = Vector3.Lerp(rect.offsetMin, nextMin, Time.deltaTime * lerpSpeed);
			rect.offsetMax = Vector3.Lerp(rect.offsetMax, nextMax, Time.deltaTime * lerpSpeed);
		}

		public void ToggleCard()
		{
			if (animationActive != 1 || animationActive == 0)
			{
				animationActive = 1;
				for (int i = 0; i < cardCorners.Length; i++)
				{
					float x = pageSize.x / 2f * Mathf.Sign(cardCorners[i].localScale.x) - cardCorners[i].localScale.x;
					float y = pageSize.y / 2f * Mathf.Sign(cardCorners[i].localScale.y) - cardCorners[i].localScale.y;
					nextCornerPos[i] = new Vector2(x, y);
				}
				for (int j = 0; j < cardEdges.Length; j++)
				{
					float x2 = 0f;
					float y2 = 0f;
					float x3 = 0f;
					float y3 = 0f;
					if (cardEdges[j].localPosition.x != 0f)
					{
						x2 = Mathf.Sign(cardEdges[j].localPosition.x) * (pageSize.x / 2f - cardEdges[j].localScale.x / 2f);
						y2 = 0f;
						x3 = cornerSize;
						y3 = pageSize.y - cornerSize * 2f;
					}
					else if (cardEdges[j].localPosition.y != 0f)
					{
						x2 = 0f;
						y2 = Mathf.Sign(cardEdges[j].localPosition.y) * (pageSize.y / 2f - cardEdges[j].localScale.y / 2f);
						x3 = pageSize.x - cornerSize * 2f;
						y3 = cornerSize;
					}
					nextEdgePos[j] = new Vector2(x2, y2);
					nextEdgeScale[j] = new Vector2(x3, y3);
				}
				nextCenterScale = pageSize - new Vector2(cornerSize * 2f, cornerSize * 2f);
				nextPos = pagePosition;
				nextSuperness = pageSuperness;
				nextMin = new Vector2((0f - pageSize.x) / 2f, (0f - pageSize.y) / 2f) + nextPos;
				nextMax = new Vector2(pageSize.x / 2f, pageSize.y / 2f) + nextPos;
			}
			else
			{
				if (animationActive == -1)
				{
					return;
				}
				animationActive = -1;
				for (int k = 0; k < cardCorners.Length; k++)
				{
					float x4 = Mathf.Sign(cardCorners[k].localScale.x) * (cardSize.x / 2f) - cardCorners[k].localScale.x;
					float y4 = Mathf.Sign(cardCorners[k].localScale.y) * (cardSize.y / 2f) - cardCorners[k].localScale.y;
					nextCornerPos[k] = new Vector2(x4, y4);
				}
				for (int l = 0; l < cardEdges.Length; l++)
				{
					float x5 = 0f;
					float y5 = 0f;
					float x6 = 0f;
					float y6 = 0f;
					if (cardEdges[l].localPosition.x != 0f)
					{
						x5 = Mathf.Sign(cardEdges[l].localPosition.x) * (cardSize.x / 2f) - Mathf.Sign(cardEdges[l].localPosition.x) * (cardEdges[l].localScale.x / 2f);
						y5 = 0f;
						x6 = cornerSize;
						y6 = cardSize.y - cornerSize * 2f;
					}
					else if (cardEdges[l].localPosition.y != 0f)
					{
						x5 = 0f;
						y5 = Mathf.Sign(cardEdges[l].localPosition.y) * (cardSize.y / 2f) - Mathf.Sign(cardEdges[l].localPosition.y) * (cardEdges[l].localScale.y / 2f);
						x6 = cardSize.x - cornerSize * 2f;
						y6 = cornerSize;
					}
					nextEdgePos[l] = new Vector2(x5, y5);
					nextEdgeScale[l] = new Vector2(x6, y6);
				}
				nextCenterScale = cardSize - new Vector2(cornerSize * 2f, cornerSize * 2f);
				nextPos = cardPosition;
				nextSuperness = cardSuperness;
				nextMin = new Vector2((0f - cardSize.x) / 2f, (0f - cardSize.y) / 2f) + nextPos;
				nextMax = new Vector2(cardSize.x / 2f, cardSize.y / 2f) + nextPos;
			}
		}
	}
}

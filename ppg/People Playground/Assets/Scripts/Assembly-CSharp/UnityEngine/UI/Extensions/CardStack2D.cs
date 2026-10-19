using System.Collections;

namespace UnityEngine.UI.Extensions
{
	public class CardStack2D : MonoBehaviour
	{
		[SerializeField]
		private float cardMoveSpeed = 8f;

		[SerializeField]
		private float buttonCooldownTime = 0.125f;

		[SerializeField]
		private int cardZMultiplier = 32;

		[SerializeField]
		private bool useDefaultUsedXPos = true;

		[SerializeField]
		private int usedCardXPos = 1280;

		[SerializeField]
		private KeyCode leftButton = KeyCode.LeftArrow;

		[SerializeField]
		private KeyCode rightButton = KeyCode.RightArrow;

		[SerializeField]
		private Transform[] cards;

		private int cardArrayOffset;

		private Vector3[] cardPositions;

		private int xPowerDifference;

		public static bool canUseHorizontalAxis = true;

		private void Start()
		{
			xPowerDifference = 9 - cards.Length;
			if (useDefaultUsedXPos)
			{
				int num = (int)cards[0].GetComponent<RectTransform>().rect.width;
				usedCardXPos = (int)((float)Screen.width * 0.5f + (float)num);
			}
			cardPositions = new Vector3[cards.Length * 2 - 1];
			for (int num2 = cards.Length; num2 > -1; num2--)
			{
				if (num2 < cards.Length - 1)
				{
					cardPositions[num2] = new Vector3(0f - Mathf.Pow(2f, num2 + xPowerDifference) + cardPositions[num2 + 1].x, 0f, cardZMultiplier * Mathf.Abs(num2 + 1 - cards.Length));
				}
				else
				{
					cardPositions[num2] = Vector3.zero;
				}
			}
			for (int i = cards.Length; i < cardPositions.Length; i++)
			{
				cardPositions[i] = new Vector3(usedCardXPos + 4 * (i - cards.Length), 0f, -2 + -2 * (i - cards.Length));
			}
		}

		private void Update()
		{
			if (canUseHorizontalAxis)
			{
				if ((UIExtensionsInputManager.GetAxisRaw("Horizontal") < 0f || UIExtensionsInputManager.GetKey(leftButton)) && cardArrayOffset > 0)
				{
					cardArrayOffset--;
					StartCoroutine(ButtonCooldown());
				}
				else if ((UIExtensionsInputManager.GetAxisRaw("Horizontal") > 0f || UIExtensionsInputManager.GetKey(rightButton)) && cardArrayOffset < cards.Length - 1)
				{
					cardArrayOffset++;
					StartCoroutine(ButtonCooldown());
				}
			}
			for (int i = 0; i < cards.Length; i++)
			{
				cards[i].localPosition = Vector3.Lerp(cards[i].localPosition, cardPositions[i + cardArrayOffset], Time.deltaTime * cardMoveSpeed);
				if (Mathf.Abs(cards[i].localPosition.x - cardPositions[i + cardArrayOffset].x) < 0.01f)
				{
					cards[i].localPosition = cardPositions[i + cardArrayOffset];
					if (cards[i].localPosition.x == 0f)
					{
						cards[i].gameObject.GetComponent<CanvasGroup>().interactable = true;
					}
					else
					{
						cards[i].gameObject.GetComponent<CanvasGroup>().interactable = false;
					}
				}
			}
		}

		private IEnumerator ButtonCooldown()
		{
			canUseHorizontalAxis = false;
			yield return new WaitForSeconds(buttonCooldownTime);
			canUseHorizontalAxis = true;
		}
	}
}

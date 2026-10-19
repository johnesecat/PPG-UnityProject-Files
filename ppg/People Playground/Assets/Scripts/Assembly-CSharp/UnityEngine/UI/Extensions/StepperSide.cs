using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(Selectable))]
	public class StepperSide : UIBehaviour, IPointerClickHandler, IEventSystemHandler, ISubmitHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
	{
		internal Sprite cutSprite;

		private Selectable button => GetComponent<Selectable>();

		private Stepper stepper => GetComponentInParent<Stepper>();

		private bool leftmost => button == stepper.sides[0];

		protected StepperSide()
		{
		}

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				Press();
				AdjustSprite(restore: false);
			}
		}

		public virtual void OnSubmit(BaseEventData eventData)
		{
			Press();
			AdjustSprite(restore: true);
		}

		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			AdjustSprite(restore: false);
		}

		public virtual void OnPointerExit(PointerEventData eventData)
		{
			AdjustSprite(restore: true);
		}

		public virtual void OnPointerDown(PointerEventData eventData)
		{
			AdjustSprite(restore: false);
		}

		public virtual void OnPointerUp(PointerEventData eventData)
		{
			AdjustSprite(restore: false);
		}

		public virtual void OnSelect(BaseEventData eventData)
		{
			AdjustSprite(restore: false);
		}

		public virtual void OnDeselect(BaseEventData eventData)
		{
			AdjustSprite(restore: true);
		}

		private void Press()
		{
			if (button.IsActive() && button.IsInteractable())
			{
				if (leftmost)
				{
					stepper.StepDown();
				}
				else
				{
					stepper.StepUp();
				}
			}
		}

		private void AdjustSprite(bool restore)
		{
			Image image = button.image;
			if ((bool)image && !(image.overrideSprite == cutSprite))
			{
				if (restore)
				{
					image.overrideSprite = cutSprite;
				}
				else
				{
					image.overrideSprite = Stepper.CutSprite(image.overrideSprite, leftmost);
				}
			}
		}
	}
}

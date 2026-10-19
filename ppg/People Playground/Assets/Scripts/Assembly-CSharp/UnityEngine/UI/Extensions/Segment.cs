using System.Collections;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/Segmented Control/Segment")]
	[RequireComponent(typeof(Selectable))]
	public class Segment : UIBehaviour, IPointerClickHandler, IEventSystemHandler, ISubmitHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
	{
		internal int index;

		internal SegmentedControl segmentedControl;

		internal Sprite cutSprite;

		internal bool leftmost => index == 0;

		internal bool rightmost => index == segmentedControl.segments.Length - 1;

		public bool selected
		{
			get
			{
				return segmentedControl.selectedSegment == button;
			}
			set
			{
				SetSelected(value);
			}
		}

		internal Selectable button => GetComponent<Selectable>();

		protected Segment()
		{
		}

		protected override void Start()
		{
			StartCoroutine(DelayedInit());
		}

		private IEnumerator DelayedInit()
		{
			yield return null;
			yield return null;
			button.image.overrideSprite = cutSprite;
			if (selected)
			{
				MaintainSelection();
			}
		}

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				selected = true;
			}
		}

		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			MaintainSelection();
		}

		public virtual void OnPointerExit(PointerEventData eventData)
		{
			MaintainSelection();
		}

		public virtual void OnPointerDown(PointerEventData eventData)
		{
			MaintainSelection();
		}

		public virtual void OnPointerUp(PointerEventData eventData)
		{
			MaintainSelection();
		}

		public virtual void OnSelect(BaseEventData eventData)
		{
			MaintainSelection();
		}

		public virtual void OnDeselect(BaseEventData eventData)
		{
			MaintainSelection();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if ((bool)segmentedControl)
			{
				MaintainSelection();
			}
		}

		public virtual void OnSubmit(BaseEventData eventData)
		{
			selected = true;
		}

		private void SetSelected(bool value)
		{
			if (value && button.IsActive() && button.IsInteractable())
			{
				if (segmentedControl.selectedSegment == button)
				{
					if (segmentedControl.allowSwitchingOff)
					{
						Deselect();
					}
					else
					{
						MaintainSelection();
					}
					return;
				}
				if ((bool)segmentedControl.selectedSegment)
				{
					Segment component = segmentedControl.selectedSegment.GetComponent<Segment>();
					segmentedControl.selectedSegment = null;
					if ((bool)component)
					{
						component.TransitionButton();
					}
				}
				segmentedControl.selectedSegment = button;
				TransitionButton();
				segmentedControl.onValueChanged.Invoke(index);
			}
			else if (segmentedControl.selectedSegment == button)
			{
				Deselect();
			}
		}

		private void Deselect()
		{
			segmentedControl.selectedSegment = null;
			TransitionButton();
			segmentedControl.onValueChanged.Invoke(-1);
		}

		private void MaintainSelection()
		{
			if (!(button != segmentedControl.selectedSegment))
			{
				TransitionButton(instant: true);
			}
		}

		internal void TransitionButton()
		{
			TransitionButton(instant: false);
		}

		internal void TransitionButton(bool instant)
		{
			Color color = (selected ? button.colors.pressedColor : button.colors.normalColor);
			Color color2 = (selected ? button.colors.normalColor : button.colors.pressedColor);
			Sprite sprite = (selected ? button.spriteState.pressedSprite : cutSprite);
			string triggername = (selected ? button.animationTriggers.pressedTrigger : button.animationTriggers.normalTrigger);
			switch (button.transition)
			{
			case Selectable.Transition.ColorTint:
				button.image.overrideSprite = cutSprite;
				StartColorTween(color * button.colors.colorMultiplier, instant);
				ChangeTextColor(color2 * button.colors.colorMultiplier);
				break;
			case Selectable.Transition.SpriteSwap:
				if (sprite != cutSprite)
				{
					sprite = SegmentedControl.CutSprite(sprite, leftmost, rightmost);
				}
				DoSpriteSwap(sprite);
				break;
			case Selectable.Transition.Animation:
				button.image.overrideSprite = cutSprite;
				TriggerAnimation(triggername);
				break;
			}
		}

		private void StartColorTween(Color targetColor, bool instant)
		{
			if (!(button.targetGraphic == null))
			{
				button.targetGraphic.CrossFadeColor(targetColor, instant ? 0f : button.colors.fadeDuration, ignoreTimeScale: true, useAlpha: true);
			}
		}

		private void ChangeTextColor(Color targetColor)
		{
			Text componentInChildren = GetComponentInChildren<Text>();
			if ((bool)componentInChildren)
			{
				componentInChildren.color = targetColor;
			}
		}

		private void DoSpriteSwap(Sprite newSprite)
		{
			if (!(button.image == null))
			{
				button.image.overrideSprite = newSprite;
			}
		}

		private void TriggerAnimation(string triggername)
		{
			if (!(button.animator == null) && button.animator.isActiveAndEnabled && button.animator.hasBoundPlayables && !string.IsNullOrEmpty(triggername))
			{
				button.animator.ResetTrigger(button.animationTriggers.normalTrigger);
				button.animator.ResetTrigger(button.animationTriggers.pressedTrigger);
				button.animator.ResetTrigger(button.animationTriggers.highlightedTrigger);
				button.animator.ResetTrigger(button.animationTriggers.disabledTrigger);
				button.animator.SetTrigger(triggername);
			}
		}
	}
}

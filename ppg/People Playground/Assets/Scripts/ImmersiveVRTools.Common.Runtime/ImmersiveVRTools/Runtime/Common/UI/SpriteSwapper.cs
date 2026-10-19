using UnityEngine;
using UnityEngine.UI;

namespace ImmersiveVRTools.Runtime.Common.UI
{
	[RequireComponent(typeof(Image))]
	public class SpriteSwapper : MonoBehaviour
	{
		[SerializeField]
		private Sprite _firstSprite;

		[SerializeField]
		private Sprite _secondSprite;

		private bool _isFirstSpriteVisible = true;

		private Image _image;

		public void Awake()
		{
			_image = GetComponent<Image>();
		}

		public void Swap()
		{
			if (_isFirstSpriteVisible)
			{
				ShowSecond();
			}
			else
			{
				ShowFirst();
			}
		}

		public void ShowFirst()
		{
			_isFirstSpriteVisible = true;
			_image.sprite = _firstSprite;
		}

		public void ShowSecond()
		{
			_isFirstSpriteVisible = false;
			_image.sprite = _secondSprite;
		}
	}
}

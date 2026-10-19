using System;
using UnityEngine;

namespace Linefy
{
	[Serializable]
	public class EditorIconContent
	{
		public string tooltip;

		[SerializeField]
		public Texture2D brightTexture;

		[SerializeField]
		public Texture2D darkTexture;

		private GUIContent _bright;

		private GUIContent _dark;

		public GUIContent bright
		{
			get
			{
				if (_bright == null)
				{
					_bright = new GUIContent(brightTexture, tooltip);
				}
				_bright.image = brightTexture;
				_bright.tooltip = tooltip;
				return _bright;
			}
		}

		public GUIContent dark
		{
			get
			{
				if (_dark == null)
				{
					_dark = new GUIContent(darkTexture, tooltip);
				}
				_dark.image = darkTexture;
				_dark.tooltip = tooltip;
				return _dark;
			}
		}
	}
}

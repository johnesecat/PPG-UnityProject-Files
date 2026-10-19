using System;
using UnityEngine;

namespace Linefy
{
	[Serializable]
	public struct Label
	{
		public string text;

		public Vector3 position;

		public Vector2 offset;

		public Label(string text, Vector3 position, Vector2 offset)
		{
			this.text = text;
			this.position = position;
			this.offset = offset;
		}

		public Label(string text, Vector3 position)
		{
			this.text = text;
			this.position = position;
			offset = Vector2.zero;
		}

		public Label(string text)
		{
			this.text = text;
			position = Vector3.zero;
			offset = Vector2.zero;
		}
	}
}

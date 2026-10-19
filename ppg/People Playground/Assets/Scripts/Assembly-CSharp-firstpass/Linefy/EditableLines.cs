using Linefy.Serialization;
using UnityEngine;

namespace Linefy
{
	[ExecuteInEditMode]
	public class EditableLines : EditableDrawableBase
	{
		public SerializationData_Lines properties = new SerializationData_Lines();

		public Line[] items = new Line[2]
		{
			new Line(new Vector3(-0.7f, 0.7f), new Vector3(0.7f, -0.7f), Color.red, Color.yellow, 1f),
			new Line(new Vector3(0.7f, 0.7f), new Vector3(-0.7f, -0.7f), Color.cyan, Color.green, 1f)
		};

		private Lines _lines;

		private Lines lines
		{
			get
			{
				if (_lines == null)
				{
					_lines = new Lines(properties);
				}
				return _lines;
			}
		}

		public override Drawable drawable => lines;

		protected override void PreDraw()
		{
			base.PreDraw();
			if (itemsModificationId != lines.itemsModificationId)
			{
				lines.count = items.Length;
				for (int i = 0; i < items.Length; i++)
				{
					lines[i] = items[i];
				}
				lines.itemsModificationId = itemsModificationId;
			}
			if (propertiesModificationId != lines.propertiesModificationId)
			{
				lines.LoadSerializationData(properties);
				lines.propertiesModificationId = propertiesModificationId;
			}
		}

		public static EditableLines CreateInstance()
		{
			return new GameObject("New EditableLines").AddComponent<EditableLines>();
		}
	}
}

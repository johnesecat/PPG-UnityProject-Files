using Linefy.Serialization;
using UnityEngine;

namespace Linefy
{
	[ExecuteInEditMode]
	public class EditableDots : EditableDrawableBase
	{
		public SerializationData_Dots properties = new SerializationData_Dots();

		public Dot[] items = new Dot[4]
		{
			new Dot(new Vector3(-0.7f, 0f, 0f), 1f, 3, Color.green),
			new Dot(new Vector3(0f, 0.7f, 0f), 1f, 19, Color.red),
			new Dot(new Vector3(0.7f, 0f, 0f), 1f, 35, Color.cyan),
			new Dot(new Vector3(0f, -0.7f, 0f), 1f, 51, Color.yellow)
		};

		private Dots _dots;

		private Dots dots
		{
			get
			{
				if (_dots == null)
				{
					_dots = new Dots(properties);
				}
				return _dots;
			}
		}

		public override Drawable drawable => dots;

		protected override void PreDraw()
		{
			base.PreDraw();
			if (itemsModificationId != dots.itemsModificationId)
			{
				dots.count = items.Length;
				for (int i = 0; i < items.Length; i++)
				{
					dots[i] = items[i];
				}
				dots.itemsModificationId = itemsModificationId;
			}
			if (propertiesModificationId != dots.propertiesModificationId)
			{
				dots.LoadSerializationData(properties);
				dots.propertiesModificationId = propertiesModificationId;
			}
		}

		public static EditableDots CreateInstance()
		{
			return new GameObject("New EditableDots").AddComponent<EditableDots>();
		}
	}
}

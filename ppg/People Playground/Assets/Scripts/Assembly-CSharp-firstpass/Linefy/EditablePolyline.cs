using Linefy.Serialization;
using UnityEngine;

namespace Linefy
{
	[ExecuteInEditMode]
	public class EditablePolyline : EditableDrawableBase
	{
		[Tooltip("Polyline properties")]
		public SerializationData_Polyline properties = new SerializationData_Polyline(1f);

		[Tooltip("Polyline vertices")]
		public PolylineVertex[] items = new PolylineVertex[4]
		{
			new PolylineVertex(new Vector3(-0.5f, 0f, 0f), Color.red, 1f, 0f),
			new PolylineVertex(new Vector3(0f, 0.5f, 0f), Color.yellow, 1f, 0.25f),
			new PolylineVertex(new Vector3(0.5f, 0f, 0f), Color.blue, 1f, 0.5f),
			new PolylineVertex(new Vector3(0f, -0.5f, 0f), Color.cyan, 1f, 0.75f)
		};

		private Polyline _polyline;

		private Polyline polyline
		{
			get
			{
				if (_polyline == null)
				{
					_polyline = new Polyline(properties);
				}
				return _polyline;
			}
		}

		public override Drawable drawable => polyline;

		protected override void PreDraw()
		{
			base.PreDraw();
			if (itemsModificationId != polyline.itemsModificationId)
			{
				polyline.count = items.Length;
				for (int i = 0; i < items.Length; i++)
				{
					polyline[i] = items[i];
				}
				polyline.itemsModificationId = itemsModificationId;
			}
			if (propertiesModificationId != polyline.propertiesModificationId)
			{
				polyline.LoadSerializationData(properties);
				polyline.propertiesModificationId = propertiesModificationId;
			}
		}

		public static EditablePolyline CreateInstance()
		{
			return new GameObject("New EditablePolyline").AddComponent<EditablePolyline>();
		}
	}
}

using UnityEngine;

namespace Linefy
{
	public class Matrix4x4InspectorAttribute : PropertyAttribute
	{
		public bool showValuesGrid = true;

		public bool showInputFields;

		public Matrix4x4InspectorAttribute(bool showInputFields)
		{
			this.showInputFields = showInputFields;
		}

		public Matrix4x4InspectorAttribute()
		{
			showInputFields = true;
			showValuesGrid = true;
		}
	}
}

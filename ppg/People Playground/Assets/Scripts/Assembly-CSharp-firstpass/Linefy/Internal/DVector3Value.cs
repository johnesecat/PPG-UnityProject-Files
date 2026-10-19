using UnityEngine;

namespace Linefy.Internal
{
	public class DVector3Value : DValue<Vector3>
	{
		public DVector3Value(Vector3 initialValue, params DFlag[] dirtyFlags)
			: base(initialValue, dirtyFlags)
		{
		}

		public static implicit operator Vector3(DVector3Value v)
		{
			return v._value;
		}
	}
}

namespace Linefy.Internal
{
	public class DFloatValue : DValue<float>
	{
		public DFloatValue(float initialValue, params DFlag[] dirtyFlags)
			: base(initialValue, dirtyFlags)
		{
		}

		public static implicit operator float(DFloatValue v)
		{
			return v._value;
		}
	}
}

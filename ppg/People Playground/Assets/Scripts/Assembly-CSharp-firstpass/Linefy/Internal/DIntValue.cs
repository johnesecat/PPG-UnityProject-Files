namespace Linefy.Internal
{
	public class DIntValue : DValue<int>
	{
		public DIntValue(int initialValue, params DFlag[] dirtyFlags)
			: base(initialValue, dirtyFlags)
		{
		}

		public static implicit operator int(DIntValue v)
		{
			return v._value;
		}
	}
}

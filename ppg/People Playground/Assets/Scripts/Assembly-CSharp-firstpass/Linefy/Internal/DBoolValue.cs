namespace Linefy.Internal
{
	public class DBoolValue : DValue<bool>
	{
		public DBoolValue(bool initialValue, params DFlag[] dirtyFlags)
			: base(initialValue, dirtyFlags)
		{
		}

		public static implicit operator bool(DBoolValue v)
		{
			return v._value;
		}
	}
}

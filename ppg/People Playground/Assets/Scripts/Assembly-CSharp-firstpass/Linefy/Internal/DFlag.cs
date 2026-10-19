namespace Linefy.Internal
{
	public class DFlag
	{
		public string name;

		private bool _value;

		public DFlag(string name, bool initialValue)
		{
			this.name = name;
			_value = initialValue;
		}

		public void Set()
		{
			_value = true;
		}

		public void Reset()
		{
			_value = false;
		}

		public static implicit operator bool(DFlag df)
		{
			return df._value;
		}

		public override string ToString()
		{
			return name + (_value ? "[dirty]" : "[NOT dirty]");
		}
	}
}

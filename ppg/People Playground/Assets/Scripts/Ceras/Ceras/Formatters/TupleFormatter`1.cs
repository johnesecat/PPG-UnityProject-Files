using System;

namespace Ceras.Formatters
{
	internal sealed class TupleFormatter<T1> : IFormatter<Tuple<T1>>, IFormatter
	{
		private IFormatter<T1> _item1Formatter;

		public void Serialize(ref byte[] buffer, ref int offset, Tuple<T1> value)
		{
			_item1Formatter.Serialize(ref buffer, ref offset, value.Item1);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref Tuple<T1> value)
		{
			T1 value2 = default(T1);
			_item1Formatter.Deserialize(buffer, ref offset, ref value2);
			value = new Tuple<T1>(value2);
		}
	}
}

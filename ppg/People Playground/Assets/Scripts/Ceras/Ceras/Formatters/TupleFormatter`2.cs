using System;

namespace Ceras.Formatters
{
	internal sealed class TupleFormatter<T1, T2> : IFormatter<Tuple<T1, T2>>, IFormatter
	{
		private IFormatter<T1> _item1Formatter;

		private IFormatter<T2> _item2Formatter;

		public void Serialize(ref byte[] buffer, ref int offset, Tuple<T1, T2> value)
		{
			_item1Formatter.Serialize(ref buffer, ref offset, value.Item1);
			_item2Formatter.Serialize(ref buffer, ref offset, value.Item2);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref Tuple<T1, T2> value)
		{
			T1 value2 = default(T1);
			T2 value3 = default(T2);
			_item1Formatter.Deserialize(buffer, ref offset, ref value2);
			_item2Formatter.Deserialize(buffer, ref offset, ref value3);
			value = new Tuple<T1, T2>(value2, value3);
		}
	}
}

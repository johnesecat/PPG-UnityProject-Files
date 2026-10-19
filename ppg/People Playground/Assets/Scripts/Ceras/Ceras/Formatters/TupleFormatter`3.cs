using System;

namespace Ceras.Formatters
{
	internal sealed class TupleFormatter<T1, T2, T3> : IFormatter<Tuple<T1, T2, T3>>, IFormatter
	{
		private IFormatter<T1> _item1Formatter;

		private IFormatter<T2> _item2Formatter;

		private IFormatter<T3> _item3Formatter;

		public void Serialize(ref byte[] buffer, ref int offset, Tuple<T1, T2, T3> value)
		{
			_item1Formatter.Serialize(ref buffer, ref offset, value.Item1);
			_item2Formatter.Serialize(ref buffer, ref offset, value.Item2);
			_item3Formatter.Serialize(ref buffer, ref offset, value.Item3);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref Tuple<T1, T2, T3> value)
		{
			T1 value2 = default(T1);
			T2 value3 = default(T2);
			T3 value4 = default(T3);
			_item1Formatter.Deserialize(buffer, ref offset, ref value2);
			_item2Formatter.Deserialize(buffer, ref offset, ref value3);
			_item3Formatter.Deserialize(buffer, ref offset, ref value4);
			value = new Tuple<T1, T2, T3>(value2, value3, value4);
		}
	}
}

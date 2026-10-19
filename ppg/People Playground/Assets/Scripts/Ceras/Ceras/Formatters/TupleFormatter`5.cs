using System;

namespace Ceras.Formatters
{
	internal sealed class TupleFormatter<T1, T2, T3, T4, T5> : IFormatter<Tuple<T1, T2, T3, T4, T5>>, IFormatter
	{
		private IFormatter<T1> _item1Formatter;

		private IFormatter<T2> _item2Formatter;

		private IFormatter<T3> _item3Formatter;

		private IFormatter<T4> _item4Formatter;

		private IFormatter<T5> _item5Formatter;

		public void Serialize(ref byte[] buffer, ref int offset, Tuple<T1, T2, T3, T4, T5> value)
		{
			_item1Formatter.Serialize(ref buffer, ref offset, value.Item1);
			_item2Formatter.Serialize(ref buffer, ref offset, value.Item2);
			_item3Formatter.Serialize(ref buffer, ref offset, value.Item3);
			_item4Formatter.Serialize(ref buffer, ref offset, value.Item4);
			_item5Formatter.Serialize(ref buffer, ref offset, value.Item5);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref Tuple<T1, T2, T3, T4, T5> value)
		{
			T1 value2 = default(T1);
			T2 value3 = default(T2);
			T3 value4 = default(T3);
			T4 value5 = default(T4);
			T5 value6 = default(T5);
			_item1Formatter.Deserialize(buffer, ref offset, ref value2);
			_item2Formatter.Deserialize(buffer, ref offset, ref value3);
			_item3Formatter.Deserialize(buffer, ref offset, ref value4);
			_item4Formatter.Deserialize(buffer, ref offset, ref value5);
			_item5Formatter.Deserialize(buffer, ref offset, ref value6);
			value = new Tuple<T1, T2, T3, T4, T5>(value2, value3, value4, value5, value6);
		}
	}
}

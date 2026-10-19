using System;

namespace Ceras.Formatters
{
	internal sealed class ValueTupleFormatter<T1, T2, T3, T4, T5, T6, T7, TRest> : IFormatter<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>>, IFormatter where TRest : struct
	{
		private IFormatter<T1> _item1Formatter;

		private IFormatter<T2> _item2Formatter;

		private IFormatter<T3> _item3Formatter;

		private IFormatter<T4> _item4Formatter;

		private IFormatter<T5> _item5Formatter;

		private IFormatter<T6> _item6Formatter;

		private IFormatter<T7> _item7Formatter;

		private IFormatter<TRest> _restFormatter;

		public void Serialize(ref byte[] buffer, ref int offset, ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value)
		{
			_item1Formatter.Serialize(ref buffer, ref offset, value.Item1);
			_item2Formatter.Serialize(ref buffer, ref offset, value.Item2);
			_item3Formatter.Serialize(ref buffer, ref offset, value.Item3);
			_item4Formatter.Serialize(ref buffer, ref offset, value.Item4);
			_item5Formatter.Serialize(ref buffer, ref offset, value.Item5);
			_item6Formatter.Serialize(ref buffer, ref offset, value.Item6);
			_item7Formatter.Serialize(ref buffer, ref offset, value.Item7);
			_restFormatter.Serialize(ref buffer, ref offset, value.Rest);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value)
		{
			T1 value2 = default(T1);
			T2 value3 = default(T2);
			T3 value4 = default(T3);
			T4 value5 = default(T4);
			T5 value6 = default(T5);
			T6 value7 = default(T6);
			T7 value8 = default(T7);
			TRest value9 = default(TRest);
			_item1Formatter.Deserialize(buffer, ref offset, ref value2);
			_item2Formatter.Deserialize(buffer, ref offset, ref value3);
			_item3Formatter.Deserialize(buffer, ref offset, ref value4);
			_item4Formatter.Deserialize(buffer, ref offset, ref value5);
			_item5Formatter.Deserialize(buffer, ref offset, ref value6);
			_item6Formatter.Deserialize(buffer, ref offset, ref value7);
			_item7Formatter.Deserialize(buffer, ref offset, ref value8);
			_restFormatter.Deserialize(buffer, ref offset, ref value9);
			value = new ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(value2, value3, value4, value5, value6, value7, value8, value9);
		}
	}
}

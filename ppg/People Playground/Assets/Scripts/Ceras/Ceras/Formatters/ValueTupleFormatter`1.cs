using System;

namespace Ceras.Formatters
{
	internal sealed class ValueTupleFormatter<T1> : IFormatter<ValueTuple<T1>>, IFormatter
	{
		private IFormatter<T1> _item1Formatter;

		public void Serialize(ref byte[] buffer, ref int offset, ValueTuple<T1> value)
		{
			_item1Formatter.Serialize(ref buffer, ref offset, value.Item1);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref ValueTuple<T1> value)
		{
			T1 value2 = default(T1);
			_item1Formatter.Deserialize(buffer, ref offset, ref value2);
			value = new ValueTuple<T1>(value2);
		}
	}
}

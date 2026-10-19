using System.Collections.Generic;

namespace Ceras.Formatters
{
	public sealed class StackFormatter<TItem> : IFormatter<Stack<TItem>>, IFormatter
	{
		private IFormatter<int> _intFormatter;

		private IFormatter<TItem> _itemFormatter;

		public StackFormatter()
		{
			CerasSerializer.AddFormatterConstructedType(typeof(Stack<TItem>));
		}

		public void Serialize(ref byte[] buffer, ref int offset, Stack<TItem> value)
		{
			_intFormatter.Serialize(ref buffer, ref offset, value.Count);
			IFormatter<TItem> itemFormatter = _itemFormatter;
			foreach (TItem item in value)
			{
				itemFormatter.Serialize(ref buffer, ref offset, item);
			}
		}

		public void Deserialize(byte[] buffer, ref int offset, ref Stack<TItem> value)
		{
			IFormatter<TItem> itemFormatter = _itemFormatter;
			int value2 = 0;
			_intFormatter.Deserialize(buffer, ref offset, ref value2);
			value = new Stack<TItem>(value2);
			TItem[] array = new TItem[value2];
			for (int i = 0; i < value2; i++)
			{
				itemFormatter.Deserialize(buffer, ref offset, ref array[i]);
			}
			for (int num = value2 - 1; num >= 0; num--)
			{
				value.Push(array[num]);
			}
		}
	}
}

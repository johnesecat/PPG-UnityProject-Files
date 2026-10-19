using System.Collections.Generic;

namespace Ceras.Formatters
{
	public sealed class QueueFormatter<TItem> : IFormatter<Queue<TItem>>, IFormatter
	{
		private IFormatter<int> _intFormatter;

		private IFormatter<TItem> _itemFormatter;

		public QueueFormatter()
		{
			CerasSerializer.AddFormatterConstructedType(typeof(Queue<TItem>));
		}

		public void Serialize(ref byte[] buffer, ref int offset, Queue<TItem> value)
		{
			_intFormatter.Serialize(ref buffer, ref offset, value.Count);
			IFormatter<TItem> itemFormatter = _itemFormatter;
			foreach (TItem item in value)
			{
				itemFormatter.Serialize(ref buffer, ref offset, item);
			}
		}

		public void Deserialize(byte[] buffer, ref int offset, ref Queue<TItem> value)
		{
			IFormatter<TItem> itemFormatter = _itemFormatter;
			int value2 = 0;
			_intFormatter.Deserialize(buffer, ref offset, ref value2);
			value = new Queue<TItem>(value2);
			for (int i = 0; i < value2; i++)
			{
				TItem value3 = default(TItem);
				itemFormatter.Deserialize(buffer, ref offset, ref value3);
				value.Enqueue(value3);
			}
		}
	}
}

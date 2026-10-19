using System.Collections.Generic;

namespace Ceras.Formatters
{
	public abstract class CollectionByProxyFormatter<TCollection, TItem, TProxyCollection> : IFormatter<TCollection>, IFormatter where TCollection : ICollection<TItem>
	{
		protected IFormatter<TItem> _itemFormatter;

		protected CollectionByProxyFormatter()
		{
			CerasSerializer.AddFormatterConstructedType(typeof(TCollection));
		}

		public void Serialize(ref byte[] buffer, ref int offset, TCollection value)
		{
			SerializerBinary.WriteUInt32(ref buffer, ref offset, (uint)value.Count);
			IFormatter<TItem> itemFormatter = _itemFormatter;
			IEnumerator<TItem> enumerator = value.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					itemFormatter.Serialize(ref buffer, ref offset, enumerator.Current);
				}
			}
			finally
			{
				enumerator.Dispose();
			}
		}

		public void Deserialize(byte[] buffer, ref int offset, ref TCollection value)
		{
			int num = (int)SerializerBinary.ReadUInt32(buffer, ref offset);
			TProxyCollection builder = CreateProxy(num);
			for (int i = 0; i < num; i++)
			{
				TItem value2 = default(TItem);
				_itemFormatter.Deserialize(buffer, ref offset, ref value2);
				AddToProxy(builder, value2);
			}
			Finalize(builder, ref value);
		}

		protected abstract TProxyCollection CreateProxy(int knownSize);

		protected abstract void AddToProxy(TProxyCollection builder, TItem item);

		protected abstract void Finalize(TProxyCollection builder, ref TCollection collection);
	}
}

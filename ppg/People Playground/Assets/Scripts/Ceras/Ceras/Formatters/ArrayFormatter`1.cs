using System;

namespace Ceras.Formatters
{
	public sealed class ArrayFormatter<TItem> : IFormatter<TItem[]>, IFormatter
	{
		private readonly IFormatter<TItem> _itemFormatter;

		private readonly uint _maxCount;

		public ArrayFormatter(CerasSerializer serializer, uint maxCount)
		{
			_maxCount = maxCount;
			Type typeFromHandle = typeof(TItem);
			_itemFormatter = (IFormatter<TItem>)serializer.GetReferenceFormatter(typeFromHandle);
		}

		public void Serialize(ref byte[] buffer, ref int offset, TItem[] ar)
		{
			if (ar == null)
			{
				SerializerBinary.WriteUInt32Bias(ref buffer, ref offset, -1, 1);
				return;
			}
			SerializerBinary.WriteUInt32Bias(ref buffer, ref offset, ar.Length, 1);
			IFormatter<TItem> itemFormatter = _itemFormatter;
			for (int i = 0; i < ar.Length; i++)
			{
				itemFormatter.Serialize(ref buffer, ref offset, ar[i]);
			}
		}

		public void Deserialize(byte[] buffer, ref int offset, ref TItem[] ar)
		{
			int num = SerializerBinary.ReadUInt32Bias(buffer, ref offset, 1);
			if (num == -1)
			{
				ar = null;
				return;
			}
			if (num > _maxCount)
			{
				throw new InvalidOperationException($"The data contains a '{typeof(TItem)}'-array of size '{num}', which exceeds the allowed limit of '{_maxCount}'");
			}
			if (ar == null || ar.Length != num)
			{
				ar = new TItem[num];
			}
			IFormatter<TItem> itemFormatter = _itemFormatter;
			for (int i = 0; i < num; i++)
			{
				itemFormatter.Deserialize(buffer, ref offset, ref ar[i]);
			}
		}
	}
}

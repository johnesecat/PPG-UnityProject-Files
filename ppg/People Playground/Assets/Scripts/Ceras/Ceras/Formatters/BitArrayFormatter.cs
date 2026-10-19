using System.Collections;

namespace Ceras.Formatters
{
	internal class BitArrayFormatter : IFormatter<BitArray>, IFormatter
	{
		[CerasNoReference]
		private IFormatter<int[]> _intFormatter;

		public BitArrayFormatter()
		{
			CerasSerializer.AddFormatterConstructedType(typeof(BitArray));
		}

		public void Serialize(ref byte[] buffer, ref int offset, BitArray value)
		{
			int count = value.Count;
			SerializerBinary.WriteUInt32(ref buffer, ref offset, (uint)count);
			int[] array = new int[count / 32 + 1];
			value.CopyTo(array, 0);
			_intFormatter.Serialize(ref buffer, ref offset, array);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref BitArray value)
		{
			int length = (int)SerializerBinary.ReadUInt32(buffer, ref offset);
			int[] value2 = null;
			_intFormatter.Deserialize(buffer, ref offset, ref value2);
			value = new BitArray(value2);
			value.Length = length;
		}
	}
}

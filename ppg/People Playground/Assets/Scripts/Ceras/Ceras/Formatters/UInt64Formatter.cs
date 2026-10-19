namespace Ceras.Formatters
{
	internal sealed class UInt64Formatter : IFormatter<ulong>, IFormatter
	{
		public void Serialize(ref byte[] buffer, ref int offset, ulong value)
		{
			SerializerBinary.WriteInt64Fixed(ref buffer, ref offset, (long)value);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref ulong value)
		{
			value = (ulong)SerializerBinary.ReadInt64Fixed(buffer, ref offset);
		}
	}
}

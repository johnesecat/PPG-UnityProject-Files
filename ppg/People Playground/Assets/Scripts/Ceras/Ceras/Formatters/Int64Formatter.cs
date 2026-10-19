namespace Ceras.Formatters
{
	internal sealed class Int64Formatter : IFormatter<long>, IFormatter
	{
		public void Serialize(ref byte[] buffer, ref int offset, long value)
		{
			SerializerBinary.WriteInt64Fixed(ref buffer, ref offset, value);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref long value)
		{
			value = SerializerBinary.ReadInt64Fixed(buffer, ref offset);
		}
	}
}

namespace Ceras.Formatters
{
	internal sealed class ByteFormatter : IFormatter<byte>, IFormatter
	{
		public void Serialize(ref byte[] buffer, ref int offset, byte value)
		{
			SerializerBinary.WriteByte(ref buffer, ref offset, value);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref byte value)
		{
			value = SerializerBinary.ReadByte(buffer, ref offset);
		}
	}
}

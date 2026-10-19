namespace Ceras.Formatters
{
	internal sealed class UInt32Formatter : IFormatter<uint>, IFormatter
	{
		public void Serialize(ref byte[] buffer, ref int offset, uint value)
		{
			SerializerBinary.WriteUInt32(ref buffer, ref offset, value);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref uint value)
		{
			value = SerializerBinary.ReadUInt32(buffer, ref offset);
		}
	}
}

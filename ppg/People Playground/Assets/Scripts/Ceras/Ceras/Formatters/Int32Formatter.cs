namespace Ceras.Formatters
{
	internal sealed class Int32Formatter : IFormatter<int>, IFormatter
	{
		public void Serialize(ref byte[] buffer, ref int offset, int value)
		{
			SerializerBinary.WriteInt32(ref buffer, ref offset, value);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref int value)
		{
			value = SerializerBinary.ReadInt32(buffer, ref offset);
		}
	}
}

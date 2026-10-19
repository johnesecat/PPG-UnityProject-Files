namespace Ceras.Formatters
{
	internal sealed class Int16Formatter : IFormatter<short>, IFormatter
	{
		public void Serialize(ref byte[] buffer, ref int offset, short value)
		{
			SerializerBinary.WriteInt16Fixed(ref buffer, ref offset, value);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref short value)
		{
			value = SerializerBinary.ReadInt16Fixed(buffer, ref offset);
		}
	}
}

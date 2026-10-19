namespace Ceras.Formatters
{
	internal sealed class CharFormatter : IFormatter<char>, IFormatter
	{
		public void Serialize(ref byte[] buffer, ref int offset, char value)
		{
			SerializerBinary.WriteInt16Fixed(ref buffer, ref offset, (short)value);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref char value)
		{
			value = (char)SerializerBinary.ReadInt16Fixed(buffer, ref offset);
		}
	}
}

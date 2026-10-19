namespace Ceras.Formatters
{
	internal sealed class SByteFormatter : IFormatter<sbyte>, IFormatter
	{
		public void Serialize(ref byte[] buffer, ref int offset, sbyte value)
		{
			SerializerBinary.WriteByte(ref buffer, ref offset, (byte)value);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref sbyte value)
		{
			value = (sbyte)SerializerBinary.ReadByte(buffer, ref offset);
		}
	}
}

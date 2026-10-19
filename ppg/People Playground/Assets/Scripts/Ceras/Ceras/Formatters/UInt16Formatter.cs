namespace Ceras.Formatters
{
	internal sealed class UInt16Formatter : IFormatter<ushort>, IFormatter
	{
		public void Serialize(ref byte[] buffer, ref int offset, ushort value)
		{
			SerializerBinary.WriteInt16Fixed(ref buffer, ref offset, (short)value);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref ushort value)
		{
			value = (ushort)SerializerBinary.ReadInt16Fixed(buffer, ref offset);
		}
	}
}

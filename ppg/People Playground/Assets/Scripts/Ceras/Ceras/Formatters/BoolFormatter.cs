namespace Ceras.Formatters
{
	internal sealed class BoolFormatter : IFormatter<bool>, IFormatter
	{
		public void Serialize(ref byte[] buffer, ref int offset, bool value)
		{
			SerializerBinary.WriteInt32(ref buffer, ref offset, value ? 1 : 0);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref bool value)
		{
			value = SerializerBinary.ReadInt32(buffer, ref offset) != 0;
		}
	}
}

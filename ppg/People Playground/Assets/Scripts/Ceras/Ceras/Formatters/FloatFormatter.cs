namespace Ceras.Formatters
{
	internal sealed class FloatFormatter : IFormatter<float>, IFormatter
	{
		public void Serialize(ref byte[] buffer, ref int offset, float value)
		{
			SerializerBinary.WriteFloat32Fixed(ref buffer, ref offset, value);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref float value)
		{
			value = SerializerBinary.ReadFloat32Fixed(buffer, ref offset);
		}
	}
}

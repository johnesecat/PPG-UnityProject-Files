namespace Ceras.Formatters
{
	internal sealed class DoubleFormatter : IFormatter<double>, IFormatter
	{
		public void Serialize(ref byte[] buffer, ref int offset, double value)
		{
			SerializerBinary.WriteDouble64Fixed(ref buffer, ref offset, value);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref double value)
		{
			value = SerializerBinary.ReadDouble64Fixed(buffer, ref offset);
		}
	}
}

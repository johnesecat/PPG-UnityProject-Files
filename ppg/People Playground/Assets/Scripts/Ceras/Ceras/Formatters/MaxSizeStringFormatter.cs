namespace Ceras.Formatters
{
	internal sealed class MaxSizeStringFormatter : IFormatter<string>, IFormatter
	{
		private readonly uint _maxLength;

		public MaxSizeStringFormatter(uint maxLength)
		{
			_maxLength = maxLength;
		}

		public void Serialize(ref byte[] buffer, ref int offset, string value)
		{
			SerializerBinary.WriteString(ref buffer, ref offset, value);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref string value)
		{
			value = SerializerBinary.ReadStringLimited(buffer, ref offset, _maxLength);
		}
	}
}

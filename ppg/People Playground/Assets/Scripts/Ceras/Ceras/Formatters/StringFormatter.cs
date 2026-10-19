namespace Ceras.Formatters
{
	public sealed class StringFormatter : IFormatter<string>, IFormatter
	{
		public static SerializeDelegate<string> SerializeOverride;

		public static DeserializeDelegate<string> DeserializeOverride;

		public void Serialize(ref byte[] buffer, ref int offset, string value)
		{
			if (SerializeOverride != null)
			{
				SerializeOverride(ref buffer, ref offset, value);
			}
			else
			{
				SerializerBinary.WriteString(ref buffer, ref offset, value);
			}
		}

		public void Deserialize(byte[] buffer, ref int offset, ref string value)
		{
			if (DeserializeOverride != null)
			{
				DeserializeOverride(buffer, ref offset, ref value);
			}
			else
			{
				value = SerializerBinary.ReadString(buffer, ref offset);
			}
		}
	}
}

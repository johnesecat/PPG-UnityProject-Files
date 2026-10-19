using System;

namespace Ceras.Formatters
{
	internal class UriFormatter : IFormatter<Uri>, IFormatter
	{
		public UriFormatter()
		{
			CerasSerializer.AddFormatterConstructedType(typeof(Uri));
		}

		public void Serialize(ref byte[] buffer, ref int offset, Uri value)
		{
			SerializerBinary.WriteString(ref buffer, ref offset, value.OriginalString);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref Uri value)
		{
			string text = SerializerBinary.ReadString(buffer, ref offset);
			if (text == null)
			{
				value = null;
			}
			else
			{
				value = new Uri(text, UriKind.RelativeOrAbsolute);
			}
		}
	}
}

using System.Drawing;

namespace Ceras.Formatters
{
	internal class ColorFormatter : IFormatter<Color>, IFormatter
	{
		public void Serialize(ref byte[] buffer, ref int offset, Color value)
		{
			SerializerBinary.WriteInt32Fixed(ref buffer, ref offset, value.ToArgb());
		}

		public void Deserialize(byte[] buffer, ref int offset, ref Color value)
		{
			int argb = SerializerBinary.ReadInt32Fixed(buffer, ref offset);
			value = Color.FromArgb(argb);
		}
	}
}

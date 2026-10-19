using System;

namespace Ceras.Formatters
{
	internal sealed class IntPtrFormatter : IFormatter<IntPtr>, IFormatter
	{
		public void Serialize(ref byte[] buffer, ref int offset, IntPtr IntPtr)
		{
			SerializerBinary.WriteInt64Fixed(ref buffer, ref offset, IntPtr.ToInt64());
		}

		public void Deserialize(byte[] buffer, ref int offset, ref IntPtr value)
		{
			value = new IntPtr(SerializerBinary.ReadInt64Fixed(buffer, ref offset));
		}
	}
}

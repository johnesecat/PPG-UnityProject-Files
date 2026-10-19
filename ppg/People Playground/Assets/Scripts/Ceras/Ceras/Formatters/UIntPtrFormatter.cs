using System;

namespace Ceras.Formatters
{
	internal sealed class UIntPtrFormatter : IFormatter<UIntPtr>, IFormatter
	{
		public void Serialize(ref byte[] buffer, ref int offset, UIntPtr IntPtr)
		{
			SerializerBinary.WriteInt64Fixed(ref buffer, ref offset, (long)IntPtr.ToUInt64());
		}

		public void Deserialize(byte[] buffer, ref int offset, ref UIntPtr value)
		{
			value = new UIntPtr((ulong)SerializerBinary.ReadInt64Fixed(buffer, ref offset));
		}
	}
}

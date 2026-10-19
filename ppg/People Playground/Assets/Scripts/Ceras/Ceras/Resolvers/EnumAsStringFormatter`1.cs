using System;
using Ceras.Formatters;

namespace Ceras.Resolvers
{
	public sealed class EnumAsStringFormatter<T> : IFormatter<T>, IFormatter where T : Enum
	{
		private ISizeLimitsConfig _sizeLimits;

		public void Serialize(ref byte[] buffer, ref int offset, T value)
		{
			string value2 = value.ToString();
			SerializerBinary.WriteString(ref buffer, ref offset, value2);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref T value)
		{
			uint maxStringLength = _sizeLimits.MaxStringLength;
			string value2 = SerializerBinary.ReadStringLimited(buffer, ref offset, maxStringLength);
			value = (T)Enum.Parse(typeof(T), value2);
		}
	}
}

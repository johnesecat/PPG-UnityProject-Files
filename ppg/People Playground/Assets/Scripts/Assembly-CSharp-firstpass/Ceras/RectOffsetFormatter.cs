using Ceras.Formatters;
using UnityEngine;

namespace Ceras
{
	internal class RectOffsetFormatter : IFormatter<RectOffset>, IFormatter
	{
		public void Serialize(ref byte[] buffer, ref int offset, RectOffset value)
		{
			SerializerBinary.WriteInt32Fixed(ref buffer, ref offset, value.left);
			SerializerBinary.WriteInt32Fixed(ref buffer, ref offset, value.right);
			SerializerBinary.WriteInt32Fixed(ref buffer, ref offset, value.top);
			SerializerBinary.WriteInt32Fixed(ref buffer, ref offset, value.bottom);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref RectOffset value)
		{
			value.left = SerializerBinary.ReadInt32Fixed(buffer, ref offset);
			value.right = SerializerBinary.ReadInt32Fixed(buffer, ref offset);
			value.top = SerializerBinary.ReadInt32Fixed(buffer, ref offset);
			value.bottom = SerializerBinary.ReadInt32Fixed(buffer, ref offset);
		}
	}
}

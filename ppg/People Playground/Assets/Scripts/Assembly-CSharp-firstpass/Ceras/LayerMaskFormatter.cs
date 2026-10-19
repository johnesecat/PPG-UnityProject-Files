using Ceras.Formatters;
using UnityEngine;

namespace Ceras
{
	internal class LayerMaskFormatter : IFormatter<LayerMask>, IFormatter
	{
		public void Serialize(ref byte[] buffer, ref int offset, LayerMask value)
		{
			SerializerBinary.WriteInt32Fixed(ref buffer, ref offset, value.value);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref LayerMask value)
		{
			value.value = SerializerBinary.ReadInt32Fixed(buffer, ref offset);
		}
	}
}

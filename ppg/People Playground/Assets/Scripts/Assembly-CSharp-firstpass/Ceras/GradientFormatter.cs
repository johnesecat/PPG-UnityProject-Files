using Ceras.Formatters;
using Ceras.Resolvers;
using UnityEngine;

namespace Ceras
{
	internal class GradientFormatter : IFormatter<Gradient>, IFormatter
	{
		private IFormatter<GradientAlphaKey[]> _alphaKeysFormatter;

		private IFormatter<GradientColorKey[]> _colorKeysFormatter;

		private EnumFormatter<GradientMode> _gradientModeFormatter;

		public void Serialize(ref byte[] buffer, ref int offset, Gradient value)
		{
			_alphaKeysFormatter.Serialize(ref buffer, ref offset, value.alphaKeys);
			_colorKeysFormatter.Serialize(ref buffer, ref offset, value.colorKeys);
			_gradientModeFormatter.Serialize(ref buffer, ref offset, value.mode);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref Gradient value)
		{
			GradientAlphaKey[] value2 = value.alphaKeys;
			_alphaKeysFormatter.Deserialize(buffer, ref offset, ref value2);
			value.alphaKeys = value2;
			GradientColorKey[] value3 = value.colorKeys;
			_colorKeysFormatter.Deserialize(buffer, ref offset, ref value3);
			value.colorKeys = value3;
			GradientMode value4 = value.mode;
			_gradientModeFormatter.Deserialize(buffer, ref offset, ref value4);
			value.mode = value4;
		}
	}
}

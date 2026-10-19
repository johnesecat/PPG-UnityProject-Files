using Ceras.Formatters;
using Ceras.Resolvers;
using UnityEngine;

namespace Ceras
{
	internal class AnimationCurveFormatter : IFormatter<AnimationCurve>, IFormatter
	{
		private IFormatter<Keyframe> _keyframeFormatter;

		private EnumFormatter<WrapMode> _wrapModeFormatter;

		public void Serialize(ref byte[] buffer, ref int offset, AnimationCurve value)
		{
			Keyframe[] keys = value.keys;
			SerializerBinary.WriteInt32(ref buffer, ref offset, keys.Length);
			for (int i = 0; i < keys.Length; i++)
			{
				_keyframeFormatter.Serialize(ref buffer, ref offset, keys[i]);
			}
			_wrapModeFormatter.Serialize(ref buffer, ref offset, value.preWrapMode);
			_wrapModeFormatter.Serialize(ref buffer, ref offset, value.postWrapMode);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref AnimationCurve value)
		{
			int num = SerializerBinary.ReadInt32(buffer, ref offset);
			Keyframe[] array = new Keyframe[num];
			for (int i = 0; i < num; i++)
			{
				_keyframeFormatter.Deserialize(buffer, ref offset, ref array[i]);
			}
			if (value == null)
			{
				value = new AnimationCurve(array);
			}
			else
			{
				value.keys = array;
			}
			WrapMode value2 = WrapMode.Default;
			_wrapModeFormatter.Deserialize(buffer, ref offset, ref value2);
			value.preWrapMode = value2;
			_wrapModeFormatter.Deserialize(buffer, ref offset, ref value2);
			value.postWrapMode = value2;
		}
	}
}

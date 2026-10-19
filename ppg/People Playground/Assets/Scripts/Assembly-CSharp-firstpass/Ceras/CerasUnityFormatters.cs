using System;
using System.Collections.Generic;
using Ceras.Formatters;
using UnityEngine;

namespace Ceras
{
	public static class CerasUnityFormatters
	{
		private static Type[] _blittableTypes = new Type[22]
		{
			typeof(Vector2),
			typeof(Vector3),
			typeof(Vector4),
			typeof(Quaternion),
			typeof(Matrix4x4),
			typeof(Color),
			typeof(Color32),
			typeof(Bounds),
			typeof(Rect),
			typeof(RangeInt),
			typeof(Plane),
			typeof(Ray),
			typeof(Ray2D),
			typeof(BoundingSphere),
			typeof(GradientAlphaKey),
			typeof(GradientColorKey),
			typeof(Keyframe),
			typeof(Hash128),
			typeof(Vector2Int),
			typeof(Vector3Int),
			typeof(RectInt),
			typeof(BoundsInt)
		};

		private static Dictionary<Type, Type> _typeToFormatterType = new Dictionary<Type, Type>
		{
			{
				typeof(AnimationCurve),
				typeof(AnimationCurveFormatter)
			},
			{
				typeof(RectOffset),
				typeof(RectOffsetFormatter)
			},
			{
				typeof(Gradient),
				typeof(GradientFormatter)
			},
			{
				typeof(LayerMask),
				typeof(LayerMaskFormatter)
			}
		};

		public static void ApplyToConfig(SerializerConfig config)
		{
			Type[] blittableTypes = _blittableTypes;
			foreach (Type type in blittableTypes)
			{
				config.ConfigType(type).CustomResolver = ForceReinterpret;
			}
			blittableTypes = _blittableTypes;
			for (int i = 0; i < blittableTypes.Length; i++)
			{
				Type type2 = blittableTypes[i].MakeArrayType();
				config.ConfigType(type2).CustomResolver = ForceReinterpretArray;
			}
			config.OnResolveFormatter.Add(GetFormatter);
		}

		private static IFormatter GetFormatter(CerasSerializer ceras, Type typeToBeFormatted)
		{
			if (_typeToFormatterType.TryGetValue(typeToBeFormatted, out var value))
			{
				return (IFormatter)Activator.CreateInstance(value);
			}
			return null;
		}

		private static IFormatter ForceReinterpret(CerasSerializer ceras, Type typeToBeFormatted)
		{
			return (IFormatter)Activator.CreateInstance(typeof(ReinterpretFormatter<>).MakeGenericType(typeToBeFormatted));
		}

		private static IFormatter ForceReinterpretArray(CerasSerializer ceras, Type typeToBeFormatted)
		{
			Type elementType = typeToBeFormatted.GetElementType();
			uint num = ((elementType == typeof(byte)) ? ceras.GetConfig().Advanced.SizeLimits.MaxByteArraySize : ceras.GetConfig().Advanced.SizeLimits.MaxArraySize);
			return (IFormatter)Activator.CreateInstance(typeof(ReinterpretArrayFormatter<>).MakeGenericType(elementType), num);
		}
	}
}

using System;
using System.Reflection;
using Ceras.Helpers;

namespace Ceras.Formatters
{
	internal class MemberInfoFormatter<T> : IFormatter<T>, IFormatter where T : MemberInfo
	{
		private IFormatter<string> _stringFormatter;

		private IFormatter<Type> _typeFormatter;

		private const BindingFlags BindingAllStatic = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private const BindingFlags BindingAllInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		public MemberInfoFormatter(CerasSerializer serializer)
		{
			_stringFormatter = serializer.GetFormatter<string>();
			_typeFormatter = (IFormatter<Type>)serializer.GetSpecificFormatter(typeof(Type));
		}

		public void Serialize(ref byte[] buffer, ref int offset, T member)
		{
			_typeFormatter.Serialize(ref buffer, ref offset, member.DeclaringType);
			byte b = 0;
			switch (member.MemberType)
			{
			case MemberTypes.Constructor:
			case MemberTypes.Method:
			{
				MethodBase methodBase = (MethodBase)(object)member;
				if (methodBase.ContainsGenericParameters)
				{
					throw new ArgumentException("The method or constructor '" + methodBase.DeclaringType?.FullName + "." + methodBase.Name + "' can not be serialized because it is not closed. If you need this functionality (or don't know what this means) please report an issue on GitHub.");
				}
				b = PackBindingData(methodBase.IsStatic, ReflectionTypeToCeras(member.MemberType));
				SerializerBinary.WriteByte(ref buffer, ref offset, b);
				_stringFormatter.Serialize(ref buffer, ref offset, methodBase.Name);
				ParameterInfo[] parameters = methodBase.GetParameters();
				SerializerBinary.WriteInt32(ref buffer, ref offset, parameters.Length);
				for (int i = 0; i < parameters.Length; i++)
				{
					_typeFormatter.Serialize(ref buffer, ref offset, parameters[i].ParameterType);
				}
				break;
			}
			case MemberTypes.Property:
			{
				PropertyInfo propertyInfo = (PropertyInfo)(object)member;
				b = PackBindingData(propertyInfo.GetAccessors(nonPublic: true)[0].IsStatic, ReflectionTypeToCeras(member.MemberType));
				SerializerBinary.WriteByte(ref buffer, ref offset, b);
				_stringFormatter.Serialize(ref buffer, ref offset, propertyInfo.Name);
				_typeFormatter.Serialize(ref buffer, ref offset, propertyInfo.PropertyType);
				break;
			}
			case MemberTypes.Field:
			{
				FieldInfo fieldInfo = (FieldInfo)(object)member;
				b = PackBindingData(fieldInfo.IsStatic, ReflectionTypeToCeras(member.MemberType));
				SerializerBinary.WriteByte(ref buffer, ref offset, b);
				_stringFormatter.Serialize(ref buffer, ref offset, fieldInfo.Name);
				_typeFormatter.Serialize(ref buffer, ref offset, fieldInfo.FieldType);
				break;
			}
			default:
				throw new ArgumentOutOfRangeException(string.Concat("Cannot serialize member type '", member.MemberType, "'"));
			}
		}

		public void Deserialize(byte[] buffer, ref int offset, ref T member)
		{
			Type value = null;
			_typeFormatter.Deserialize(buffer, ref offset, ref value);
			UnpackBindingData(SerializerBinary.ReadByte(buffer, ref offset), out var isStatic, out var memberType);
			BindingFlags bindingAttr = (isStatic ? (BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) : (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
			string value2 = null;
			switch (memberType)
			{
			case MemberType.Constructor:
			case MemberType.Method:
			{
				_stringFormatter.Deserialize(buffer, ref offset, ref value2);
				int num = SerializerBinary.ReadInt32(buffer, ref offset);
				Type[] array = new Type[num];
				for (int i = 0; i < num; i++)
				{
					_typeFormatter.Deserialize(buffer, ref offset, ref array[i]);
				}
				if (memberType == MemberType.Constructor)
				{
					member = (T)(MemberInfo)value.GetConstructor(bindingAttr, null, array, null);
					break;
				}
				MethodInfo methodInfo = ReflectionHelper.ResolveMethod(value, value2, array);
				if (methodInfo != null)
				{
					member = (T)(MemberInfo)methodInfo;
					break;
				}
				throw new AmbiguousMatchException($"Can't resolve method named '{value2}' with '{num}' arguments.");
			}
			case MemberType.Field:
			case MemberType.Property:
			{
				_stringFormatter.Deserialize(buffer, ref offset, ref value2);
				Type value3 = null;
				_typeFormatter.Deserialize(buffer, ref offset, ref value3);
				if (memberType == MemberType.Field)
				{
					member = (T)(MemberInfo)value.GetField(value2, bindingAttr);
				}
				else
				{
					member = (T)(MemberInfo)value.GetProperty(value2, bindingAttr, null, value3, new Type[0], null);
				}
				break;
			}
			default:
				throw new ArgumentOutOfRangeException(string.Concat("Cannot deserialize member type '", memberType, "'"));
			}
		}

		private static MemberType ReflectionTypeToCeras(MemberTypes memberTypes)
		{
			if ((memberTypes & MemberTypes.Constructor) != 0)
			{
				return MemberType.Constructor;
			}
			if ((memberTypes & MemberTypes.Method) != 0)
			{
				return MemberType.Method;
			}
			if ((memberTypes & MemberTypes.Field) != 0)
			{
				return MemberType.Field;
			}
			if ((memberTypes & MemberTypes.Property) != 0)
			{
				return MemberType.Property;
			}
			throw new InvalidOperationException("MemberTypes enum is out of range");
		}

		private static byte PackBindingData(bool isStatic, MemberType memberType)
		{
			byte b = (byte)memberType;
			if (isStatic)
			{
				b = (byte)(b | 0x80u);
			}
			return b;
		}

		private static void UnpackBindingData(byte b, out bool isStatic, out MemberType memberType)
		{
			isStatic = (b & 0x80) != 0;
			memberType = (MemberType)(b & 0xFFFFFF7Fu);
		}
	}
}

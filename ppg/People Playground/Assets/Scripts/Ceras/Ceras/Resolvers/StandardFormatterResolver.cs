using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Ceras.Formatters;
using Ceras.Helpers;

namespace Ceras.Resolvers
{
	public sealed class StandardFormatterResolver : IFormatterResolver
	{
		private class NullableFormatter<T> : IFormatter<T?>, IFormatter where T : struct
		{
			private IFormatter<T> _specificFormatter;

			public NullableFormatter(CerasSerializer serializer)
			{
				if (!typeof(T).IsValueType)
				{
					throw new InvalidOperationException("Trying to create a 'NullableFormatter<>' for reference type '" + typeof(T).FullName + "'!");
				}
				_specificFormatter = (IFormatter<T>)serializer.GetSpecificFormatter(typeof(T));
			}

			public void Serialize(ref byte[] buffer, ref int offset, T? value)
			{
				if (value.HasValue)
				{
					SerializerBinary.WriteByte(ref buffer, ref offset, 1);
					_specificFormatter.Serialize(ref buffer, ref offset, value.Value);
				}
				else
				{
					SerializerBinary.WriteByte(ref buffer, ref offset, 0);
				}
			}

			public void Deserialize(byte[] buffer, ref int offset, ref T? value)
			{
				if (SerializerBinary.ReadByte(buffer, ref offset) != 0)
				{
					T value2 = default(T);
					_specificFormatter.Deserialize(buffer, ref offset, ref value2);
					value = value2;
				}
				else
				{
					value = null;
				}
			}
		}

		private class KeyValuePairFormatter<TKey, TValue> : IFormatter<KeyValuePair<TKey, TValue>>, IFormatter
		{
			private IFormatter<TKey> _keyFormatter;

			private IFormatter<TValue> _valueFormatter;

			public void Serialize(ref byte[] buffer, ref int offset, KeyValuePair<TKey, TValue> value)
			{
				_keyFormatter.Serialize(ref buffer, ref offset, value.Key);
				_valueFormatter.Serialize(ref buffer, ref offset, value.Value);
			}

			public void Deserialize(byte[] buffer, ref int offset, ref KeyValuePair<TKey, TValue> kvp)
			{
				TKey value = default(TKey);
				_keyFormatter.Deserialize(buffer, ref offset, ref value);
				TValue value2 = default(TValue);
				_valueFormatter.Deserialize(buffer, ref offset, ref value2);
				kvp = new KeyValuePair<TKey, TValue>(value, value2);
			}
		}

		private class DateTimeFormatter : IFormatter<DateTime>, IFormatter
		{
			public void Serialize(ref byte[] buffer, ref int offset, DateTime value)
			{
				long value2 = value.ToBinary();
				SerializerBinary.WriteInt64Fixed(ref buffer, ref offset, value2);
			}

			public void Deserialize(byte[] buffer, ref int offset, ref DateTime value)
			{
				long dateData = SerializerBinary.ReadInt64Fixed(buffer, ref offset);
				value = DateTime.FromBinary(dateData);
			}
		}

		private class DateTimeOffsetFormatter : IFormatter<DateTimeOffset>, IFormatter
		{
			public void Serialize(ref byte[] buffer, ref int offset, DateTimeOffset value)
			{
				SerializerBinary.WriteInt64Fixed(ref buffer, ref offset, value.Ticks);
				SerializerBinary.WriteInt16Fixed(ref buffer, ref offset, (short)value.Offset.TotalMinutes);
			}

			public void Deserialize(byte[] buffer, ref int offset, ref DateTimeOffset value)
			{
				long ticks = SerializerBinary.ReadInt64Fixed(buffer, ref offset);
				short num = SerializerBinary.ReadInt16Fixed(buffer, ref offset);
				value = new DateTimeOffset(ticks, TimeSpan.FromMinutes(num));
			}
		}

		private class TimeSpanFormatter : IFormatter<TimeSpan>, IFormatter
		{
			public void Serialize(ref byte[] buffer, ref int offset, TimeSpan value)
			{
				SerializerBinary.WriteInt64Fixed(ref buffer, ref offset, value.Ticks);
			}

			public void Deserialize(byte[] buffer, ref int offset, ref TimeSpan value)
			{
				value = new TimeSpan(SerializerBinary.ReadInt64Fixed(buffer, ref offset));
			}
		}

		private class BitVector32Formatter : IFormatter<BitVector32>, IFormatter
		{
			public void Serialize(ref byte[] buffer, ref int offset, BitVector32 value)
			{
				SerializerBinary.WriteInt32Fixed(ref buffer, ref offset, value.Data);
			}

			public void Deserialize(byte[] buffer, ref int offset, ref BitVector32 value)
			{
				int data = SerializerBinary.ReadInt32Fixed(buffer, ref offset);
				value = new BitVector32(data);
			}
		}

		private class BigIntegerFormatter : IFormatter<BigInteger>, IFormatter
		{
			public void Serialize(ref byte[] buffer, ref int offset, BigInteger value)
			{
				byte[] array = value.ToByteArray();
				SerializerBinary.WriteUInt32(ref buffer, ref offset, (uint)array.Length);
				SerializerBinary.EnsureCapacity(ref buffer, offset, array.Length);
				Buffer.BlockCopy(array, 0, buffer, offset, array.Length);
				offset += array.Length;
			}

			public void Deserialize(byte[] buffer, ref int offset, ref BigInteger value)
			{
				int num = (int)SerializerBinary.ReadUInt32(buffer, ref offset);
				byte[] array = new byte[num];
				Buffer.BlockCopy(buffer, offset, array, 0, num);
				offset += num;
				value = new BigInteger(array);
			}
		}

		private static readonly Type _iTupleInterface = typeof(Tuple<>).GetInterfaces().First((Type t) => t.Name == "ITuple");

		private static readonly Type[] _tupleFormatterTypes = new Type[8]
		{
			null,
			typeof(TupleFormatter<>),
			typeof(TupleFormatter<, >),
			typeof(TupleFormatter<, , >),
			typeof(TupleFormatter<, , , >),
			typeof(TupleFormatter<, , , , , >),
			typeof(TupleFormatter<, , , , , , >),
			typeof(TupleFormatter<, , , , , , , >)
		};

		private static readonly Type[] _valueTupleFormatterTypes = new Type[8]
		{
			null,
			typeof(ValueTupleFormatter<>),
			typeof(ValueTupleFormatter<, >),
			typeof(ValueTupleFormatter<, , >),
			typeof(ValueTupleFormatter<, , , >),
			typeof(ValueTupleFormatter<, , , , , >),
			typeof(ValueTupleFormatter<, , , , , , >),
			typeof(ValueTupleFormatter<, , , , , , , >)
		};

		private readonly TypeDictionary<IFormatter> _primitiveFormatters = new TypeDictionary<IFormatter>();

		private readonly CerasSerializer _ceras;

		public StandardFormatterResolver(CerasSerializer ceras)
		{
			_ceras = ceras;
			_primitiveFormatters.GetOrAddValueRef(typeof(DateTime)) = new DateTimeFormatter();
			_primitiveFormatters.GetOrAddValueRef(typeof(DateTimeOffset)) = new DateTimeOffsetFormatter();
			_primitiveFormatters.GetOrAddValueRef(typeof(TimeSpan)) = new TimeSpanFormatter();
			_primitiveFormatters.GetOrAddValueRef(typeof(BitVector32)) = new BitVector32Formatter();
			_primitiveFormatters.GetOrAddValueRef(typeof(BigInteger)) = new BigIntegerFormatter();
			_primitiveFormatters.GetOrAddValueRef(typeof(Uri)) = new UriFormatter();
			_primitiveFormatters.GetOrAddValueRef(typeof(BitArray)) = new BitArrayFormatter();
			_primitiveFormatters.GetOrAddValueRef(typeof(Bitmap)) = new BitmapFormatter();
			_primitiveFormatters.GetOrAddValueRef(typeof(Color)) = new ColorFormatter();
		}

		public IFormatter GetFormatter(Type type)
		{
			if (_primitiveFormatters.TryGetValue(type, out var value))
			{
				return value;
			}
			if (type.IsGenericType)
			{
				Type genericTypeDefinition = type.GetGenericTypeDefinition();
				if (genericTypeDefinition == typeof(Nullable<>))
				{
					Type type2 = ReflectionHelper.FindClosedType(type, typeof(Nullable<>));
					return (IFormatter)Activator.CreateInstance(typeof(NullableFormatter<>).MakeGenericType(type2.GetGenericArguments()), _ceras);
				}
				if (genericTypeDefinition == typeof(KeyValuePair<, >))
				{
					Type type3 = ReflectionHelper.FindClosedType(type, typeof(KeyValuePair<, >));
					return (IFormatter)Activator.CreateInstance(typeof(KeyValuePairFormatter<, >).MakeGenericType(type3.GetGenericArguments()));
				}
				if (_iTupleInterface.IsAssignableFrom(type))
				{
					if (type.IsValueType)
					{
						int num = type.GenericTypeArguments.Length;
						IFormatter result = (IFormatter)Activator.CreateInstance(_valueTupleFormatterTypes[num].MakeGenericType(type.GenericTypeArguments));
						CerasSerializer.AddFormatterConstructedType(type);
						return result;
					}
					if (type.IsClass)
					{
						int num2 = type.GenericTypeArguments.Length;
						IFormatter result2 = (IFormatter)Activator.CreateInstance(_tupleFormatterTypes[num2].MakeGenericType(type.GenericTypeArguments));
						CerasSerializer.AddFormatterConstructedType(type);
						return result2;
					}
				}
			}
			return null;
		}
	}
}

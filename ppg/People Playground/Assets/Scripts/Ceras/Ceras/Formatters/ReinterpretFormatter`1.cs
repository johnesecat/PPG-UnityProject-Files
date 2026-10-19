using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Ceras.Helpers;

namespace Ceras.Formatters
{
	public sealed class ReinterpretFormatter<T> : IFormatter<T>, IFormatter, IInlineEmitter where T : unmanaged
	{
		private delegate void ReadWriteRawDelegate(byte[] buffer, int offset, ref T value);

		internal static readonly MethodInfo _writeMethod;

		internal static readonly MethodInfo _readMethod;

		internal static readonly int _itemSize;

		static ReinterpretFormatter()
		{
			_writeMethod = new ReadWriteRawDelegate(Write_Raw).Method;
			_readMethod = new ReadWriteRawDelegate(Read_Raw).Method;
			Type type = typeof(T);
			if (type.IsEnum)
			{
				type = type.GetEnumUnderlyingType();
			}
			_itemSize = ReflectionHelper.GetSize(type);
			if (_itemSize < 0)
			{
				throw new InvalidOperationException("Type is not blittable");
			}
		}

		public ReinterpretFormatter()
		{
			ThrowIfNotSupported();
		}

		public void Serialize(ref byte[] buffer, ref int offset, T value)
		{
			SerializerBinary.EnsureCapacity(ref buffer, offset, _itemSize);
			Write_Raw(buffer, offset, ref value);
			offset += _itemSize;
		}

		public void Deserialize(byte[] buffer, ref int offset, ref T value)
		{
			Read_Raw(buffer, offset, ref value);
			offset += _itemSize;
		}

		Expression IInlineEmitter.EmitWrite(ParameterExpression bufferExp, ParameterExpression offsetExp, ParameterExpression valueExp, out int writtenSize)
		{
			MethodCallExpression result = Expression.Call(_writeMethod, bufferExp, offsetExp, valueExp);
			writtenSize = _itemSize;
			return result;
		}

		Expression IInlineEmitter.EmitRead(ParameterExpression bufferExp, ParameterExpression offsetExp, ParameterExpression valueExp, out int readSize)
		{
			MethodCallExpression result = Expression.Call(_readMethod, bufferExp, offsetExp, valueExp);
			readSize = _itemSize;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static void Write_Raw(byte[] buffer, int offset, ref T value)
		{
			fixed (byte* ptr = &buffer[0])
			{
				T* ptr2 = (T*)(ptr + offset);
				*ptr2 = value;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static void Read_Raw(byte[] buffer, int offset, ref T value)
		{
			fixed (byte* ptr = &buffer[0])
			{
				T* ptr2 = (T*)(ptr + offset);
				value = *ptr2;
			}
		}

		internal static void ThrowIfNotSupported()
		{
			if (!BitConverter.IsLittleEndian)
			{
				throw new Exception("The reinterpret formatters require a little endian environment (CPU/OS). Please turn off UseReinterpretFormatter");
			}
		}
	}
}

using System;
using System.Runtime.InteropServices;
using Ceras.Helpers;

namespace Ceras.Formatters
{
	public sealed class ReinterpretArrayFormatter<T> : IFormatter<T[]>, IFormatter where T : unmanaged
	{
		private static readonly int _itemSize;

		private readonly uint _maxCount;

		static ReinterpretArrayFormatter()
		{
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

		public ReinterpretArrayFormatter(uint maxCount)
		{
			ReinterpretFormatter<T>.ThrowIfNotSupported();
			_maxCount = maxCount;
		}

		public unsafe void Serialize(ref byte[] buffer, ref int offset, T[] value)
		{
			int num = value.Length;
			int itemSize = _itemSize;
			int size = num * itemSize + 5;
			SerializerBinary.EnsureCapacity(ref buffer, offset, size);
			SerializerBinary.WriteUInt32NoCheck(buffer, ref offset, (uint)num);
			int num2 = num * itemSize;
			if (num2 == 0)
			{
				return;
			}
			fixed (T* ptr = &value[0])
			{
				fixed (byte* destination = &buffer[offset])
				{
					byte* source = (byte*)ptr;
					Buffer.MemoryCopy(source, destination, num2, num2);
				}
			}
			offset += num2;
		}

		public unsafe void Deserialize(byte[] buffer, ref int offset, ref T[] value)
		{
			int num = (int)SerializerBinary.ReadUInt32(buffer, ref offset);
			if (num > _maxCount)
			{
				throw new InvalidOperationException($"The data describes an array with '{num}' elements, which exceeds the allowed limit of '{_maxCount}'");
			}
			if (value == null || value.Length != num)
			{
				value = new T[num];
			}
			int num2 = num * _itemSize;
			if (num2 != 0)
			{
				int num3 = buffer.Length - offset;
				if (num2 > num3)
				{
					throw new IndexOutOfRangeException($"Trying to read an array of '{typeof(T).FriendlyName()}' ({num} elements, {num2} bytes) but only {num3} bytes are left in the buffer (buffer length: {buffer.Length}, offset: {offset}).");
				}
				fixed (T* ptr = &value[0])
				{
					byte* value2 = (byte*)ptr;
					Marshal.Copy(buffer, offset, new IntPtr(value2), num2);
				}
				offset += num2;
			}
		}
	}
}

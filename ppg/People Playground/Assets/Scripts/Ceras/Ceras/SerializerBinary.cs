using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ceras
{
	public static class SerializerBinary
	{
		private static readonly UTF8Encoding _utf8Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

		private const int MaximumArraySize = 2147483591;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteInt16Fixed(ref byte[] buffer, ref int offset, short value)
		{
			EnsureCapacity(ref buffer, offset, 2);
			fixed (byte* ptr = &buffer[0])
			{
				*(short*)(ptr + offset) = value;
			}
			offset += 2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static short ReadInt16Fixed(byte[] buffer, ref int offset)
		{
			short result;
			fixed (byte* ptr = &buffer[0])
			{
				result = *(short*)(ptr + offset);
			}
			offset += 2;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteInt32(ref byte[] buffer, ref int offset, int value)
		{
			EnsureCapacity(ref buffer, offset, 5);
			long value2 = EncodeZigZag(value, 32);
			WriteVarInt(ref buffer, ref offset, (ulong)value2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ReadInt32(byte[] buffer, ref int offset)
		{
			return (int)DecodeZigZag(ReadVarInt(buffer, ref offset, 32));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt32(ref byte[] buffer, ref int offset, uint value)
		{
			EnsureCapacity(ref buffer, offset, 5);
			WriteVarInt(ref buffer, ref offset, value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ReadUInt32(byte[] buffer, ref int offset)
		{
			return (uint)ReadVarInt(buffer, ref offset, 32);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt32Bias(ref byte[] buffer, ref int offset, int value, int bias)
		{
			value += bias;
			EnsureCapacity(ref buffer, offset, 5);
			WriteVarInt(ref buffer, ref offset, (ulong)value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ReadUInt32Bias(byte[] buffer, ref int offset, int bias)
		{
			return (int)ReadVarInt(buffer, ref offset, 32) - bias;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt32BiasNoCheck(byte[] buffer, ref int offset, int value, int bias)
		{
			value += bias;
			WriteVarInt(ref buffer, ref offset, (ulong)value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt32NoCheck(byte[] buffer, ref int offset, uint value)
		{
			WriteVarInt(ref buffer, ref offset, value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteInt64(ref byte[] buffer, ref int offset, long value)
		{
			EnsureCapacity(ref buffer, offset, 9);
			long value2 = EncodeZigZag(value, 64);
			WriteVarInt(ref buffer, ref offset, (ulong)value2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long ReadInt64(byte[] buffer, ref int offset)
		{
			return (int)DecodeZigZag(ReadVarInt(buffer, ref offset, 64));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUInt64(ref byte[] buffer, ref int offset, ulong value)
		{
			EnsureCapacity(ref buffer, offset, 9);
			long value2 = EncodeZigZag((long)value, 64);
			WriteVarInt(ref buffer, ref offset, (ulong)value2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong ReadUInt64(byte[] buffer, ref int offset)
		{
			return (ulong)DecodeZigZag(ReadVarInt(buffer, ref offset, 64));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteInt64Fixed(ref byte[] buffer, ref int offset, long value)
		{
			EnsureCapacity(ref buffer, offset, 8);
			fixed (byte* ptr = &buffer[0])
			{
				*(long*)(ptr + offset) = value;
			}
			offset += 8;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static long ReadInt64Fixed(byte[] buffer, ref int offset)
		{
			long result;
			fixed (byte* ptr = &buffer[0])
			{
				result = *(long*)(ptr + offset);
			}
			offset += 8;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteInt32Fixed(ref byte[] buffer, ref int offset, int value)
		{
			EnsureCapacity(ref buffer, offset, 4);
			fixed (byte* ptr = &buffer[0])
			{
				*(int*)(ptr + offset) = value;
			}
			offset += 4;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int ReadInt32Fixed(byte[] buffer, ref int offset)
		{
			int result;
			fixed (byte* ptr = &buffer[0])
			{
				result = *(int*)(ptr + offset);
			}
			offset += 4;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteUInt32Fixed(ref byte[] buffer, ref int offset, uint value)
		{
			EnsureCapacity(ref buffer, offset, 4);
			fixed (byte* ptr = &buffer[0])
			{
				*(uint*)(ptr + offset) = value;
			}
			offset += 4;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static uint ReadUInt32Fixed(byte[] buffer, ref int offset)
		{
			int result;
			fixed (byte* ptr = &buffer[0])
			{
				result = *(int*)(ptr + offset);
			}
			offset += 4;
			return (uint)result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteByte(ref byte[] buffer, ref int offset, byte value)
		{
			EnsureCapacity(ref buffer, offset, 1);
			buffer[offset] = value;
			offset++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte ReadByte(byte[] buffer, ref int offset)
		{
			byte result = buffer[offset];
			offset++;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void WriteVarInt(ref byte[] buffer, ref int offset, ulong value)
		{
			do
			{
				ulong num = value & 0x7F;
				value >>= 7;
				if (value != 0L)
				{
					num |= 0x80;
				}
				buffer[offset++] = (byte)num;
			}
			while (value != 0L);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ulong ReadVarInt(byte[] bytes, ref int offset, int bits)
		{
			int num = 0;
			ulong num2 = 0uL;
			while (true)
			{
				long num3 = bytes[offset++];
				ulong num4 = (ulong)(num3 & 0x7F);
				num2 |= num4 << num;
				if (num > bits)
				{
					throw new ArgumentOutOfRangeException("bytes", "Malformed VarInt");
				}
				if ((num3 & 0x80) != 128)
				{
					break;
				}
				num += 7;
			}
			return num2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteFloat32FixedNoCheck(byte[] buffer, ref int offset, float value)
		{
			fixed (byte* ptr = &buffer[0])
			{
				*(float*)(ptr + offset) = value;
			}
			offset += 4;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteFloat32Fixed(ref byte[] buffer, ref int offset, float value)
		{
			EnsureCapacity(ref buffer, offset, 4);
			fixed (byte* ptr = &buffer[0])
			{
				*(float*)(ptr + offset) = value;
			}
			offset += 4;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static float ReadFloat32Fixed(byte[] buffer, ref int offset)
		{
			float result;
			fixed (byte* ptr = &buffer[0])
			{
				result = *(float*)(ptr + offset);
			}
			offset += 4;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteDouble64Fixed(ref byte[] buffer, ref int offset, double value)
		{
			EnsureCapacity(ref buffer, offset, 8);
			fixed (byte* ptr = &buffer[0])
			{
				*(double*)(ptr + offset) = value;
			}
			offset += 8;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static double ReadDouble64Fixed(byte[] buffer, ref int offset)
		{
			double result;
			fixed (byte* ptr = &buffer[0])
			{
				result = *(double*)(ptr + offset);
			}
			offset += 8;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteString(ref byte[] buffer, ref int offset, string value)
		{
			if (value == null)
			{
				WriteUInt32Bias(ref buffer, ref offset, -1, 1);
				return;
			}
			UTF8Encoding utf8Encoding = _utf8Encoding;
			int byteCount = utf8Encoding.GetByteCount(value);
			EnsureCapacity(ref buffer, offset, byteCount + 5);
			WriteUInt32BiasNoCheck(buffer, ref offset, byteCount, 1);
			int bytes = utf8Encoding.GetBytes(value, 0, value.Length, buffer, offset);
			offset += bytes;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ReadString(byte[] buffer, ref int offset)
		{
			int num = ReadUInt32Bias(buffer, ref offset, 1);
			if (num == -1)
			{
				return null;
			}
			string @string = _utf8Encoding.GetString(buffer, offset, num);
			offset += num;
			return @string;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ReadStringLimited(byte[] buffer, ref int offset, uint maxLength)
		{
			int num = ReadUInt32Bias(buffer, ref offset, 1);
			if (num == -1)
			{
				return null;
			}
			if ((uint)num > maxLength)
			{
				throw new InvalidOperationException($"The current data contains a string of length '{num}', but the maximum allowed string length is '{maxLength}'");
			}
			string @string = _utf8Encoding.GetString(buffer, offset, num);
			offset += num;
			return @string;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long EncodeZigZag(long value, int bitLength)
		{
			return (value << 1) ^ (value >> bitLength - 1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long DecodeZigZag(ulong value)
		{
			if ((value & 1) == 1)
			{
				return -1L * (long)((value >> 1) + 1);
			}
			return (long)(value >> 1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static void FastCopy(byte[] sourceArray, int sourceOffset, byte[] targetArray, int targetOffset, int n)
		{
			fixed (byte* ptr = &sourceArray[0])
			{
				fixed (byte* ptr2 = &targetArray[0])
				{
					byte* src = ptr + sourceOffset;
					byte* dest = ptr2 + targetOffset;
					FastCopy(src, dest, n);
				}
			}
		}

		internal unsafe static void FastCopy(byte* src, byte* dest, int n)
		{
			if (n > 512)
			{
				void* source = src;
				void* destination = dest;
				Unsafe.CopyBlock(destination, source, (uint)n);
				return;
			}
			while (true)
			{
				switch (n)
				{
				case 16:
					*(long*)dest = *(long*)src;
					*(long*)(dest + 8) = *(long*)(src + 8);
					return;
				case 15:
					*(short*)(dest + 12) = *(short*)(src + 12);
					dest[14] = src[14];
					goto case 12;
				case 14:
					*(short*)(dest + 12) = *(short*)(src + 12);
					goto case 12;
				case 13:
					dest[12] = src[12];
					goto case 12;
				case 12:
					*(long*)dest = *(long*)src;
					*(int*)(dest + 8) = *(int*)(src + 8);
					return;
				case 11:
					*(short*)(dest + 8) = *(short*)(src + 8);
					dest[10] = src[10];
					goto case 8;
				case 10:
					*(short*)(dest + 8) = *(short*)(src + 8);
					goto case 8;
				case 9:
					dest[8] = src[8];
					goto case 8;
				case 8:
					*(long*)dest = *(long*)src;
					return;
				case 7:
					*(short*)(dest + 4) = *(short*)(src + 4);
					dest[6] = src[6];
					goto case 4;
				case 6:
					*(short*)(dest + 4) = *(short*)(src + 4);
					goto case 4;
				case 5:
					dest[4] = src[4];
					goto case 4;
				case 4:
					*(int*)dest = *(int*)src;
					return;
				case 3:
					dest[2] = src[2];
					goto case 2;
				case 2:
					*(short*)dest = *(short*)src;
					return;
				case 1:
					*dest = *src;
					return;
				case 0:
					return;
				}
				int num = n / 32;
				n -= n / 32 * 32;
				while (num > 0)
				{
					*(long*)dest = *(long*)src;
					*(long*)(dest + 8) = *(long*)(src + 8);
					*(long*)(dest + 2L * 8L) = *(long*)(src + 2L * 8L);
					*(long*)(dest + 3L * 8L) = *(long*)(src + 3L * 8L);
					dest += 32;
					src += 32;
					num--;
				}
				if (n > 16)
				{
					*(long*)dest = *(long*)src;
					*(long*)(dest + 8) = *(long*)(src + 8);
					src += 16;
					dest += 16;
					n -= 16;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void EnsureCapacity(ref byte[] buffer, int offset, int size)
		{
			int num = offset + size;
			if (buffer.Length < num)
			{
				ExpandBuffer(ref buffer, num);
			}
		}

		private static void ExpandBuffer(ref byte[] buffer, int newSize)
		{
			ThrowIfBufferTooLarge(newSize);
			if (newSize < 16384)
			{
				newSize = 16384;
			}
			else
			{
				int num = 16384;
				while (num < newSize)
				{
					num *= 2;
					if (num < 0)
					{
						num = 2147483591;
						break;
					}
				}
				newSize = num;
			}
			FastResize(ref buffer, newSize);
		}

		private unsafe static void FastResize(ref byte[] buffer, int newSize)
		{
			if (newSize <= 0)
			{
				throw new ArgumentOutOfRangeException("newSize");
			}
			byte[] array = buffer;
			ICerasBufferPool obj = CerasBufferPool.Pool ?? NullPool.Instance;
			if (newSize <= array.Length)
			{
				throw new ArgumentOutOfRangeException("newSize cannot be smaller than (or equal to) the old size");
			}
			byte[] array2 = obj.RentBuffer(newSize);
			fixed (byte* source = &array[0])
			{
				fixed (byte* destination = &array2[0])
				{
					Buffer.MemoryCopy(source, destination, array2.Length, buffer.Length);
				}
			}
			obj.Return(buffer);
			buffer = array2;
		}

		private static void ThrowIfBufferTooLarge(int newSize)
		{
			if (newSize > 2147483591 || newSize < 0)
			{
				throw new InvalidOperationException($"Trying to expand a buffer to {newSize} bytes, which is greater than the maximum allowed size {2147483591}. This is a limitation of the runtime, but you can either use IExternalRootObject to split your object graph into parts (if there is no single element that is causing this), or write a custom formatter if you have a single huge element that is causing this. Checkout the GitHub page for more information.");
			}
		}
	}
}

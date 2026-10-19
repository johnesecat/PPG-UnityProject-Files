using System;
using System.Runtime.InteropServices;

namespace NAudio.Wave
{
	[StructLayout(2, Pack = 2)]
	public class WaveBuffer : IWaveBuffer
	{
		[FieldOffset(0)]
		public int numberOfBytes;

		[FieldOffset(8)]
		private byte[] byteBuffer;

		[FieldOffset(8)]
		private float[] floatBuffer;

		[FieldOffset(8)]
		private short[] shortBuffer;

		[FieldOffset(8)]
		private int[] intBuffer;

		public byte[] ByteBuffer => byteBuffer;

		public float[] FloatBuffer => floatBuffer;

		public short[] ShortBuffer => shortBuffer;

		public int[] IntBuffer => intBuffer;

		public int MaxSize => byteBuffer.Length;

		public int ByteBufferCount
		{
			get
			{
				return numberOfBytes;
			}
			set
			{
				numberOfBytes = CheckValidityCount("ByteBufferCount", value, 1);
			}
		}

		public int FloatBufferCount
		{
			get
			{
				return numberOfBytes / 4;
			}
			set
			{
				numberOfBytes = CheckValidityCount("FloatBufferCount", value, 4);
			}
		}

		public int ShortBufferCount
		{
			get
			{
				return numberOfBytes / 2;
			}
			set
			{
				numberOfBytes = CheckValidityCount("ShortBufferCount", value, 2);
			}
		}

		public int IntBufferCount
		{
			get
			{
				return numberOfBytes / 4;
			}
			set
			{
				numberOfBytes = CheckValidityCount("IntBufferCount", value, 4);
			}
		}

		public WaveBuffer(int sizeToAllocateInBytes)
		{
			int num = sizeToAllocateInBytes % 4;
			sizeToAllocateInBytes = ((num == 0) ? sizeToAllocateInBytes : (sizeToAllocateInBytes + 4 - num));
			byteBuffer = new byte[sizeToAllocateInBytes];
			numberOfBytes = 0;
		}

		public WaveBuffer(byte[] bufferToBoundTo)
		{
			BindTo(bufferToBoundTo);
		}

		public void BindTo(byte[] bufferToBoundTo)
		{
			byteBuffer = bufferToBoundTo;
			numberOfBytes = 0;
		}

		public static implicit operator byte[](WaveBuffer waveBuffer)
		{
			return waveBuffer.byteBuffer;
		}

		public static implicit operator float[](WaveBuffer waveBuffer)
		{
			return waveBuffer.floatBuffer;
		}

		public static implicit operator int[](WaveBuffer waveBuffer)
		{
			return waveBuffer.intBuffer;
		}

		public static implicit operator short[](WaveBuffer waveBuffer)
		{
			return waveBuffer.shortBuffer;
		}

		public void Clear()
		{
			global::System.Array.Clear((global::System.Array)byteBuffer, 0, byteBuffer.Length);
		}

		public void Copy(global::System.Array destinationArray)
		{
			global::System.Array.Copy((global::System.Array)byteBuffer, destinationArray, numberOfBytes);
		}

		private int CheckValidityCount(string argName, int value, int sizeOfValue)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			int num = value * sizeOfValue;
			if (num % 4 != 0)
			{
				throw new ArgumentOutOfRangeException(argName, $"{argName} cannot set a count ({num}) that is not 4 bytes aligned ");
			}
			if (value < 0 || value > byteBuffer.Length / sizeOfValue)
			{
				throw new ArgumentOutOfRangeException(argName, $"{argName} cannot set a count that exceed max count {byteBuffer.Length / sizeOfValue}");
			}
			return num;
		}
	}
}

using System;

namespace NAudio.Wave.Asio
{
	internal class AsioSampleConvertor
	{
		public delegate void SampleConvertor(global::System.IntPtr inputInterleavedBuffer, global::System.IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples);

		public static SampleConvertor SelectSampleConvertor(WaveFormat waveFormat, AsioSampleType asioType)
		{
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			SampleConvertor result = null;
			bool flag = waveFormat.Channels == 2;
			switch (asioType)
			{
			case AsioSampleType.Int32LSB:
				switch (waveFormat.BitsPerSample)
				{
				case 16:
					result = (flag ? new SampleConvertor(ConvertorShortToInt2Channels) : new SampleConvertor(ConvertorShortToIntGeneric));
					break;
				case 32:
					result = (flag ? new SampleConvertor(ConvertorFloatToInt2Channels) : new SampleConvertor(ConvertorFloatToIntGeneric));
					break;
				}
				break;
			case AsioSampleType.Int16LSB:
				switch (waveFormat.BitsPerSample)
				{
				case 16:
					result = (flag ? new SampleConvertor(ConvertorShortToShort2Channels) : new SampleConvertor(ConvertorShortToShortGeneric));
					break;
				case 32:
					result = (flag ? new SampleConvertor(ConvertorFloatToShort2Channels) : new SampleConvertor(ConvertorFloatToShortGeneric));
					break;
				}
				break;
			case AsioSampleType.Int24LSB:
				switch (waveFormat.BitsPerSample)
				{
				case 16:
					throw new ArgumentException("Not a supported conversion");
				case 32:
					result = ConverterFloatTo24LSBGeneric;
					break;
				}
				break;
			case AsioSampleType.Float32LSB:
				switch (waveFormat.BitsPerSample)
				{
				case 16:
					throw new ArgumentException("Not a supported conversion");
				case 32:
					result = ConverterFloatToFloatGeneric;
					break;
				}
				break;
			default:
				throw new ArgumentException($"ASIO Buffer Type {global::System.Enum.GetName(typeof(AsioSampleType), (object)asioType)} is not yet supported.");
			}
			return result;
		}

		public unsafe static void ConvertorShortToInt2Channels(global::System.IntPtr inputInterleavedBuffer, global::System.IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			short* ptr = (short*)(void*)inputInterleavedBuffer;
			short* ptr2 = (short*)(void*)asioOutputBuffers[0];
			short* ptr3 = (short*)(void*)asioOutputBuffers[1];
			ptr2++;
			ptr3++;
			for (int i = 0; i < nbSamples; i++)
			{
				*ptr2 = *ptr;
				*ptr3 = ptr[1];
				ptr += 2;
				ptr2 += 2;
				ptr3 += 2;
			}
		}

		public unsafe static void ConvertorShortToIntGeneric(global::System.IntPtr inputInterleavedBuffer, global::System.IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			short* ptr = (short*)(void*)inputInterleavedBuffer;
			short*[] array = new short*[nbChannels];
			for (int i = 0; i < nbChannels; i++)
			{
				array[i] = (short*)(void*)asioOutputBuffers[i];
				array[i]++;
			}
			for (int j = 0; j < nbSamples; j++)
			{
				for (int k = 0; k < nbChannels; k++)
				{
					short* intPtr = array[k];
					short* intPtr2 = ptr;
					ptr = intPtr2 + 1;
					*intPtr = *intPtr2;
					array[k] += 2;
				}
			}
		}

		public unsafe static void ConvertorFloatToInt2Channels(global::System.IntPtr inputInterleavedBuffer, global::System.IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			float* ptr = (float*)(void*)inputInterleavedBuffer;
			int* ptr2 = (int*)(void*)asioOutputBuffers[0];
			int* ptr3 = (int*)(void*)asioOutputBuffers[1];
			for (int i = 0; i < nbSamples; i++)
			{
				int* intPtr = ptr2;
				ptr2 = intPtr + 1;
				*intPtr = clampToInt(*ptr);
				int* intPtr2 = ptr3;
				ptr3 = intPtr2 + 1;
				*intPtr2 = clampToInt(ptr[1]);
				ptr += 2;
			}
		}

		public unsafe static void ConvertorFloatToIntGeneric(global::System.IntPtr inputInterleavedBuffer, global::System.IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			float* ptr = (float*)(void*)inputInterleavedBuffer;
			int*[] array = new int*[nbChannels];
			for (int i = 0; i < nbChannels; i++)
			{
				array[i] = (int*)(void*)asioOutputBuffers[i];
			}
			for (int j = 0; j < nbSamples; j++)
			{
				for (int k = 0; k < nbChannels; k++)
				{
					ref int* reference = ref array[k];
					int* ptr2 = reference;
					reference = ptr2 + 1;
					float* intPtr = ptr;
					ptr = intPtr + 1;
					*ptr2 = clampToInt(*intPtr);
				}
			}
		}

		public unsafe static void ConvertorShortToShort2Channels(global::System.IntPtr inputInterleavedBuffer, global::System.IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			short* ptr = (short*)(void*)inputInterleavedBuffer;
			short* ptr2 = (short*)(void*)asioOutputBuffers[0];
			short* ptr3 = (short*)(void*)asioOutputBuffers[1];
			for (int i = 0; i < nbSamples; i++)
			{
				short* intPtr = ptr2;
				ptr2 = intPtr + 1;
				*intPtr = *ptr;
				short* intPtr2 = ptr3;
				ptr3 = intPtr2 + 1;
				*intPtr2 = ptr[1];
				ptr += 2;
			}
		}

		public unsafe static void ConvertorShortToShortGeneric(global::System.IntPtr inputInterleavedBuffer, global::System.IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			short* ptr = (short*)(void*)inputInterleavedBuffer;
			short*[] array = new short*[nbChannels];
			for (int i = 0; i < nbChannels; i++)
			{
				array[i] = (short*)(void*)asioOutputBuffers[i];
			}
			for (int j = 0; j < nbSamples; j++)
			{
				for (int k = 0; k < nbChannels; k++)
				{
					ref short* reference = ref array[k];
					short* ptr2 = reference;
					reference = ptr2 + 1;
					short* intPtr = ptr;
					ptr = intPtr + 1;
					*ptr2 = *intPtr;
				}
			}
		}

		public unsafe static void ConvertorFloatToShort2Channels(global::System.IntPtr inputInterleavedBuffer, global::System.IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			float* ptr = (float*)(void*)inputInterleavedBuffer;
			short* ptr2 = (short*)(void*)asioOutputBuffers[0];
			short* ptr3 = (short*)(void*)asioOutputBuffers[1];
			for (int i = 0; i < nbSamples; i++)
			{
				short* intPtr = ptr2;
				ptr2 = intPtr + 1;
				*intPtr = clampToShort(*ptr);
				short* intPtr2 = ptr3;
				ptr3 = intPtr2 + 1;
				*intPtr2 = clampToShort(ptr[1]);
				ptr += 2;
			}
		}

		public unsafe static void ConvertorFloatToShortGeneric(global::System.IntPtr inputInterleavedBuffer, global::System.IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			float* ptr = (float*)(void*)inputInterleavedBuffer;
			short*[] array = new short*[nbChannels];
			for (int i = 0; i < nbChannels; i++)
			{
				array[i] = (short*)(void*)asioOutputBuffers[i];
			}
			for (int j = 0; j < nbSamples; j++)
			{
				for (int k = 0; k < nbChannels; k++)
				{
					ref short* reference = ref array[k];
					short* ptr2 = reference;
					reference = ptr2 + 1;
					float* intPtr = ptr;
					ptr = intPtr + 1;
					*ptr2 = clampToShort(*intPtr);
				}
			}
		}

		public unsafe static void ConverterFloatTo24LSBGeneric(global::System.IntPtr inputInterleavedBuffer, global::System.IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			float* ptr = (float*)(void*)inputInterleavedBuffer;
			byte*[] array = new byte*[nbChannels];
			for (int i = 0; i < nbChannels; i++)
			{
				array[i] = (byte*)(void*)asioOutputBuffers[i];
			}
			for (int j = 0; j < nbSamples; j++)
			{
				for (int k = 0; k < nbChannels; k++)
				{
					float* intPtr = ptr;
					ptr = intPtr + 1;
					int num = clampTo24Bit(*intPtr);
					*(array[k]++) = (byte)num;
					*(array[k]++) = (byte)(num >> 8);
					*(array[k]++) = (byte)(num >> 16);
				}
			}
		}

		public unsafe static void ConverterFloatToFloatGeneric(global::System.IntPtr inputInterleavedBuffer, global::System.IntPtr[] asioOutputBuffers, int nbChannels, int nbSamples)
		{
			float* ptr = (float*)(void*)inputInterleavedBuffer;
			float*[] array = new float*[nbChannels];
			for (int i = 0; i < nbChannels; i++)
			{
				array[i] = (float*)(void*)asioOutputBuffers[i];
			}
			for (int j = 0; j < nbSamples; j++)
			{
				for (int k = 0; k < nbChannels; k++)
				{
					ref float* reference = ref array[k];
					float* ptr2 = reference;
					reference = ptr2 + 1;
					float* intPtr = ptr;
					ptr = intPtr + 1;
					*ptr2 = *intPtr;
				}
			}
		}

		private static int clampTo24Bit(double sampleValue)
		{
			sampleValue = ((sampleValue < -1.0) ? (-1.0) : ((sampleValue > 1.0) ? 1.0 : sampleValue));
			return (int)(sampleValue * 8388607.0);
		}

		private static int clampToInt(double sampleValue)
		{
			sampleValue = ((sampleValue < -1.0) ? (-1.0) : ((sampleValue > 1.0) ? 1.0 : sampleValue));
			return (int)(sampleValue * 2147483647.0);
		}

		private static short clampToShort(double sampleValue)
		{
			sampleValue = ((sampleValue < -1.0) ? (-1.0) : ((sampleValue > 1.0) ? 1.0 : sampleValue));
			return (short)(sampleValue * 32767.0);
		}
	}
}

using System;
using System.Runtime.CompilerServices;
using NAudio.Wave.Asio;

namespace NAudio.Wave
{
	public class AsioAudioAvailableEventArgs : EventArgs
	{
		public global::System.IntPtr[] InputBuffers
		{
			[CompilerGenerated]
			get
			{
				return <InputBuffers>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<InputBuffers>k__BackingField = value;
			}
		}

		public global::System.IntPtr[] OutputBuffers
		{
			[CompilerGenerated]
			get
			{
				return <OutputBuffers>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<OutputBuffers>k__BackingField = value;
			}
		}

		public bool WrittenToOutputBuffers
		{
			[CompilerGenerated]
			get
			{
				return <WrittenToOutputBuffers>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<WrittenToOutputBuffers>k__BackingField = value;
			}
		}

		public int SamplesPerBuffer
		{
			[CompilerGenerated]
			get
			{
				return <SamplesPerBuffer>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<SamplesPerBuffer>k__BackingField = value;
			}
		}

		public AsioSampleType AsioSampleType
		{
			[CompilerGenerated]
			get
			{
				return <AsioSampleType>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<AsioSampleType>k__BackingField = value;
			}
		}

		public AsioAudioAvailableEventArgs(global::System.IntPtr[] inputBuffers, global::System.IntPtr[] outputBuffers, int samplesPerBuffer, AsioSampleType asioSampleType)
			: this()
		{
			InputBuffers = inputBuffers;
			OutputBuffers = outputBuffers;
			SamplesPerBuffer = samplesPerBuffer;
			AsioSampleType = asioSampleType;
		}

		public unsafe int GetAsInterleavedSamples(float[] samples)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			int num = InputBuffers.Length;
			if (samples.Length < SamplesPerBuffer * num)
			{
				throw new ArgumentException("Buffer not big enough");
			}
			int num2 = 0;
			if (AsioSampleType == AsioSampleType.Int32LSB)
			{
				for (int i = 0; i < SamplesPerBuffer; i++)
				{
					for (int j = 0; j < num; j++)
					{
						samples[num2++] = (float)(*(int*)((byte*)(void*)InputBuffers[j] + (long)i * 4L)) / 2.1474836E+09f;
					}
				}
			}
			else if (AsioSampleType == AsioSampleType.Int16LSB)
			{
				for (int k = 0; k < SamplesPerBuffer; k++)
				{
					for (int l = 0; l < num; l++)
					{
						samples[num2++] = (float)(*(short*)((byte*)(void*)InputBuffers[l] + (long)k * 2L)) / 32767f;
					}
				}
			}
			else if (AsioSampleType == AsioSampleType.Int24LSB)
			{
				for (int m = 0; m < SamplesPerBuffer; m++)
				{
					for (int n = 0; n < num; n++)
					{
						byte* ptr = (byte*)(void*)InputBuffers[n] + m * 3;
						int num3 = *ptr | (ptr[1] << 8) | ((sbyte)ptr[2] << 16);
						samples[num2++] = (float)num3 / 8388608f;
					}
				}
			}
			else
			{
				if (AsioSampleType != AsioSampleType.Float32LSB)
				{
					throw new NotImplementedException($"ASIO Sample Type {AsioSampleType} not supported");
				}
				for (int num4 = 0; num4 < SamplesPerBuffer; num4++)
				{
					for (int num5 = 0; num5 < num; num5++)
					{
						samples[num2++] = *(float*)((byte*)(void*)InputBuffers[num5] + (long)num4 * 4L);
					}
				}
			}
			return SamplesPerBuffer * num;
		}

		[Obsolete("Better performance if you use the overload that takes an array, and reuse the same one")]
		public float[] GetAsInterleavedSamples()
		{
			int num = InputBuffers.Length;
			float[] array = new float[SamplesPerBuffer * num];
			GetAsInterleavedSamples(array);
			return array;
		}
	}
}

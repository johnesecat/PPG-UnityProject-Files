using System;
using System.Runtime.CompilerServices;

namespace NAudio.Wave.SampleProviders
{
	public class MonoToStereoSampleProvider : ISampleProvider
	{
		private readonly ISampleProvider source;

		private float[] sourceBuffer;

		public WaveFormat WaveFormat
		{
			[CompilerGenerated]
			get
			{
				return <WaveFormat>k__BackingField;
			}
		}

		public float LeftVolume
		{
			[CompilerGenerated]
			get
			{
				return <LeftVolume>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<LeftVolume>k__BackingField = value;
			}
		}

		public float RightVolume
		{
			[CompilerGenerated]
			get
			{
				return <RightVolume>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<RightVolume>k__BackingField = value;
			}
		}

		public MonoToStereoSampleProvider(ISampleProvider source)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			LeftVolume = 1f;
			RightVolume = 1f;
			if (source.WaveFormat.Channels != 1)
			{
				throw new ArgumentException("Source must be mono");
			}
			this.source = source;
			<WaveFormat>k__BackingField = WaveFormat.CreateIeeeFloatWaveFormat(source.WaveFormat.SampleRate, 2);
		}

		public int Read(float[] buffer, int offset, int count)
		{
			int count2 = count / 2;
			int num = offset;
			EnsureSourceBuffer(count2);
			int num2 = source.Read(sourceBuffer, 0, count2);
			for (int i = 0; i < num2; i++)
			{
				buffer[num++] = sourceBuffer[i] * LeftVolume;
				buffer[num++] = sourceBuffer[i] * RightVolume;
			}
			return num2 * 2;
		}

		private void EnsureSourceBuffer(int count)
		{
			if (sourceBuffer == null || sourceBuffer.Length < count)
			{
				sourceBuffer = new float[count];
			}
		}
	}
}

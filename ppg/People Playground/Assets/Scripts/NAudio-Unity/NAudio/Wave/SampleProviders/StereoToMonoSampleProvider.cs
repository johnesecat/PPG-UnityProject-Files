using System;
using System.Runtime.CompilerServices;

namespace NAudio.Wave.SampleProviders
{
	public class StereoToMonoSampleProvider : ISampleProvider
	{
		private readonly ISampleProvider sourceProvider;

		private float[] sourceBuffer;

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

		public WaveFormat WaveFormat
		{
			[CompilerGenerated]
			get
			{
				return <WaveFormat>k__BackingField;
			}
		}

		public StereoToMonoSampleProvider(ISampleProvider sourceProvider)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			LeftVolume = 0.5f;
			RightVolume = 0.5f;
			if (sourceProvider.WaveFormat.Channels != 2)
			{
				throw new ArgumentException("Source must be stereo");
			}
			this.sourceProvider = sourceProvider;
			<WaveFormat>k__BackingField = WaveFormat.CreateIeeeFloatWaveFormat(sourceProvider.WaveFormat.SampleRate, 1);
		}

		public int Read(float[] buffer, int offset, int count)
		{
			int num = count * 2;
			if (sourceBuffer == null || sourceBuffer.Length < num)
			{
				sourceBuffer = new float[num];
			}
			int num2 = sourceProvider.Read(sourceBuffer, 0, num);
			int num3 = offset / 2;
			for (int i = 0; i < num2; i += 2)
			{
				float num4 = sourceBuffer[i];
				float num5 = sourceBuffer[i + 1];
				float num6 = num4 * LeftVolume + num5 * RightVolume;
				buffer[num3++] = num6;
			}
			return num2 / 2;
		}
	}
}

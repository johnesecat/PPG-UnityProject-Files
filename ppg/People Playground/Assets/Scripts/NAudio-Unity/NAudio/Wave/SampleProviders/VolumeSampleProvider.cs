using System.Runtime.CompilerServices;

namespace NAudio.Wave.SampleProviders
{
	public class VolumeSampleProvider : ISampleProvider
	{
		private readonly ISampleProvider source;

		public WaveFormat WaveFormat => source.WaveFormat;

		public float Volume
		{
			[CompilerGenerated]
			get
			{
				return <Volume>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<Volume>k__BackingField = value;
			}
		}

		public VolumeSampleProvider(ISampleProvider source)
		{
			this.source = source;
			Volume = 1f;
		}

		public int Read(float[] buffer, int offset, int sampleCount)
		{
			int result = source.Read(buffer, offset, sampleCount);
			if (Volume != 1f)
			{
				for (int i = 0; i < sampleCount; i++)
				{
					buffer[offset + i] *= Volume;
				}
			}
			return result;
		}
	}
}

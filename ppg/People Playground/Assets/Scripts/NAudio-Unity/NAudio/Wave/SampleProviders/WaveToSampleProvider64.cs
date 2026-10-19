using System;

namespace NAudio.Wave.SampleProviders
{
	public class WaveToSampleProvider64 : SampleProviderConverterBase
	{
		public WaveToSampleProvider64(IWaveProvider source)
			: base(source)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (source.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
			{
				throw new ArgumentException("Must be already floating point");
			}
		}

		public override int Read(float[] buffer, int offset, int count)
		{
			int num = count * 8;
			EnsureSourceBuffer(num);
			int num2 = source.Read(sourceBuffer, 0, num);
			int result = num2 / 8;
			int num3 = offset;
			for (int i = 0; i < num2; i += 8)
			{
				long num4 = BitConverter.ToInt64(sourceBuffer, i);
				buffer[num3++] = (float)BitConverter.Int64BitsToDouble(num4);
			}
			return result;
		}
	}
}

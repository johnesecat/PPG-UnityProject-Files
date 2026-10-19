using System;
using NAudio.Wave;

namespace NAudio.Utils
{
	public static class WavePositionExtensions
	{
		public static TimeSpan GetPositionTimeSpan(this IWavePosition @this)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			return TimeSpan.FromMilliseconds((double)(@this.GetPosition() / (@this.OutputWaveFormat.Channels * @this.OutputWaveFormat.BitsPerSample / 8)) * 1000.0 / (double)@this.OutputWaveFormat.SampleRate);
		}
	}
}

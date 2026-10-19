using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NAudio.Wave.SampleProviders
{
	public class NotifyingSampleProvider : ISampleProvider, ISampleNotifier
	{
		private readonly ISampleProvider source;

		private readonly SampleEventArgs sampleArgs = new SampleEventArgs(0f, 0f);

		private readonly int channels;

		public WaveFormat WaveFormat => source.WaveFormat;

		public event EventHandler<SampleEventArgs> Sample
		{
			[CompilerGenerated]
			add
			{
				EventHandler<SampleEventArgs> val = this.Sample;
				EventHandler<SampleEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<SampleEventArgs> val3 = (EventHandler<SampleEventArgs>)(object)global::System.Delegate.Combine((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<SampleEventArgs>>(ref this.Sample, val3, val2);
				}
				while (val != val2);
			}
			[CompilerGenerated]
			remove
			{
				EventHandler<SampleEventArgs> val = this.Sample;
				EventHandler<SampleEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<SampleEventArgs> val3 = (EventHandler<SampleEventArgs>)(object)global::System.Delegate.Remove((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<SampleEventArgs>>(ref this.Sample, val3, val2);
				}
				while (val != val2);
			}
		}

		public NotifyingSampleProvider(ISampleProvider source)
		{
			this.source = source;
			channels = WaveFormat.Channels;
		}

		public int Read(float[] buffer, int offset, int sampleCount)
		{
			int num = source.Read(buffer, offset, sampleCount);
			if (this.Sample != null)
			{
				for (int i = 0; i < num; i += channels)
				{
					sampleArgs.Left = buffer[offset + i];
					sampleArgs.Right = ((channels > 1) ? buffer[offset + i + 1] : sampleArgs.Left);
					this.Sample.Invoke((object)this, sampleArgs);
				}
			}
			return num;
		}
	}
}

using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NAudio.Wave.SampleProviders
{
	public class MeteringSampleProvider : ISampleProvider
	{
		private readonly ISampleProvider source;

		private readonly float[] maxSamples;

		private int sampleCount;

		private readonly int channels;

		private readonly StreamVolumeEventArgs args;

		public int SamplesPerNotification
		{
			[CompilerGenerated]
			get
			{
				return <SamplesPerNotification>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<SamplesPerNotification>k__BackingField = value;
			}
		}

		public WaveFormat WaveFormat => source.WaveFormat;

		public event EventHandler<StreamVolumeEventArgs> StreamVolume
		{
			[CompilerGenerated]
			add
			{
				EventHandler<StreamVolumeEventArgs> val = this.StreamVolume;
				EventHandler<StreamVolumeEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<StreamVolumeEventArgs> val3 = (EventHandler<StreamVolumeEventArgs>)(object)global::System.Delegate.Combine((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<StreamVolumeEventArgs>>(ref this.StreamVolume, val3, val2);
				}
				while (val != val2);
			}
			[CompilerGenerated]
			remove
			{
				EventHandler<StreamVolumeEventArgs> val = this.StreamVolume;
				EventHandler<StreamVolumeEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<StreamVolumeEventArgs> val3 = (EventHandler<StreamVolumeEventArgs>)(object)global::System.Delegate.Remove((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<StreamVolumeEventArgs>>(ref this.StreamVolume, val3, val2);
				}
				while (val != val2);
			}
		}

		public MeteringSampleProvider(ISampleProvider source)
			: this(source, source.WaveFormat.SampleRate / 10)
		{
		}

		public MeteringSampleProvider(ISampleProvider source, int samplesPerNotification)
		{
			this.source = source;
			channels = source.WaveFormat.Channels;
			maxSamples = new float[channels];
			SamplesPerNotification = samplesPerNotification;
			args = new StreamVolumeEventArgs
			{
				MaxSampleValues = maxSamples
			};
		}

		public int Read(float[] buffer, int offset, int count)
		{
			int num = source.Read(buffer, offset, count);
			if (this.StreamVolume != null)
			{
				for (int i = 0; i < num; i += channels)
				{
					for (int j = 0; j < channels; j++)
					{
						float num2 = Math.Abs(buffer[offset + i + j]);
						maxSamples[j] = Math.Max(maxSamples[j], num2);
					}
					sampleCount++;
					if (sampleCount >= SamplesPerNotification)
					{
						this.StreamVolume.Invoke((object)this, args);
						sampleCount = 0;
						global::System.Array.Clear((global::System.Array)maxSamples, 0, channels);
					}
				}
			}
			return num;
		}
	}
}

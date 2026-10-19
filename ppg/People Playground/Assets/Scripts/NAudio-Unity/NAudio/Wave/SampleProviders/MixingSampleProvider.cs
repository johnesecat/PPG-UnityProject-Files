using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using NAudio.Utils;

namespace NAudio.Wave.SampleProviders
{
	public class MixingSampleProvider : ISampleProvider
	{
		private readonly List<ISampleProvider> sources;

		private float[] sourceBuffer;

		private const int MaxInputs = 1024;

		public global::System.Collections.Generic.IEnumerable<ISampleProvider> MixerInputs => (global::System.Collections.Generic.IEnumerable<ISampleProvider>)sources;

		public bool ReadFully
		{
			[CompilerGenerated]
			get
			{
				return <ReadFully>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<ReadFully>k__BackingField = value;
			}
		}

		public WaveFormat WaveFormat
		{
			[CompilerGenerated]
			get
			{
				return <WaveFormat>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<WaveFormat>k__BackingField = value;
			}
		}

		public event EventHandler<SampleProviderEventArgs> MixerInputEnded
		{
			[CompilerGenerated]
			add
			{
				EventHandler<SampleProviderEventArgs> val = this.MixerInputEnded;
				EventHandler<SampleProviderEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<SampleProviderEventArgs> val3 = (EventHandler<SampleProviderEventArgs>)(object)global::System.Delegate.Combine((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<SampleProviderEventArgs>>(ref this.MixerInputEnded, val3, val2);
				}
				while (val != val2);
			}
			[CompilerGenerated]
			remove
			{
				EventHandler<SampleProviderEventArgs> val = this.MixerInputEnded;
				EventHandler<SampleProviderEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<SampleProviderEventArgs> val3 = (EventHandler<SampleProviderEventArgs>)(object)global::System.Delegate.Remove((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<SampleProviderEventArgs>>(ref this.MixerInputEnded, val3, val2);
				}
				while (val != val2);
			}
		}

		public MixingSampleProvider(WaveFormat waveFormat)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			if (waveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
			{
				throw new ArgumentException("Mixer wave format must be IEEE float");
			}
			sources = new List<ISampleProvider>();
			WaveFormat = waveFormat;
		}

		public MixingSampleProvider(global::System.Collections.Generic.IEnumerable<ISampleProvider> sources)
		{
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			this.sources = new List<ISampleProvider>();
			global::System.Collections.Generic.IEnumerator<ISampleProvider> enumerator = sources.GetEnumerator();
			try
			{
				while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
				{
					ISampleProvider current = enumerator.get_Current();
					AddMixerInput(current);
				}
			}
			finally
			{
				((global::System.IDisposable)enumerator)?.Dispose();
			}
			if (this.sources.get_Count() == 0)
			{
				throw new ArgumentException("Must provide at least one input in this constructor");
			}
		}

		public void AddMixerInput(IWaveProvider mixerInput)
		{
			AddMixerInput(SampleProviderConverters.ConvertWaveProviderIntoSampleProvider(mixerInput));
		}

		public void AddMixerInput(ISampleProvider mixerInput)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			lock (sources)
			{
				if (sources.get_Count() >= 1024)
				{
					throw new InvalidOperationException("Too many mixer inputs");
				}
				sources.Add(mixerInput);
			}
			if (WaveFormat == null)
			{
				WaveFormat = mixerInput.WaveFormat;
			}
			else if (WaveFormat.SampleRate != mixerInput.WaveFormat.SampleRate || WaveFormat.Channels != mixerInput.WaveFormat.Channels)
			{
				throw new ArgumentException("All mixer inputs must have the same WaveFormat");
			}
		}

		public void RemoveMixerInput(ISampleProvider mixerInput)
		{
			lock (sources)
			{
				sources.Remove(mixerInput);
			}
		}

		public void RemoveAllMixerInputs()
		{
			lock (sources)
			{
				sources.Clear();
			}
		}

		public int Read(float[] buffer, int offset, int count)
		{
			int num = 0;
			sourceBuffer = BufferHelpers.Ensure(sourceBuffer, count);
			lock (sources)
			{
				for (int num2 = sources.get_Count() - 1; num2 >= 0; num2--)
				{
					ISampleProvider sampleProvider = sources.get_Item(num2);
					int num3 = sampleProvider.Read(sourceBuffer, 0, count);
					int num4 = offset;
					for (int i = 0; i < num3; i++)
					{
						if (i >= num)
						{
							buffer[num4++] = sourceBuffer[i];
						}
						else
						{
							buffer[num4++] += sourceBuffer[i];
						}
					}
					num = Math.Max(num3, num);
					if (num3 < count)
					{
						this.MixerInputEnded?.Invoke((object)this, new SampleProviderEventArgs(sampleProvider));
						sources.RemoveAt(num2);
					}
				}
			}
			if (ReadFully && num < count)
			{
				int num5 = offset + num;
				while (num5 < offset + count)
				{
					buffer[num5++] = 0f;
				}
				num = count;
			}
			return num;
		}
	}
}

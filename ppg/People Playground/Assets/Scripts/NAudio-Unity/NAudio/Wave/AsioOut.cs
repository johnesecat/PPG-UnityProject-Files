using System;
using System.Runtime.CompilerServices;
using System.Threading;
using NAudio.Wave.Asio;

namespace NAudio.Wave
{
	public class AsioOut : IWavePlayer, global::System.IDisposable
	{
		private AsioDriverExt driver;

		private IWaveProvider sourceStream;

		private PlaybackState playbackState;

		private int nbSamples;

		private byte[] waveBuffer;

		private AsioSampleConvertor.SampleConvertor convertor;

		private string driverName;

		private readonly SynchronizationContext syncContext;

		private bool isInitialized;

		public int PlaybackLatency
		{
			get
			{
				driver.Driver.GetLatencies(out var _, out var outputLatency);
				return outputLatency;
			}
		}

		public PlaybackState PlaybackState => playbackState;

		public string DriverName => driverName;

		public int NumberOfOutputChannels
		{
			[CompilerGenerated]
			get
			{
				return <NumberOfOutputChannels>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<NumberOfOutputChannels>k__BackingField = value;
			}
		}

		public int NumberOfInputChannels
		{
			[CompilerGenerated]
			get
			{
				return <NumberOfInputChannels>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<NumberOfInputChannels>k__BackingField = value;
			}
		}

		public int DriverInputChannelCount => driver.Capabilities.NbInputChannels;

		public int DriverOutputChannelCount => driver.Capabilities.NbOutputChannels;

		public int FramesPerBuffer
		{
			get
			{
				if (!isInitialized)
				{
					throw new global::System.Exception("Not initialized yet. Call this after calling Init");
				}
				return nbSamples;
			}
		}

		public int ChannelOffset
		{
			[CompilerGenerated]
			get
			{
				return <ChannelOffset>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<ChannelOffset>k__BackingField = value;
			}
		}

		public int InputChannelOffset
		{
			[CompilerGenerated]
			get
			{
				return <InputChannelOffset>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<InputChannelOffset>k__BackingField = value;
			}
		}

		[Obsolete("this function will be removed in a future NAudio as ASIO does not support setting the volume on the device")]
		public float Volume
		{
			get
			{
				return 1f;
			}
			set
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				if (value != 1f)
				{
					throw new InvalidOperationException("AsioOut does not support setting the device volume");
				}
			}
		}

		public event EventHandler<StoppedEventArgs> PlaybackStopped
		{
			[CompilerGenerated]
			add
			{
				EventHandler<StoppedEventArgs> val = this.PlaybackStopped;
				EventHandler<StoppedEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<StoppedEventArgs> val3 = (EventHandler<StoppedEventArgs>)(object)global::System.Delegate.Combine((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<StoppedEventArgs>>(ref this.PlaybackStopped, val3, val2);
				}
				while (val != val2);
			}
			[CompilerGenerated]
			remove
			{
				EventHandler<StoppedEventArgs> val = this.PlaybackStopped;
				EventHandler<StoppedEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<StoppedEventArgs> val3 = (EventHandler<StoppedEventArgs>)(object)global::System.Delegate.Remove((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<StoppedEventArgs>>(ref this.PlaybackStopped, val3, val2);
				}
				while (val != val2);
			}
		}

		public event EventHandler<AsioAudioAvailableEventArgs> AudioAvailable
		{
			[CompilerGenerated]
			add
			{
				EventHandler<AsioAudioAvailableEventArgs> val = this.AudioAvailable;
				EventHandler<AsioAudioAvailableEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<AsioAudioAvailableEventArgs> val3 = (EventHandler<AsioAudioAvailableEventArgs>)(object)global::System.Delegate.Combine((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<AsioAudioAvailableEventArgs>>(ref this.AudioAvailable, val3, val2);
				}
				while (val != val2);
			}
			[CompilerGenerated]
			remove
			{
				EventHandler<AsioAudioAvailableEventArgs> val = this.AudioAvailable;
				EventHandler<AsioAudioAvailableEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<AsioAudioAvailableEventArgs> val3 = (EventHandler<AsioAudioAvailableEventArgs>)(object)global::System.Delegate.Remove((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<AsioAudioAvailableEventArgs>>(ref this.AudioAvailable, val3, val2);
				}
				while (val != val2);
			}
		}

		public AsioOut()
			: this(0)
		{
		}

		public AsioOut(string driverName)
		{
			syncContext = SynchronizationContext.get_Current();
			InitFromName(driverName);
		}

		public AsioOut(int driverIndex)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			syncContext = SynchronizationContext.get_Current();
			string[] driverNames = GetDriverNames();
			if (driverNames.Length == 0)
			{
				throw new ArgumentException("There is no ASIO Driver installed on your system");
			}
			if (driverIndex < 0 || driverIndex > driverNames.Length)
			{
				throw new ArgumentException($"Invalid device number. Must be in the range [0,{driverNames.Length}]");
			}
			InitFromName(driverNames[driverIndex]);
		}

		~AsioOut()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (driver != null)
			{
				if (playbackState != 0)
				{
					driver.Stop();
				}
				driver.ReleaseDriver();
				driver = null;
			}
		}

		public static string[] GetDriverNames()
		{
			return AsioDriver.GetAsioDriverNames();
		}

		public static bool isSupported()
		{
			return GetDriverNames().Length != 0;
		}

		private void InitFromName(string driverName)
		{
			this.driverName = driverName;
			AsioDriver asioDriverByName = AsioDriver.GetAsioDriverByName(driverName);
			driver = new AsioDriverExt(asioDriverByName);
			ChannelOffset = 0;
		}

		public void ShowControlPanel()
		{
			driver.ShowControlPanel();
		}

		public void Play()
		{
			if (playbackState != PlaybackState.Playing)
			{
				playbackState = PlaybackState.Playing;
				driver.Start();
			}
		}

		public void Stop()
		{
			playbackState = PlaybackState.Stopped;
			driver.Stop();
			RaisePlaybackStopped(null);
		}

		public void Pause()
		{
			playbackState = PlaybackState.Paused;
			driver.Stop();
		}

		public void Init(IWaveProvider waveProvider)
		{
			InitRecordAndPlayback(waveProvider, 0, -1);
		}

		public void InitRecordAndPlayback(IWaveProvider waveProvider, int recordChannels, int recordOnlySampleRate)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			if (isInitialized)
			{
				throw new InvalidOperationException("Already initialised this instance of AsioOut - dispose and create a new one");
			}
			isInitialized = true;
			int num = waveProvider?.WaveFormat.SampleRate ?? recordOnlySampleRate;
			if (waveProvider != null)
			{
				sourceStream = waveProvider;
				NumberOfOutputChannels = waveProvider.WaveFormat.Channels;
				convertor = AsioSampleConvertor.SelectSampleConvertor(waveProvider.WaveFormat, driver.Capabilities.OutputChannelInfos[0].type);
			}
			else
			{
				NumberOfOutputChannels = 0;
			}
			if (!driver.IsSampleRateSupported(num))
			{
				throw new ArgumentException("SampleRate is not supported");
			}
			if (driver.Capabilities.SampleRate != (double)num)
			{
				driver.SetSampleRate(num);
			}
			driver.FillBufferCallback = driver_BufferUpdate;
			NumberOfInputChannels = recordChannels;
			nbSamples = driver.CreateBuffers(NumberOfOutputChannels, NumberOfInputChannels, useMaxBufferSize: false);
			driver.SetChannelOffset(ChannelOffset, InputChannelOffset);
			if (waveProvider != null)
			{
				waveBuffer = new byte[nbSamples * NumberOfOutputChannels * waveProvider.WaveFormat.BitsPerSample / 8];
			}
		}

		private unsafe void driver_BufferUpdate(global::System.IntPtr[] inputChannels, global::System.IntPtr[] outputChannels)
		{
			if (NumberOfInputChannels > 0)
			{
				EventHandler<AsioAudioAvailableEventArgs> audioAvailable = this.AudioAvailable;
				if (audioAvailable != null)
				{
					AsioAudioAvailableEventArgs asioAudioAvailableEventArgs = new AsioAudioAvailableEventArgs(inputChannels, outputChannels, nbSamples, driver.Capabilities.InputChannelInfos[0].type);
					audioAvailable.Invoke((object)this, asioAudioAvailableEventArgs);
					if (asioAudioAvailableEventArgs.WrittenToOutputBuffers)
					{
						return;
					}
				}
			}
			if (NumberOfOutputChannels > 0)
			{
				int num = sourceStream.Read(waveBuffer, 0, waveBuffer.Length);
				_ = waveBuffer.Length;
				fixed (byte* ptr = &waveBuffer[0])
				{
					void* ptr2 = ptr;
					convertor(new global::System.IntPtr(ptr2), outputChannels, NumberOfOutputChannels, nbSamples);
				}
				if (num == 0)
				{
					Stop();
				}
			}
		}

		private void RaisePlaybackStopped(global::System.Exception e)
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Expected O, but got Unknown
			<>c__DisplayClass61_0 CS$<>8__locals0 = new <>c__DisplayClass61_0();
			CS$<>8__locals0.<>4__this = this;
			CS$<>8__locals0.e = e;
			CS$<>8__locals0.handler = this.PlaybackStopped;
			if (CS$<>8__locals0.handler == null)
			{
				return;
			}
			if (syncContext == null)
			{
				CS$<>8__locals0.handler.Invoke((object)this, new StoppedEventArgs(CS$<>8__locals0.e));
				return;
			}
			syncContext.Post((SendOrPostCallback)delegate
			{
				CS$<>8__locals0.handler.Invoke((object)CS$<>8__locals0.<>4__this, new StoppedEventArgs(CS$<>8__locals0.e));
			}, (object)null);
		}

		public string AsioInputChannelName(int channel)
		{
			if (channel <= DriverInputChannelCount)
			{
				return driver.Capabilities.InputChannelInfos[channel].name;
			}
			return "";
		}

		public string AsioOutputChannelName(int channel)
		{
			if (channel <= DriverOutputChannelCount)
			{
				return driver.Capabilities.OutputChannelInfos[channel].name;
			}
			return "";
		}
	}
}

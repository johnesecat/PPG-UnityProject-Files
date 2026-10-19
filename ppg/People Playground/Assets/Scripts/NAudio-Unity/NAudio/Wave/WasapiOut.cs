using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using NAudio.CoreAudioApi;

namespace NAudio.Wave
{
	public class WasapiOut : IWavePlayer, global::System.IDisposable, IWavePosition
	{
		private AudioClient audioClient;

		private readonly MMDevice mmDevice;

		private readonly AudioClientShareMode shareMode;

		private AudioRenderClient renderClient;

		private IWaveProvider sourceProvider;

		private int latencyMilliseconds;

		private int bufferFrameCount;

		private int bytesPerFrame;

		private readonly bool isUsingEventSync;

		private EventWaitHandle frameEventWaitHandle;

		private byte[] readBuffer;

		private volatile PlaybackState playbackState;

		private Thread playThread;

		private WaveFormat outputFormat;

		private bool dmoResamplerNeeded;

		private readonly SynchronizationContext syncContext;

		public WaveFormat OutputWaveFormat => outputFormat;

		public PlaybackState PlaybackState => playbackState;

		public float Volume
		{
			get
			{
				return mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
			}
			set
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				if (value < 0f)
				{
					throw new ArgumentOutOfRangeException("value", "Volume must be between 0.0 and 1.0");
				}
				if (value > 1f)
				{
					throw new ArgumentOutOfRangeException("value", "Volume must be between 0.0 and 1.0");
				}
				mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar = value;
			}
		}

		public AudioStreamVolume AudioStreamVolume
		{
			get
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				if (shareMode == AudioClientShareMode.Exclusive)
				{
					throw new InvalidOperationException("AudioStreamVolume is ONLY supported for shared audio streams.");
				}
				return audioClient.AudioStreamVolume;
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

		public WasapiOut()
			: this(GetDefaultAudioEndpoint(), AudioClientShareMode.Shared, useEventSync: true, 200)
		{
		}

		public WasapiOut(AudioClientShareMode shareMode, int latency)
			: this(GetDefaultAudioEndpoint(), shareMode, useEventSync: true, latency)
		{
		}

		public WasapiOut(AudioClientShareMode shareMode, bool useEventSync, int latency)
			: this(GetDefaultAudioEndpoint(), shareMode, useEventSync, latency)
		{
		}

		public WasapiOut(MMDevice device, AudioClientShareMode shareMode, bool useEventSync, int latency)
		{
			audioClient = device.AudioClient;
			mmDevice = device;
			this.shareMode = shareMode;
			isUsingEventSync = useEventSync;
			latencyMilliseconds = latency;
			syncContext = SynchronizationContext.get_Current();
			outputFormat = audioClient.MixFormat;
		}

		private static MMDevice GetDefaultAudioEndpoint()
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			if (Environment.get_OSVersion().get_Version().get_Major() < 6)
			{
				throw new NotSupportedException("WASAPI supported only on Windows Vista and above");
			}
			return new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
		}

		private void PlayThread()
		{
			ResamplerDmoStream resamplerDmoStream = null;
			IWaveProvider playbackProvider = sourceProvider;
			global::System.Exception e = null;
			try
			{
				if (dmoResamplerNeeded)
				{
					resamplerDmoStream = new ResamplerDmoStream(sourceProvider, outputFormat);
					playbackProvider = resamplerDmoStream;
				}
				bufferFrameCount = audioClient.BufferSize;
				bytesPerFrame = outputFormat.Channels * outputFormat.BitsPerSample / 8;
				readBuffer = new byte[bufferFrameCount * bytesPerFrame];
				FillBuffer(playbackProvider, bufferFrameCount);
				WaitHandle[] array = (WaitHandle[])(object)new WaitHandle[1] { (WaitHandle)frameEventWaitHandle };
				audioClient.Start();
				while (playbackState != 0)
				{
					int num = 0;
					if (isUsingEventSync)
					{
						num = WaitHandle.WaitAny(array, 3 * latencyMilliseconds, false);
					}
					else
					{
						Thread.Sleep(latencyMilliseconds / 2);
					}
					if (playbackState == PlaybackState.Playing && num != 258)
					{
						int num2 = ((!isUsingEventSync) ? audioClient.CurrentPadding : ((shareMode == AudioClientShareMode.Shared) ? audioClient.CurrentPadding : 0));
						int num3 = bufferFrameCount - num2;
						if (num3 > 10)
						{
							FillBuffer(playbackProvider, num3);
						}
					}
				}
				Thread.Sleep(latencyMilliseconds / 2);
				audioClient.Stop();
				if (playbackState == PlaybackState.Stopped)
				{
					audioClient.Reset();
				}
			}
			catch (global::System.Exception ex)
			{
				e = ex;
			}
			finally
			{
				if (resamplerDmoStream != null)
				{
					((Stream)resamplerDmoStream).Dispose();
				}
				RaisePlaybackStopped(e);
			}
		}

		private void RaisePlaybackStopped(global::System.Exception e)
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Expected O, but got Unknown
			<>c__DisplayClass25_0 CS$<>8__locals0 = new <>c__DisplayClass25_0();
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

		private void FillBuffer(IWaveProvider playbackProvider, int frameCount)
		{
			global::System.IntPtr buffer = renderClient.GetBuffer(frameCount);
			int count = frameCount * bytesPerFrame;
			int num = playbackProvider.Read(readBuffer, 0, count);
			if (num == 0)
			{
				playbackState = PlaybackState.Stopped;
			}
			Marshal.Copy(readBuffer, 0, buffer, num);
			int numFramesWritten = num / bytesPerFrame;
			renderClient.ReleaseBuffer(numFramesWritten, AudioClientBufferFlags.None);
		}

		public long GetPosition()
		{
			if (playbackState == PlaybackState.Stopped)
			{
				return 0L;
			}
			return (long)audioClient.AudioClockClient.AdjustedPosition;
		}

		public void Play()
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			if (playbackState != PlaybackState.Playing)
			{
				if (playbackState == PlaybackState.Stopped)
				{
					playThread = new Thread(new ThreadStart(PlayThread));
					playbackState = PlaybackState.Playing;
					playThread.Start();
				}
				else
				{
					playbackState = PlaybackState.Playing;
				}
			}
		}

		public void Stop()
		{
			if (playbackState != 0)
			{
				playbackState = PlaybackState.Stopped;
				playThread.Join();
				playThread = null;
			}
		}

		public void Pause()
		{
			if (playbackState == PlaybackState.Playing)
			{
				playbackState = PlaybackState.Paused;
			}
		}

		public void Init(IWaveProvider waveProvider)
		{
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Expected O, but got Unknown
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			long num = latencyMilliseconds * 10000;
			outputFormat = waveProvider.WaveFormat;
			if (!audioClient.IsFormatSupported(shareMode, outputFormat, out var closestMatchFormat))
			{
				if (closestMatchFormat == null)
				{
					WaveFormat waveFormat = audioClient.MixFormat;
					if (!audioClient.IsFormatSupported(shareMode, waveFormat))
					{
						WaveFormatExtensible[] array = new WaveFormatExtensible[3]
						{
							new WaveFormatExtensible(outputFormat.SampleRate, 32, outputFormat.Channels),
							new WaveFormatExtensible(outputFormat.SampleRate, 24, outputFormat.Channels),
							new WaveFormatExtensible(outputFormat.SampleRate, 16, outputFormat.Channels)
						};
						for (int i = 0; i < array.Length; i++)
						{
							waveFormat = array[i];
							if (audioClient.IsFormatSupported(shareMode, waveFormat))
							{
								break;
							}
							waveFormat = null;
						}
						if (waveFormat == null)
						{
							waveFormat = new WaveFormatExtensible(outputFormat.SampleRate, 16, 2);
							if (!audioClient.IsFormatSupported(shareMode, waveFormat))
							{
								throw new NotSupportedException("Can't find a supported format to use");
							}
						}
					}
					outputFormat = waveFormat;
				}
				else
				{
					outputFormat = closestMatchFormat;
				}
				ResamplerDmoStream resamplerDmoStream = new ResamplerDmoStream(waveProvider, outputFormat);
				try
				{
				}
				finally
				{
					((global::System.IDisposable)resamplerDmoStream)?.Dispose();
				}
				dmoResamplerNeeded = true;
			}
			else
			{
				dmoResamplerNeeded = false;
			}
			sourceProvider = waveProvider;
			if (isUsingEventSync)
			{
				if (shareMode == AudioClientShareMode.Shared)
				{
					audioClient.Initialize(shareMode, AudioClientStreamFlags.EventCallback, num, 0L, outputFormat, Guid.Empty);
					long streamLatency = audioClient.StreamLatency;
					if (streamLatency != 0L)
					{
						latencyMilliseconds = (int)(streamLatency / 10000);
					}
				}
				else
				{
					audioClient.Initialize(shareMode, AudioClientStreamFlags.EventCallback, num, num, outputFormat, Guid.Empty);
				}
				frameEventWaitHandle = new EventWaitHandle(false, (EventResetMode)0);
				audioClient.SetEventHandle(((SafeHandle)((WaitHandle)frameEventWaitHandle).get_SafeWaitHandle()).DangerousGetHandle());
			}
			else
			{
				audioClient.Initialize(shareMode, AudioClientStreamFlags.None, num, 0L, outputFormat, Guid.Empty);
			}
			renderClient = audioClient.AudioRenderClient;
		}

		public void Dispose()
		{
			if (audioClient != null)
			{
				Stop();
				audioClient.Dispose();
				audioClient = null;
				renderClient = null;
			}
		}
	}
}

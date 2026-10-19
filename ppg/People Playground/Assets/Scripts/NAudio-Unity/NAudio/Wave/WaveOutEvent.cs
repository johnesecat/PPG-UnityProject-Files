using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace NAudio.Wave
{
	public class WaveOutEvent : IWavePlayer, global::System.IDisposable, IWavePosition
	{
		private readonly object waveOutLock;

		private readonly SynchronizationContext syncContext;

		private global::System.IntPtr hWaveOut;

		private WaveOutBuffer[] buffers;

		private IWaveProvider waveStream;

		private volatile PlaybackState playbackState;

		private AutoResetEvent callbackEvent;

		private float volume = 1f;

		public int DesiredLatency
		{
			[CompilerGenerated]
			get
			{
				return <DesiredLatency>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<DesiredLatency>k__BackingField = value;
			}
		}

		public int NumberOfBuffers
		{
			[CompilerGenerated]
			get
			{
				return <NumberOfBuffers>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<NumberOfBuffers>k__BackingField = value;
			}
		}

		public int DeviceNumber
		{
			[CompilerGenerated]
			get
			{
				return <DeviceNumber>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<DeviceNumber>k__BackingField = value;
			}
		}

		public WaveFormat OutputWaveFormat => waveStream.WaveFormat;

		public PlaybackState PlaybackState => playbackState;

		public float Volume
		{
			get
			{
				return volume;
			}
			set
			{
				volume = value;
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

		public WaveOutEvent()
		{
			<DeviceNumber>k__BackingField = -1;
			base..ctor();
			syncContext = SynchronizationContext.get_Current();
			if (syncContext != null && (((MemberInfo)((object)syncContext).GetType()).get_Name() == "LegacyAspNetSynchronizationContext" || ((MemberInfo)((object)syncContext).GetType()).get_Name() == "AspNetSynchronizationContext"))
			{
				syncContext = null;
			}
			DesiredLatency = 300;
			NumberOfBuffers = 2;
			waveOutLock = new object();
		}

		public void Init(IWaveProvider waveProvider)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Expected O, but got Unknown
			if (playbackState != 0)
			{
				throw new InvalidOperationException("Can't re-initialize during playback");
			}
			if (hWaveOut != global::System.IntPtr.Zero)
			{
				DisposeBuffers();
				CloseWaveOut();
			}
			callbackEvent = new AutoResetEvent(false);
			waveStream = waveProvider;
			int bufferSize = waveProvider.WaveFormat.ConvertLatencyToByteSize((DesiredLatency + NumberOfBuffers - 1) / NumberOfBuffers);
			MmResult result;
			lock (waveOutLock)
			{
				result = WaveInterop.waveOutOpenWindow(out hWaveOut, (global::System.IntPtr)DeviceNumber, waveStream.WaveFormat, ((SafeHandle)((WaitHandle)callbackEvent).get_SafeWaitHandle()).DangerousGetHandle(), global::System.IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackEvent);
			}
			MmException.Try(result, "waveOutOpen");
			buffers = new WaveOutBuffer[NumberOfBuffers];
			playbackState = PlaybackState.Stopped;
			for (int i = 0; i < NumberOfBuffers; i++)
			{
				buffers[i] = new WaveOutBuffer(hWaveOut, bufferSize, waveStream, waveOutLock);
			}
		}

		public void Play()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Expected O, but got Unknown
			if (buffers == null || waveStream == null)
			{
				throw new InvalidOperationException("Must call Init first");
			}
			if (playbackState == PlaybackState.Stopped)
			{
				playbackState = PlaybackState.Playing;
				((EventWaitHandle)callbackEvent).Set();
				ThreadPool.QueueUserWorkItem((WaitCallback)delegate
				{
					PlaybackThread();
				}, (object)null);
			}
			else if (playbackState == PlaybackState.Paused)
			{
				Resume();
				((EventWaitHandle)callbackEvent).Set();
			}
		}

		private void PlaybackThread()
		{
			global::System.Exception e = null;
			try
			{
				DoPlayback();
			}
			catch (global::System.Exception ex)
			{
				e = ex;
			}
			finally
			{
				playbackState = PlaybackState.Stopped;
				RaisePlaybackStoppedEvent(e);
			}
		}

		private void DoPlayback()
		{
			while (playbackState != 0)
			{
				if (!((WaitHandle)callbackEvent).WaitOne(DesiredLatency))
				{
					_ = playbackState;
					_ = 1;
				}
				if (playbackState != PlaybackState.Playing)
				{
					continue;
				}
				int num = 0;
				WaveOutBuffer[] array = buffers;
				foreach (WaveOutBuffer waveOutBuffer in array)
				{
					if (waveOutBuffer.InQueue || waveOutBuffer.OnDone())
					{
						num++;
					}
				}
				if (num == 0)
				{
					playbackState = PlaybackState.Stopped;
					((EventWaitHandle)callbackEvent).Set();
				}
			}
		}

		public void Pause()
		{
			if (playbackState == PlaybackState.Playing)
			{
				playbackState = PlaybackState.Paused;
				MmResult mmResult;
				lock (waveOutLock)
				{
					mmResult = WaveInterop.waveOutPause(hWaveOut);
				}
				if (mmResult != 0)
				{
					throw new MmException(mmResult, "waveOutPause");
				}
			}
		}

		private void Resume()
		{
			if (playbackState == PlaybackState.Paused)
			{
				MmResult mmResult;
				lock (waveOutLock)
				{
					mmResult = WaveInterop.waveOutRestart(hWaveOut);
				}
				if (mmResult != 0)
				{
					throw new MmException(mmResult, "waveOutRestart");
				}
				playbackState = PlaybackState.Playing;
			}
		}

		public void Stop()
		{
			if (playbackState != 0)
			{
				playbackState = PlaybackState.Stopped;
				MmResult mmResult;
				lock (waveOutLock)
				{
					mmResult = WaveInterop.waveOutReset(hWaveOut);
				}
				if (mmResult != 0)
				{
					throw new MmException(mmResult, "waveOutReset");
				}
				((EventWaitHandle)callbackEvent).Set();
			}
		}

		public long GetPosition()
		{
			lock (waveOutLock)
			{
				MmTime mmTime = default(MmTime);
				mmTime.wType = 4u;
				MmException.Try(WaveInterop.waveOutGetPosition(hWaveOut, out mmTime, Marshal.SizeOf((object)mmTime)), "waveOutGetPosition");
				if (mmTime.wType != 4)
				{
					throw new global::System.Exception($"waveOutGetPosition: wType -> Expected {4}, Received {mmTime.wType}");
				}
				return mmTime.cb;
			}
		}

		public void Dispose()
		{
			GC.SuppressFinalize((object)this);
			Dispose(disposing: true);
		}

		protected void Dispose(bool disposing)
		{
			Stop();
			if (disposing)
			{
				DisposeBuffers();
			}
			CloseWaveOut();
		}

		private void CloseWaveOut()
		{
			if (callbackEvent != null)
			{
				((WaitHandle)callbackEvent).Close();
				callbackEvent = null;
			}
			lock (waveOutLock)
			{
				if (hWaveOut != global::System.IntPtr.Zero)
				{
					WaveInterop.waveOutClose(hWaveOut);
					hWaveOut = global::System.IntPtr.Zero;
				}
			}
		}

		private void DisposeBuffers()
		{
			if (buffers != null)
			{
				WaveOutBuffer[] array = buffers;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Dispose();
				}
				buffers = null;
			}
		}

		~WaveOutEvent()
		{
			Dispose(disposing: false);
		}

		private void RaisePlaybackStoppedEvent(global::System.Exception e)
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Expected O, but got Unknown
			<>c__DisplayClass44_0 CS$<>8__locals0 = new <>c__DisplayClass44_0();
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
	}
}

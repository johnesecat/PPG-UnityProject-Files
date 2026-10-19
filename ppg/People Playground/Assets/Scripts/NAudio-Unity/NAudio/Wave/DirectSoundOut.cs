using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace NAudio.Wave
{
	public class DirectSoundOut : IWavePlayer, global::System.IDisposable
	{
		[StructLayout(0, Pack = 2)]
		internal class BufferDescription
		{
			public int dwSize;

			[MarshalAs(8)]
			public DirectSoundBufferCaps dwFlags;

			public uint dwBufferBytes;

			public int dwReserved;

			public global::System.IntPtr lpwfxFormat;

			public Guid guidAlgo;
		}

		[StructLayout(0, Pack = 2)]
		internal class BufferCaps
		{
			public int dwSize;

			public int dwFlags;

			public int dwBufferBytes;

			public int dwUnlockTransferRate;

			public int dwPlayCpuOverhead;
		}

		internal enum DirectSoundCooperativeLevel : uint
		{
			DSSCL_NORMAL = 1u,
			DSSCL_PRIORITY,
			DSSCL_EXCLUSIVE,
			DSSCL_WRITEPRIMARY
		}

		[Flags]
		internal enum DirectSoundPlayFlags : uint
		{
			DSBPLAY_LOOPING = 1u,
			DSBPLAY_LOCHARDWARE = 2u,
			DSBPLAY_LOCSOFTWARE = 4u,
			DSBPLAY_TERMINATEBY_TIME = 8u,
			DSBPLAY_TERMINATEBY_DISTANCE = 0x10u,
			DSBPLAY_TERMINATEBY_PRIORITY = 0x20u
		}

		internal enum DirectSoundBufferLockFlag : uint
		{
			None,
			FromWriteCursor,
			EntireBuffer
		}

		[Flags]
		internal enum DirectSoundBufferStatus : uint
		{
			DSBSTATUS_PLAYING = 1u,
			DSBSTATUS_BUFFERLOST = 2u,
			DSBSTATUS_LOOPING = 4u,
			DSBSTATUS_LOCHARDWARE = 8u,
			DSBSTATUS_LOCSOFTWARE = 0x10u,
			DSBSTATUS_TERMINATED = 0x20u
		}

		[Flags]
		internal enum DirectSoundBufferCaps : uint
		{
			DSBCAPS_PRIMARYBUFFER = 1u,
			DSBCAPS_STATIC = 2u,
			DSBCAPS_LOCHARDWARE = 4u,
			DSBCAPS_LOCSOFTWARE = 8u,
			DSBCAPS_CTRL3D = 0x10u,
			DSBCAPS_CTRLFREQUENCY = 0x20u,
			DSBCAPS_CTRLPAN = 0x40u,
			DSBCAPS_CTRLVOLUME = 0x80u,
			DSBCAPS_CTRLPOSITIONNOTIFY = 0x100u,
			DSBCAPS_CTRLFX = 0x200u,
			DSBCAPS_STICKYFOCUS = 0x4000u,
			DSBCAPS_GLOBALFOCUS = 0x8000u,
			DSBCAPS_GETCURRENTPOSITION2 = 0x10000u,
			DSBCAPS_MUTE3DATMAXDISTANCE = 0x20000u,
			DSBCAPS_LOCDEFER = 0x40000u
		}

		internal struct DirectSoundBufferPositionNotify
		{
			public uint dwOffset;

			public global::System.IntPtr hEventNotify;
		}

		[ComImport]
		[Guid("279AFA83-4981-11CE-A521-0020AF0BE560")]
		[InterfaceType(/*Could not decode attribute arguments.*/)]
		[SuppressUnmanagedCodeSecurity]
		internal interface IDirectSound
		{
			void CreateSoundBuffer([In] BufferDescription desc, [MarshalAs(28)] out object dsDSoundBuffer, global::System.IntPtr pUnkOuter);

			void GetCaps(global::System.IntPtr caps);

			void DuplicateSoundBuffer([In][MarshalAs(28)] IDirectSoundBuffer bufferOriginal, [In][MarshalAs(28)] IDirectSoundBuffer bufferDuplicate);

			void SetCooperativeLevel(global::System.IntPtr HWND, [In][MarshalAs(8)] DirectSoundCooperativeLevel dwLevel);

			void Compact();

			void GetSpeakerConfig(global::System.IntPtr pdwSpeakerConfig);

			void SetSpeakerConfig(uint pdwSpeakerConfig);

			void Initialize([In][MarshalAs(43)] Guid guid);
		}

		[ComImport]
		[Guid("279AFA85-4981-11CE-A521-0020AF0BE560")]
		[InterfaceType(/*Could not decode attribute arguments.*/)]
		[SuppressUnmanagedCodeSecurity]
		internal interface IDirectSoundBuffer
		{
			void GetCaps([MarshalAs(43)] BufferCaps pBufferCaps);

			void GetCurrentPosition(out uint currentPlayCursor, out uint currentWriteCursor);

			void GetFormat();

			[return: MarshalAs(7)]
			int GetVolume();

			void GetPan(out uint pan);

			[return: MarshalAs(7)]
			int GetFrequency();

			[return: MarshalAs(8)]
			DirectSoundBufferStatus GetStatus();

			void Initialize([In][MarshalAs(28)] IDirectSound directSound, [In] BufferDescription desc);

			void Lock(int dwOffset, uint dwBytes, out global::System.IntPtr audioPtr1, out int audioBytes1, out global::System.IntPtr audioPtr2, out int audioBytes2, [MarshalAs(8)] DirectSoundBufferLockFlag dwFlags);

			void Play(uint dwReserved1, uint dwPriority, [In][MarshalAs(8)] DirectSoundPlayFlags dwFlags);

			void SetCurrentPosition(uint dwNewPosition);

			void SetFormat([In] WaveFormat pcfxFormat);

			void SetVolume(int volume);

			void SetPan(uint pan);

			void SetFrequency(uint frequency);

			void Stop();

			void Unlock(global::System.IntPtr pvAudioPtr1, int dwAudioBytes1, global::System.IntPtr pvAudioPtr2, int dwAudioBytes2);

			void Restore();
		}

		[ComImport]
		[Guid("b0210783-89cd-11d0-af08-00a0c925cd16")]
		[InterfaceType(/*Could not decode attribute arguments.*/)]
		[SuppressUnmanagedCodeSecurity]
		internal interface IDirectSoundNotify
		{
			void SetNotificationPositions(uint dwPositionNotifies, [In][MarshalAs(42)] DirectSoundBufferPositionNotify[] pcPositionNotifies);
		}

		private delegate bool DSEnumCallback(global::System.IntPtr lpGuid, global::System.IntPtr lpcstrDescription, global::System.IntPtr lpcstrModule, global::System.IntPtr lpContext);

		private PlaybackState playbackState;

		private WaveFormat waveFormat;

		private int samplesTotalSize;

		private int samplesFrameSize;

		private int nextSamplesWriteIndex;

		private int desiredLatency;

		private Guid device;

		private byte[] samples;

		private IWaveProvider waveStream;

		private IDirectSound directSound;

		private IDirectSoundBuffer primarySoundBuffer;

		private IDirectSoundBuffer secondaryBuffer;

		private EventWaitHandle frameEventWaitHandle1;

		private EventWaitHandle frameEventWaitHandle2;

		private EventWaitHandle endEventWaitHandle;

		private Thread notifyThread;

		private SynchronizationContext syncContext;

		private long bytesPlayed;

		private object m_LockObject = new object();

		private static List<DirectSoundDeviceInfo> devices;

		public static readonly Guid DSDEVID_DefaultPlayback = new Guid("DEF00000-9C6D-47ED-AAF1-4DDA8F2B5C03");

		public static readonly Guid DSDEVID_DefaultCapture = new Guid("DEF00001-9C6D-47ED-AAF1-4DDA8F2B5C03");

		public static readonly Guid DSDEVID_DefaultVoicePlayback = new Guid("DEF00002-9C6D-47ED-AAF1-4DDA8F2B5C03");

		public static readonly Guid DSDEVID_DefaultVoiceCapture = new Guid("DEF00003-9C6D-47ED-AAF1-4DDA8F2B5C03");

		public static global::System.Collections.Generic.IEnumerable<DirectSoundDeviceInfo> Devices
		{
			get
			{
				devices = new List<DirectSoundDeviceInfo>();
				DirectSoundEnumerate(EnumCallback, global::System.IntPtr.Zero);
				return (global::System.Collections.Generic.IEnumerable<DirectSoundDeviceInfo>)devices;
			}
		}

		public TimeSpan PlaybackPosition => TimeSpan.FromMilliseconds((double)(GetPosition() / (waveFormat.Channels * waveFormat.BitsPerSample / 8)) * 1000.0 / (double)waveFormat.SampleRate);

		public PlaybackState PlaybackState => playbackState;

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
					throw new InvalidOperationException("Setting volume not supported on DirectSoundOut, adjust the volume on your WaveProvider instead");
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

		private static bool EnumCallback(global::System.IntPtr lpGuid, global::System.IntPtr lpcstrDescription, global::System.IntPtr lpcstrModule, global::System.IntPtr lpContext)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			DirectSoundDeviceInfo directSoundDeviceInfo = new DirectSoundDeviceInfo();
			if (lpGuid == global::System.IntPtr.Zero)
			{
				directSoundDeviceInfo.Guid = Guid.Empty;
			}
			else
			{
				byte[] array = new byte[16];
				Marshal.Copy(lpGuid, array, 0, 16);
				directSoundDeviceInfo.Guid = new Guid(array);
			}
			directSoundDeviceInfo.Description = Marshal.PtrToStringAnsi(lpcstrDescription);
			directSoundDeviceInfo.ModuleName = Marshal.PtrToStringAnsi(lpcstrModule);
			devices.Add(directSoundDeviceInfo);
			return true;
		}

		public DirectSoundOut()
			: this(DSDEVID_DefaultPlayback)
		{
		}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


		public DirectSoundOut(Guid device)
			: this(device, 40)
		{
		}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


		public DirectSoundOut(int latency)
			: this(DSDEVID_DefaultPlayback, latency)
		{
		}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


		public DirectSoundOut(Guid device, int latency)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			if (device == Guid.Empty)
			{
				device = DSDEVID_DefaultPlayback;
			}
			this.device = device;
			desiredLatency = latency;
			syncContext = SynchronizationContext.get_Current();
		}

		~DirectSoundOut()
		{
			Dispose();
		}

		public void Play()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Expected O, but got Unknown
			if (playbackState == PlaybackState.Stopped)
			{
				notifyThread = new Thread(new ThreadStart(PlaybackThreadFunc));
				notifyThread.set_Priority((ThreadPriority)2);
				notifyThread.set_IsBackground(true);
				notifyThread.Start();
			}
			lock (m_LockObject)
			{
				playbackState = PlaybackState.Playing;
			}
		}

		public void Stop()
		{
			if (Monitor.TryEnter(m_LockObject, 50))
			{
				playbackState = PlaybackState.Stopped;
				Monitor.Exit(m_LockObject);
			}
			else if (notifyThread != null)
			{
				notifyThread.Abort();
				notifyThread = null;
			}
		}

		public void Pause()
		{
			lock (m_LockObject)
			{
				playbackState = PlaybackState.Paused;
			}
		}

		public long GetPosition()
		{
			if (playbackState != 0)
			{
				IDirectSoundBuffer directSoundBuffer = secondaryBuffer;
				if (directSoundBuffer != null)
				{
					directSoundBuffer.GetCurrentPosition(out var currentPlayCursor, out var _);
					return currentPlayCursor + bytesPlayed;
				}
			}
			return 0L;
		}

		public void Init(IWaveProvider waveProvider)
		{
			waveStream = waveProvider;
			waveFormat = waveProvider.WaveFormat;
		}

		private void InitializeDirectSound()
		{
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Expected O, but got Unknown
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Expected O, but got Unknown
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Expected O, but got Unknown
			lock (m_LockObject)
			{
				directSound = null;
				DirectSoundCreate(ref device, out directSound, global::System.IntPtr.Zero);
				if (directSound != null)
				{
					directSound.SetCooperativeLevel(GetDesktopWindow(), DirectSoundCooperativeLevel.DSSCL_PRIORITY);
					BufferDescription bufferDescription = new BufferDescription();
					bufferDescription.dwSize = Marshal.SizeOf((object)bufferDescription);
					bufferDescription.dwBufferBytes = 0u;
					bufferDescription.dwFlags = DirectSoundBufferCaps.DSBCAPS_PRIMARYBUFFER;
					bufferDescription.dwReserved = 0;
					bufferDescription.lpwfxFormat = global::System.IntPtr.Zero;
					bufferDescription.guidAlgo = Guid.Empty;
					directSound.CreateSoundBuffer(bufferDescription, out var dsDSoundBuffer, global::System.IntPtr.Zero);
					primarySoundBuffer = (IDirectSoundBuffer)dsDSoundBuffer;
					primarySoundBuffer.Play(0u, 0u, DirectSoundPlayFlags.DSBPLAY_LOOPING);
					samplesFrameSize = MsToBytes(desiredLatency);
					BufferDescription bufferDescription2 = new BufferDescription();
					bufferDescription2.dwSize = Marshal.SizeOf((object)bufferDescription2);
					bufferDescription2.dwBufferBytes = (uint)(samplesFrameSize * 2);
					bufferDescription2.dwFlags = (DirectSoundBufferCaps)115072u;
					bufferDescription2.dwReserved = 0;
					GCHandle val = GCHandle.Alloc((object)waveFormat, (GCHandleType)3);
					bufferDescription2.lpwfxFormat = ((GCHandle)(ref val)).AddrOfPinnedObject();
					bufferDescription2.guidAlgo = Guid.Empty;
					directSound.CreateSoundBuffer(bufferDescription2, out dsDSoundBuffer, global::System.IntPtr.Zero);
					secondaryBuffer = (IDirectSoundBuffer)dsDSoundBuffer;
					((GCHandle)(ref val)).Free();
					BufferCaps bufferCaps = new BufferCaps();
					bufferCaps.dwSize = Marshal.SizeOf((object)bufferCaps);
					secondaryBuffer.GetCaps(bufferCaps);
					nextSamplesWriteIndex = 0;
					samplesTotalSize = bufferCaps.dwBufferBytes;
					samples = new byte[samplesTotalSize];
					IDirectSoundNotify obj = (IDirectSoundNotify)dsDSoundBuffer;
					frameEventWaitHandle1 = new EventWaitHandle(false, (EventResetMode)0);
					frameEventWaitHandle2 = new EventWaitHandle(false, (EventResetMode)0);
					endEventWaitHandle = new EventWaitHandle(false, (EventResetMode)0);
					DirectSoundBufferPositionNotify[] array = new DirectSoundBufferPositionNotify[3]
					{
						default(DirectSoundBufferPositionNotify),
						default(DirectSoundBufferPositionNotify),
						default(DirectSoundBufferPositionNotify)
					};
					array[0].dwOffset = 0u;
					array[0].hEventNotify = ((SafeHandle)((WaitHandle)frameEventWaitHandle1).get_SafeWaitHandle()).DangerousGetHandle();
					array[1] = default(DirectSoundBufferPositionNotify);
					array[1].dwOffset = (uint)samplesFrameSize;
					array[1].hEventNotify = ((SafeHandle)((WaitHandle)frameEventWaitHandle2).get_SafeWaitHandle()).DangerousGetHandle();
					array[2] = default(DirectSoundBufferPositionNotify);
					array[2].dwOffset = 4294967295u;
					array[2].hEventNotify = ((SafeHandle)((WaitHandle)endEventWaitHandle).get_SafeWaitHandle()).DangerousGetHandle();
					obj.SetNotificationPositions(3u, array);
				}
			}
		}

		public void Dispose()
		{
			Stop();
			GC.SuppressFinalize((object)this);
		}

		private bool IsBufferLost()
		{
			if ((secondaryBuffer.GetStatus() & DirectSoundBufferStatus.DSBSTATUS_BUFFERLOST) == 0)
			{
				return false;
			}
			return true;
		}

		private int MsToBytes(int ms)
		{
			int num = ms * (waveFormat.AverageBytesPerSecond / 1000);
			return num - num % waveFormat.BlockAlign;
		}

		private void PlaybackThreadFunc()
		{
			bool flag = false;
			bool flag2 = false;
			bytesPlayed = 0L;
			global::System.Exception ex = null;
			try
			{
				InitializeDirectSound();
				int num = 1;
				if (PlaybackState == PlaybackState.Stopped)
				{
					secondaryBuffer.SetCurrentPosition(0u);
					nextSamplesWriteIndex = 0;
					num = Feed(samplesTotalSize);
				}
				if (num <= 0)
				{
					return;
				}
				lock (m_LockObject)
				{
					playbackState = PlaybackState.Playing;
				}
				secondaryBuffer.Play(0u, 0u, DirectSoundPlayFlags.DSBPLAY_LOOPING);
				WaitHandle[] array = (WaitHandle[])(object)new WaitHandle[3]
				{
					(WaitHandle)frameEventWaitHandle1,
					(WaitHandle)frameEventWaitHandle2,
					(WaitHandle)endEventWaitHandle
				};
				bool flag3 = true;
				while (PlaybackState != PlaybackState.Stopped && flag3)
				{
					int num2 = WaitHandle.WaitAny(array, 3 * desiredLatency, false);
					if (num2 != 258)
					{
						switch (num2)
						{
						case 2:
							StopPlayback();
							flag = true;
							flag3 = false;
							continue;
						case 0:
							if (flag2)
							{
								bytesPlayed += samplesFrameSize * 2;
							}
							break;
						default:
							flag2 = true;
							break;
						}
						num2 = ((num2 == 0) ? 1 : 0);
						nextSamplesWriteIndex = num2 * samplesFrameSize;
						if (Feed(samplesFrameSize) == 0)
						{
							StopPlayback();
							flag = true;
							flag3 = false;
						}
						continue;
					}
					StopPlayback();
					flag = true;
					flag3 = false;
					throw new global::System.Exception("DirectSound buffer timeout");
				}
			}
			catch (global::System.Exception ex2)
			{
				ex = ex2;
			}
			finally
			{
				if (!flag)
				{
					try
					{
						StopPlayback();
					}
					catch (global::System.Exception ex3)
					{
						if (ex == null)
						{
							ex = ex3;
						}
					}
				}
				lock (m_LockObject)
				{
					playbackState = PlaybackState.Stopped;
				}
				bytesPlayed = 0L;
				RaisePlaybackStopped(ex);
			}
		}

		private void RaisePlaybackStopped(global::System.Exception e)
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Expected O, but got Unknown
			<>c__DisplayClass48_0 CS$<>8__locals0 = new <>c__DisplayClass48_0();
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

		private void StopPlayback()
		{
			lock (m_LockObject)
			{
				if (secondaryBuffer != null)
				{
					CleanUpSecondaryBuffer();
					secondaryBuffer.Stop();
					secondaryBuffer = null;
				}
				if (primarySoundBuffer != null)
				{
					primarySoundBuffer.Stop();
					primarySoundBuffer = null;
				}
			}
		}

		private void CleanUpSecondaryBuffer()
		{
			if (secondaryBuffer == null)
			{
				return;
			}
			byte[] array = new byte[samplesTotalSize];
			secondaryBuffer.Lock(0, (uint)samplesTotalSize, out var audioPtr, out var audioBytes, out var audioPtr2, out var audioBytes2, DirectSoundBufferLockFlag.None);
			if (audioPtr != global::System.IntPtr.Zero)
			{
				Marshal.Copy(array, 0, audioPtr, audioBytes);
				if (audioPtr2 != global::System.IntPtr.Zero)
				{
					Marshal.Copy(array, 0, audioPtr, audioBytes);
				}
			}
			secondaryBuffer.Unlock(audioPtr, audioBytes, audioPtr2, audioBytes2);
		}

		private int Feed(int bytesToCopy)
		{
			int num = bytesToCopy;
			if (IsBufferLost())
			{
				secondaryBuffer.Restore();
			}
			if (playbackState == PlaybackState.Paused)
			{
				global::System.Array.Clear((global::System.Array)samples, 0, samples.Length);
			}
			else
			{
				num = waveStream.Read(samples, 0, bytesToCopy);
				if (num == 0)
				{
					global::System.Array.Clear((global::System.Array)samples, 0, samples.Length);
					return 0;
				}
			}
			secondaryBuffer.Lock(nextSamplesWriteIndex, (uint)num, out var audioPtr, out var audioBytes, out var audioPtr2, out var audioBytes2, DirectSoundBufferLockFlag.None);
			if (audioPtr != global::System.IntPtr.Zero)
			{
				Marshal.Copy(samples, 0, audioPtr, audioBytes);
				if (audioPtr2 != global::System.IntPtr.Zero)
				{
					Marshal.Copy(samples, 0, audioPtr, audioBytes);
				}
			}
			secondaryBuffer.Unlock(audioPtr, audioBytes, audioPtr2, audioBytes2);
			return num;
		}

		[DllImport("dsound.dll", CallingConvention = 3, CharSet = 3, ExactSpelling = true, SetLastError = true)]
		private static extern void DirectSoundCreate(ref Guid GUID, [MarshalAs(28)] out IDirectSound directSound, global::System.IntPtr pUnkOuter);

		[DllImport("dsound.dll", CallingConvention = 3, CharSet = 3, EntryPoint = "DirectSoundEnumerateA", ExactSpelling = true, SetLastError = true)]
		private static extern void DirectSoundEnumerate(DSEnumCallback lpDSEnumCallback, global::System.IntPtr lpContext);

		[DllImport("user32.dll")]
		private static extern global::System.IntPtr GetDesktopWindow();
	}
}

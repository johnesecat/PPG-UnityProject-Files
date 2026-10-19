using System;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi.Interfaces;
using NAudio.Utils;
using NAudio.Wave;

namespace NAudio.CoreAudioApi
{
	public class AudioClient : global::System.IDisposable
	{
		private IAudioClient audioClientInterface;

		private WaveFormat mixFormat;

		private AudioRenderClient audioRenderClient;

		private AudioCaptureClient audioCaptureClient;

		private AudioClockClient audioClockClient;

		private AudioStreamVolume audioStreamVolume;

		private AudioClientShareMode shareMode;

		public WaveFormat MixFormat
		{
			get
			{
				if (mixFormat == null)
				{
					Marshal.ThrowExceptionForHR(audioClientInterface.GetMixFormat(out var deviceFormatPointer));
					WaveFormat waveFormat = WaveFormat.MarshalFromPtr(deviceFormatPointer);
					Marshal.FreeCoTaskMem(deviceFormatPointer);
					mixFormat = waveFormat;
				}
				return mixFormat;
			}
		}

		public int BufferSize
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioClientInterface.GetBufferSize(out var bufferSize));
				return (int)bufferSize;
			}
		}

		public long StreamLatency => audioClientInterface.GetStreamLatency();

		public int CurrentPadding
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioClientInterface.GetCurrentPadding(out var currentPadding));
				return currentPadding;
			}
		}

		public long DefaultDevicePeriod
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioClientInterface.GetDevicePeriod(out var defaultDevicePeriod, out var _));
				return defaultDevicePeriod;
			}
		}

		public long MinimumDevicePeriod
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioClientInterface.GetDevicePeriod(out var _, out var minimumDevicePeriod));
				return minimumDevicePeriod;
			}
		}

		public AudioStreamVolume AudioStreamVolume
		{
			get
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				if (shareMode == AudioClientShareMode.Exclusive)
				{
					throw new InvalidOperationException("AudioStreamVolume is ONLY supported for shared audio streams.");
				}
				if (audioStreamVolume == null)
				{
					Guid interfaceId = default(Guid);
					((Guid)(ref interfaceId))..ctor("93014887-242D-4068-8A15-CF5E93B90FE3");
					Marshal.ThrowExceptionForHR(audioClientInterface.GetService(interfaceId, out var interfacePointer));
					audioStreamVolume = new AudioStreamVolume((IAudioStreamVolume)interfacePointer);
				}
				return audioStreamVolume;
			}
		}

		public AudioClockClient AudioClockClient
		{
			get
			{
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				if (audioClockClient == null)
				{
					Guid interfaceId = default(Guid);
					((Guid)(ref interfaceId))..ctor("CD63314F-3FBA-4a1b-812C-EF96358728E7");
					Marshal.ThrowExceptionForHR(audioClientInterface.GetService(interfaceId, out var interfacePointer));
					audioClockClient = new AudioClockClient((IAudioClock)interfacePointer);
				}
				return audioClockClient;
			}
		}

		public AudioRenderClient AudioRenderClient
		{
			get
			{
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				if (audioRenderClient == null)
				{
					Guid interfaceId = default(Guid);
					((Guid)(ref interfaceId))..ctor("F294ACFC-3146-4483-A7BF-ADDCA7C260E2");
					Marshal.ThrowExceptionForHR(audioClientInterface.GetService(interfaceId, out var interfacePointer));
					audioRenderClient = new AudioRenderClient((IAudioRenderClient)interfacePointer);
				}
				return audioRenderClient;
			}
		}

		public AudioCaptureClient AudioCaptureClient
		{
			get
			{
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				if (audioCaptureClient == null)
				{
					Guid interfaceId = default(Guid);
					((Guid)(ref interfaceId))..ctor("c8adbd64-e71e-48a0-a4de-185c395cd317");
					Marshal.ThrowExceptionForHR(audioClientInterface.GetService(interfaceId, out var interfacePointer));
					audioCaptureClient = new AudioCaptureClient((IAudioCaptureClient)interfacePointer);
				}
				return audioCaptureClient;
			}
		}

		internal AudioClient(IAudioClient audioClientInterface)
		{
			this.audioClientInterface = audioClientInterface;
		}

		public void Initialize(AudioClientShareMode shareMode, AudioClientStreamFlags streamFlags, long bufferDuration, long periodicity, WaveFormat waveFormat, Guid audioSessionGuid)
		{
			this.shareMode = shareMode;
			Marshal.ThrowExceptionForHR(audioClientInterface.Initialize(shareMode, streamFlags, bufferDuration, periodicity, waveFormat, ref audioSessionGuid));
			mixFormat = null;
		}

		public bool IsFormatSupported(AudioClientShareMode shareMode, WaveFormat desiredFormat)
		{
			WaveFormatExtensible closestMatchFormat;
			return IsFormatSupported(shareMode, desiredFormat, out closestMatchFormat);
		}

		private global::System.IntPtr GetPointerToPointer()
		{
			return Marshal.AllocHGlobal(MarshalHelpers.SizeOf<global::System.IntPtr>());
		}

		public bool IsFormatSupported(AudioClientShareMode shareMode, WaveFormat desiredFormat, out WaveFormatExtensible closestMatchFormat)
		{
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			global::System.IntPtr pointerToPointer = GetPointerToPointer();
			closestMatchFormat = null;
			int num = audioClientInterface.IsFormatSupported(shareMode, desiredFormat, pointerToPointer);
			global::System.IntPtr intPtr = MarshalHelpers.PtrToStructure<global::System.IntPtr>(pointerToPointer);
			if (intPtr != global::System.IntPtr.Zero)
			{
				closestMatchFormat = MarshalHelpers.PtrToStructure<WaveFormatExtensible>(intPtr);
				Marshal.FreeCoTaskMem(intPtr);
			}
			Marshal.FreeHGlobal(pointerToPointer);
			switch (num)
			{
			case 0:
				return true;
			case 1:
				return false;
			case -2004287480:
				return shareMode != AudioClientShareMode.Exclusive;
			default:
				Marshal.ThrowExceptionForHR(num);
				throw new NotSupportedException(string.Concat((object)"Unknown hresult ", (object)num));
			}
		}

		public void Start()
		{
			audioClientInterface.Start();
		}

		public void Stop()
		{
			audioClientInterface.Stop();
		}

		public void SetEventHandle(global::System.IntPtr eventWaitHandle)
		{
			audioClientInterface.SetEventHandle(eventWaitHandle);
		}

		public void Reset()
		{
			audioClientInterface.Reset();
		}

		public void Dispose()
		{
			if (audioClientInterface != null)
			{
				if (audioClockClient != null)
				{
					audioClockClient.Dispose();
					audioClockClient = null;
				}
				if (audioRenderClient != null)
				{
					audioRenderClient.Dispose();
					audioRenderClient = null;
				}
				if (audioCaptureClient != null)
				{
					audioCaptureClient.Dispose();
					audioCaptureClient = null;
				}
				if (audioStreamVolume != null)
				{
					audioStreamVolume.Dispose();
					audioStreamVolume = null;
				}
				Marshal.ReleaseComObject((object)audioClientInterface);
				audioClientInterface = null;
				GC.SuppressFinalize((object)this);
			}
		}
	}
}

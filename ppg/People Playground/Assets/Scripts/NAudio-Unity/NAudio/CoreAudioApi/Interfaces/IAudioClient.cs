using System;
using System.Runtime.InteropServices;
using NAudio.Wave;

namespace NAudio.CoreAudioApi.Interfaces
{
	[ComImport]
	[Guid("1CB9AD4C-DBFA-4c32-B178-C2F568A703B2")]
	[InterfaceType(/*Could not decode attribute arguments.*/)]
	internal interface IAudioClient
	{
		[PreserveSig]
		int Initialize(AudioClientShareMode shareMode, AudioClientStreamFlags streamFlags, long hnsBufferDuration, long hnsPeriodicity, [In] WaveFormat pFormat, [In] ref Guid audioSessionGuid);

		int GetBufferSize(out uint bufferSize);

		[return: MarshalAs(9)]
		long GetStreamLatency();

		int GetCurrentPadding(out int currentPadding);

		[PreserveSig]
		int IsFormatSupported(AudioClientShareMode shareMode, [In] WaveFormat pFormat, global::System.IntPtr closestMatchFormat);

		int GetMixFormat(out global::System.IntPtr deviceFormatPointer);

		int GetDevicePeriod(out long defaultDevicePeriod, out long minimumDevicePeriod);

		int Start();

		int Stop();

		int Reset();

		int SetEventHandle(global::System.IntPtr eventHandle);

		[PreserveSig]
		int GetService([In][MarshalAs(43)] Guid interfaceId, [MarshalAs(25)] out object interfacePointer);
	}
}

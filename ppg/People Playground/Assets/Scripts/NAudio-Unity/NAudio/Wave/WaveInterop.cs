using System;
using System.Runtime.InteropServices;

namespace NAudio.Wave
{
	internal class WaveInterop
	{
		[Flags]
		public enum WaveInOutOpenFlags
		{
			CallbackNull = 0,
			CallbackFunction = 196608,
			CallbackEvent = 327680,
			CallbackWindow = 0x10000,
			CallbackThread = 0x20000
		}

		public enum WaveMessage
		{
			WaveInOpen = 958,
			WaveInClose = 959,
			WaveInData = 960,
			WaveOutClose = 956,
			WaveOutDone = 957,
			WaveOutOpen = 955
		}

		public delegate void WaveCallback(global::System.IntPtr hWaveOut, WaveMessage message, global::System.IntPtr dwInstance, WaveHeader wavhdr, global::System.IntPtr dwReserved);

		[DllImport("winmm.dll")]
		public static extern int mmioStringToFOURCC([MarshalAs(20)] string s, int flags);

		[DllImport("winmm.dll")]
		public static extern int waveOutGetNumDevs();

		[DllImport("winmm.dll")]
		public static extern MmResult waveOutPrepareHeader(global::System.IntPtr hWaveOut, WaveHeader lpWaveOutHdr, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult waveOutUnprepareHeader(global::System.IntPtr hWaveOut, WaveHeader lpWaveOutHdr, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult waveOutWrite(global::System.IntPtr hWaveOut, WaveHeader lpWaveOutHdr, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult waveOutOpen(out global::System.IntPtr hWaveOut, global::System.IntPtr uDeviceID, WaveFormat lpFormat, WaveCallback dwCallback, global::System.IntPtr dwInstance, WaveInOutOpenFlags dwFlags);

		[DllImport("winmm.dll", EntryPoint = "waveOutOpen")]
		public static extern MmResult waveOutOpenWindow(out global::System.IntPtr hWaveOut, global::System.IntPtr uDeviceID, WaveFormat lpFormat, global::System.IntPtr callbackWindowHandle, global::System.IntPtr dwInstance, WaveInOutOpenFlags dwFlags);

		[DllImport("winmm.dll")]
		public static extern MmResult waveOutReset(global::System.IntPtr hWaveOut);

		[DllImport("winmm.dll")]
		public static extern MmResult waveOutClose(global::System.IntPtr hWaveOut);

		[DllImport("winmm.dll")]
		public static extern MmResult waveOutPause(global::System.IntPtr hWaveOut);

		[DllImport("winmm.dll")]
		public static extern MmResult waveOutRestart(global::System.IntPtr hWaveOut);

		[DllImport("winmm.dll")]
		public static extern MmResult waveOutGetPosition(global::System.IntPtr hWaveOut, out MmTime mmTime, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult waveOutSetVolume(global::System.IntPtr hWaveOut, int dwVolume);

		[DllImport("winmm.dll")]
		public static extern MmResult waveOutGetVolume(global::System.IntPtr hWaveOut, out int dwVolume);

		[DllImport("winmm.dll", CharSet = 4)]
		public static extern MmResult waveOutGetDevCaps(global::System.IntPtr deviceID, out WaveOutCapabilities waveOutCaps, int waveOutCapsSize);

		[DllImport("winmm.dll")]
		public static extern int waveInGetNumDevs();

		[DllImport("winmm.dll", CharSet = 4)]
		public static extern MmResult waveInGetDevCaps(global::System.IntPtr deviceID, out WaveInCapabilities waveInCaps, int waveInCapsSize);

		[DllImport("winmm.dll")]
		public static extern MmResult waveInAddBuffer(global::System.IntPtr hWaveIn, WaveHeader pwh, int cbwh);

		[DllImport("winmm.dll")]
		public static extern MmResult waveInClose(global::System.IntPtr hWaveIn);

		[DllImport("winmm.dll")]
		public static extern MmResult waveInOpen(out global::System.IntPtr hWaveIn, global::System.IntPtr uDeviceID, WaveFormat lpFormat, WaveCallback dwCallback, global::System.IntPtr dwInstance, WaveInOutOpenFlags dwFlags);

		[DllImport("winmm.dll", EntryPoint = "waveInOpen")]
		public static extern MmResult waveInOpenWindow(out global::System.IntPtr hWaveIn, global::System.IntPtr uDeviceID, WaveFormat lpFormat, global::System.IntPtr callbackWindowHandle, global::System.IntPtr dwInstance, WaveInOutOpenFlags dwFlags);

		[DllImport("winmm.dll")]
		public static extern MmResult waveInPrepareHeader(global::System.IntPtr hWaveIn, WaveHeader lpWaveInHdr, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult waveInUnprepareHeader(global::System.IntPtr hWaveIn, WaveHeader lpWaveInHdr, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult waveInReset(global::System.IntPtr hWaveIn);

		[DllImport("winmm.dll")]
		public static extern MmResult waveInStart(global::System.IntPtr hWaveIn);

		[DllImport("winmm.dll")]
		public static extern MmResult waveInStop(global::System.IntPtr hWaveIn);
	}
}

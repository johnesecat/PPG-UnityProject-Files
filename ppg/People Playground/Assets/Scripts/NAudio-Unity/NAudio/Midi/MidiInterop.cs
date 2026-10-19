using System;
using System.Runtime.InteropServices;

namespace NAudio.Midi
{
	internal class MidiInterop
	{
		public enum MidiInMessage
		{
			Open = 961,
			Close = 962,
			Data = 963,
			LongData = 964,
			Error = 965,
			LongError = 966,
			MoreData = 972
		}

		public enum MidiOutMessage
		{
			Open = 967,
			Close,
			Done
		}

		public delegate void MidiInCallback(global::System.IntPtr midiInHandle, MidiInMessage message, global::System.IntPtr userData, global::System.IntPtr messageParameter1, global::System.IntPtr messageParameter2);

		public delegate void MidiOutCallback(global::System.IntPtr midiInHandle, MidiOutMessage message, global::System.IntPtr userData, global::System.IntPtr messageParameter1, global::System.IntPtr messageParameter2);

		public struct MMTIME
		{
			public int wType;

			public int u;
		}

		public struct MIDIEVENT
		{
			public int dwDeltaTime;

			public int dwStreamID;

			public int dwEvent;

			[MarshalAs(30, SizeConst = 1)]
			public int dwParms;
		}

		public struct MIDIHDR
		{
			public global::System.IntPtr lpData;

			public int dwBufferLength;

			public int dwBytesRecorded;

			public global::System.IntPtr dwUser;

			public int dwFlags;

			public global::System.IntPtr lpNext;

			public global::System.IntPtr reserved;

			public int dwOffset;

			[MarshalAs(30, SizeConst = 8)]
			public global::System.IntPtr[] dwReserved;
		}

		public struct MIDIPROPTEMPO
		{
			public int cbStruct;

			public int dwTempo;
		}

		public const int CALLBACK_FUNCTION = 196608;

		public const int CALLBACK_NULL = 0;

		[DllImport("winmm.dll")]
		public static extern MmResult midiConnect(global::System.IntPtr hMidiIn, global::System.IntPtr hMidiOut, global::System.IntPtr pReserved);

		[DllImport("winmm.dll")]
		public static extern MmResult midiDisconnect(global::System.IntPtr hMidiIn, global::System.IntPtr hMidiOut, global::System.IntPtr pReserved);

		[DllImport("winmm.dll")]
		public static extern MmResult midiInAddBuffer(global::System.IntPtr hMidiIn, [MarshalAs(27)] ref MIDIHDR lpMidiInHdr, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult midiInClose(global::System.IntPtr hMidiIn);

		[DllImport("winmm.dll", CharSet = 4)]
		public static extern MmResult midiInGetDevCaps(global::System.IntPtr deviceId, out MidiInCapabilities capabilities, int size);

		[DllImport("winmm.dll")]
		public static extern MmResult midiInGetErrorText(int err, string lpText, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult midiInGetID(global::System.IntPtr hMidiIn, out int lpuDeviceId);

		[DllImport("winmm.dll")]
		public static extern int midiInGetNumDevs();

		[DllImport("winmm.dll")]
		public static extern MmResult midiInMessage(global::System.IntPtr hMidiIn, int msg, global::System.IntPtr dw1, global::System.IntPtr dw2);

		[DllImport("winmm.dll")]
		public static extern MmResult midiInOpen(out global::System.IntPtr hMidiIn, global::System.IntPtr uDeviceID, MidiInCallback callback, global::System.IntPtr dwInstance, int dwFlags);

		[DllImport("winmm.dll", EntryPoint = "midiInOpen")]
		public static extern MmResult midiInOpenWindow(out global::System.IntPtr hMidiIn, global::System.IntPtr uDeviceID, global::System.IntPtr callbackWindowHandle, global::System.IntPtr dwInstance, int dwFlags);

		[DllImport("winmm.dll")]
		public static extern MmResult midiInPrepareHeader(global::System.IntPtr hMidiIn, [MarshalAs(27)] ref MIDIHDR lpMidiInHdr, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult midiInReset(global::System.IntPtr hMidiIn);

		[DllImport("winmm.dll")]
		public static extern MmResult midiInStart(global::System.IntPtr hMidiIn);

		[DllImport("winmm.dll")]
		public static extern MmResult midiInStop(global::System.IntPtr hMidiIn);

		[DllImport("winmm.dll")]
		public static extern MmResult midiInUnprepareHeader(global::System.IntPtr hMidiIn, [MarshalAs(27)] ref MIDIHDR lpMidiInHdr, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutCacheDrumPatches(global::System.IntPtr hMidiOut, int uPatch, global::System.IntPtr lpKeyArray, int uFlags);

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutCachePatches(global::System.IntPtr hMidiOut, int uBank, global::System.IntPtr lpPatchArray, int uFlags);

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutClose(global::System.IntPtr hMidiOut);

		[DllImport("winmm.dll", CharSet = 4)]
		public static extern MmResult midiOutGetDevCaps(global::System.IntPtr deviceNumber, out MidiOutCapabilities caps, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutGetErrorText(global::System.IntPtr err, string lpText, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutGetID(global::System.IntPtr hMidiOut, out int lpuDeviceID);

		[DllImport("winmm.dll")]
		public static extern int midiOutGetNumDevs();

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutGetVolume(global::System.IntPtr uDeviceID, ref int lpdwVolume);

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutLongMsg(global::System.IntPtr hMidiOut, [MarshalAs(27)] ref MIDIHDR lpMidiOutHdr, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutMessage(global::System.IntPtr hMidiOut, int msg, global::System.IntPtr dw1, global::System.IntPtr dw2);

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutOpen(out global::System.IntPtr lphMidiOut, global::System.IntPtr uDeviceID, MidiOutCallback dwCallback, global::System.IntPtr dwInstance, int dwFlags);

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutPrepareHeader(global::System.IntPtr hMidiOut, [MarshalAs(27)] ref MIDIHDR lpMidiOutHdr, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutReset(global::System.IntPtr hMidiOut);

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutSetVolume(global::System.IntPtr hMidiOut, int dwVolume);

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutShortMsg(global::System.IntPtr hMidiOut, int dwMsg);

		[DllImport("winmm.dll")]
		public static extern MmResult midiOutUnprepareHeader(global::System.IntPtr hMidiOut, [MarshalAs(27)] ref MIDIHDR lpMidiOutHdr, int uSize);

		[DllImport("winmm.dll")]
		public static extern MmResult midiStreamClose(global::System.IntPtr hMidiStream);

		[DllImport("winmm.dll")]
		public static extern MmResult midiStreamOpen(out global::System.IntPtr hMidiStream, global::System.IntPtr puDeviceID, int cMidi, global::System.IntPtr dwCallback, global::System.IntPtr dwInstance, int fdwOpen);

		[DllImport("winmm.dll")]
		public static extern MmResult midiStreamOut(global::System.IntPtr hMidiStream, [MarshalAs(27)] ref MIDIHDR pmh, int cbmh);

		[DllImport("winmm.dll")]
		public static extern MmResult midiStreamPause(global::System.IntPtr hMidiStream);

		[DllImport("winmm.dll")]
		public static extern MmResult midiStreamPosition(global::System.IntPtr hMidiStream, [MarshalAs(27)] ref MMTIME lpmmt, int cbmmt);

		[DllImport("winmm.dll")]
		public static extern MmResult midiStreamProperty(global::System.IntPtr hMidiStream, global::System.IntPtr lppropdata, int dwProperty);

		[DllImport("winmm.dll")]
		public static extern MmResult midiStreamRestart(global::System.IntPtr hMidiStream);

		[DllImport("winmm.dll")]
		public static extern MmResult midiStreamStop(global::System.IntPtr hMidiStream);
	}
}

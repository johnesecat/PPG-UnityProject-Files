using System;
using System.Runtime.InteropServices;

namespace NAudio.Midi
{
	public class MidiOut : global::System.IDisposable
	{
		private global::System.IntPtr hMidiOut = global::System.IntPtr.Zero;

		private bool disposed;

		private MidiInterop.MidiOutCallback callback;

		public static int NumberOfDevices => MidiInterop.midiOutGetNumDevs();

		public int Volume
		{
			get
			{
				int lpdwVolume = 0;
				MmException.Try(MidiInterop.midiOutGetVolume(hMidiOut, ref lpdwVolume), "midiOutGetVolume");
				return lpdwVolume;
			}
			set
			{
				MmException.Try(MidiInterop.midiOutSetVolume(hMidiOut, value), "midiOutSetVolume");
			}
		}

		public static MidiOutCapabilities DeviceInfo(int midiOutDeviceNumber)
		{
			MidiOutCapabilities caps = default(MidiOutCapabilities);
			int uSize = Marshal.SizeOf((object)caps);
			MmException.Try(MidiInterop.midiOutGetDevCaps((global::System.IntPtr)midiOutDeviceNumber, out caps, uSize), "midiOutGetDevCaps");
			return caps;
		}

		public MidiOut(int deviceNo)
		{
			callback = Callback;
			MmException.Try(MidiInterop.midiOutOpen(out hMidiOut, (global::System.IntPtr)deviceNo, callback, global::System.IntPtr.Zero, 196608), "midiOutOpen");
		}

		public void Close()
		{
			Dispose();
		}

		public void Dispose()
		{
			GC.KeepAlive((object)callback);
			Dispose(disposing: true);
			GC.SuppressFinalize((object)this);
		}

		public void Reset()
		{
			MmException.Try(MidiInterop.midiOutReset(hMidiOut), "midiOutReset");
		}

		public void SendDriverMessage(int message, int param1, int param2)
		{
			MmException.Try(MidiInterop.midiOutMessage(hMidiOut, message, (global::System.IntPtr)param1, (global::System.IntPtr)param2), "midiOutMessage");
		}

		public void Send(int message)
		{
			MmException.Try(MidiInterop.midiOutShortMsg(hMidiOut, message), "midiOutShortMsg");
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				MidiInterop.midiOutClose(hMidiOut);
			}
			disposed = true;
		}

		private void Callback(global::System.IntPtr midiInHandle, MidiInterop.MidiOutMessage message, global::System.IntPtr userData, global::System.IntPtr messageParameter1, global::System.IntPtr messageParameter2)
		{
		}

		public void SendBuffer(byte[] byteBuffer)
		{
			MidiInterop.MIDIHDR lpMidiOutHdr = default(MidiInterop.MIDIHDR);
			lpMidiOutHdr.lpData = Marshal.AllocHGlobal(byteBuffer.Length);
			Marshal.Copy(byteBuffer, 0, lpMidiOutHdr.lpData, byteBuffer.Length);
			lpMidiOutHdr.dwBufferLength = byteBuffer.Length;
			lpMidiOutHdr.dwBytesRecorded = byteBuffer.Length;
			int uSize = Marshal.SizeOf((object)lpMidiOutHdr);
			MidiInterop.midiOutPrepareHeader(hMidiOut, ref lpMidiOutHdr, uSize);
			if (MidiInterop.midiOutLongMsg(hMidiOut, ref lpMidiOutHdr, uSize) != 0)
			{
				MidiInterop.midiOutUnprepareHeader(hMidiOut, ref lpMidiOutHdr, uSize);
			}
			Marshal.FreeHGlobal(lpMidiOutHdr.lpData);
		}

		~MidiOut()
		{
			Dispose(disposing: false);
		}
	}
}

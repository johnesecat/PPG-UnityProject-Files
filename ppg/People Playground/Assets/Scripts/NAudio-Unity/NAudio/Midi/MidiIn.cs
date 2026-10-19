using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace NAudio.Midi
{
	public class MidiIn : global::System.IDisposable
	{
		private global::System.IntPtr hMidiIn = global::System.IntPtr.Zero;

		private bool disposed;

		private MidiInterop.MidiInCallback callback;

		public static int NumberOfDevices => MidiInterop.midiInGetNumDevs();

		public event EventHandler<MidiInMessageEventArgs> MessageReceived
		{
			[CompilerGenerated]
			add
			{
				EventHandler<MidiInMessageEventArgs> val = this.MessageReceived;
				EventHandler<MidiInMessageEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<MidiInMessageEventArgs> val3 = (EventHandler<MidiInMessageEventArgs>)(object)global::System.Delegate.Combine((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<MidiInMessageEventArgs>>(ref this.MessageReceived, val3, val2);
				}
				while (val != val2);
			}
			[CompilerGenerated]
			remove
			{
				EventHandler<MidiInMessageEventArgs> val = this.MessageReceived;
				EventHandler<MidiInMessageEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<MidiInMessageEventArgs> val3 = (EventHandler<MidiInMessageEventArgs>)(object)global::System.Delegate.Remove((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<MidiInMessageEventArgs>>(ref this.MessageReceived, val3, val2);
				}
				while (val != val2);
			}
		}

		public event EventHandler<MidiInMessageEventArgs> ErrorReceived
		{
			[CompilerGenerated]
			add
			{
				EventHandler<MidiInMessageEventArgs> val = this.ErrorReceived;
				EventHandler<MidiInMessageEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<MidiInMessageEventArgs> val3 = (EventHandler<MidiInMessageEventArgs>)(object)global::System.Delegate.Combine((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<MidiInMessageEventArgs>>(ref this.ErrorReceived, val3, val2);
				}
				while (val != val2);
			}
			[CompilerGenerated]
			remove
			{
				EventHandler<MidiInMessageEventArgs> val = this.ErrorReceived;
				EventHandler<MidiInMessageEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<MidiInMessageEventArgs> val3 = (EventHandler<MidiInMessageEventArgs>)(object)global::System.Delegate.Remove((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<MidiInMessageEventArgs>>(ref this.ErrorReceived, val3, val2);
				}
				while (val != val2);
			}
		}

		public MidiIn(int deviceNo)
		{
			callback = Callback;
			MmException.Try(MidiInterop.midiInOpen(out hMidiIn, (global::System.IntPtr)deviceNo, callback, global::System.IntPtr.Zero, 196608), "midiInOpen");
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

		public void Start()
		{
			MmException.Try(MidiInterop.midiInStart(hMidiIn), "midiInStart");
		}

		public void Stop()
		{
			MmException.Try(MidiInterop.midiInStop(hMidiIn), "midiInStop");
		}

		public void Reset()
		{
			MmException.Try(MidiInterop.midiInReset(hMidiIn), "midiInReset");
		}

		private void Callback(global::System.IntPtr midiInHandle, MidiInterop.MidiInMessage message, global::System.IntPtr userData, global::System.IntPtr messageParameter1, global::System.IntPtr messageParameter2)
		{
			switch (message)
			{
			case MidiInterop.MidiInMessage.Data:
				if (this.MessageReceived != null)
				{
					this.MessageReceived.Invoke((object)this, new MidiInMessageEventArgs(messageParameter1.ToInt32(), messageParameter2.ToInt32()));
				}
				break;
			case MidiInterop.MidiInMessage.Error:
				if (this.ErrorReceived != null)
				{
					this.ErrorReceived.Invoke((object)this, new MidiInMessageEventArgs(messageParameter1.ToInt32(), messageParameter2.ToInt32()));
				}
				break;
			case MidiInterop.MidiInMessage.Open:
			case MidiInterop.MidiInMessage.Close:
			case MidiInterop.MidiInMessage.LongData:
			case MidiInterop.MidiInMessage.LongError:
			case (MidiInterop.MidiInMessage)967:
			case (MidiInterop.MidiInMessage)968:
			case (MidiInterop.MidiInMessage)969:
			case (MidiInterop.MidiInMessage)970:
			case (MidiInterop.MidiInMessage)971:
			case MidiInterop.MidiInMessage.MoreData:
				break;
			}
		}

		public static MidiInCapabilities DeviceInfo(int midiInDeviceNumber)
		{
			MidiInCapabilities capabilities = default(MidiInCapabilities);
			int size = Marshal.SizeOf((object)capabilities);
			MmException.Try(MidiInterop.midiInGetDevCaps((global::System.IntPtr)midiInDeviceNumber, out capabilities, size), "midiInGetDevCaps");
			return capabilities;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				MidiInterop.midiInClose(hMidiIn);
			}
			disposed = true;
		}

		~MidiIn()
		{
			Dispose(disposing: false);
		}
	}
}

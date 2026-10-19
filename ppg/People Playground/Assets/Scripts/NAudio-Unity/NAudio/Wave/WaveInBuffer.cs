using System;
using System.Runtime.InteropServices;

namespace NAudio.Wave
{
	internal class WaveInBuffer : global::System.IDisposable
	{
		private readonly WaveHeader header;

		private readonly int bufferSize;

		private readonly byte[] buffer;

		private GCHandle hBuffer;

		private global::System.IntPtr waveInHandle;

		private GCHandle hHeader;

		private GCHandle hThis;

		public byte[] Data => buffer;

		public bool Done => (header.flags & WaveHeaderFlags.Done) == WaveHeaderFlags.Done;

		public bool InQueue => (header.flags & WaveHeaderFlags.InQueue) == WaveHeaderFlags.InQueue;

		public int BytesRecorded => header.bytesRecorded;

		public int BufferSize => bufferSize;

		public WaveInBuffer(global::System.IntPtr waveInHandle, int bufferSize)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			this.bufferSize = bufferSize;
			buffer = new byte[bufferSize];
			hBuffer = GCHandle.Alloc((object)buffer, (GCHandleType)3);
			this.waveInHandle = waveInHandle;
			header = new WaveHeader();
			hHeader = GCHandle.Alloc((object)header, (GCHandleType)3);
			header.dataBuffer = ((GCHandle)(ref hBuffer)).AddrOfPinnedObject();
			header.bufferLength = bufferSize;
			header.loops = 1;
			hThis = GCHandle.Alloc((object)this);
			header.userData = (global::System.IntPtr)hThis;
			MmException.Try(WaveInterop.waveInPrepareHeader(waveInHandle, header, Marshal.SizeOf((object)header)), "waveInPrepareHeader");
		}

		public void Reuse()
		{
			MmException.Try(WaveInterop.waveInUnprepareHeader(waveInHandle, header, Marshal.SizeOf((object)header)), "waveUnprepareHeader");
			MmException.Try(WaveInterop.waveInPrepareHeader(waveInHandle, header, Marshal.SizeOf((object)header)), "waveInPrepareHeader");
			MmException.Try(WaveInterop.waveInAddBuffer(waveInHandle, header, Marshal.SizeOf((object)header)), "waveInAddBuffer");
		}

		~WaveInBuffer()
		{
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			GC.SuppressFinalize((object)this);
			Dispose(disposing: true);
		}

		protected void Dispose(bool disposing)
		{
			if (waveInHandle != global::System.IntPtr.Zero)
			{
				WaveInterop.waveInUnprepareHeader(waveInHandle, header, Marshal.SizeOf((object)header));
				waveInHandle = global::System.IntPtr.Zero;
			}
			if (((GCHandle)(ref hHeader)).get_IsAllocated())
			{
				((GCHandle)(ref hHeader)).Free();
			}
			if (((GCHandle)(ref hBuffer)).get_IsAllocated())
			{
				((GCHandle)(ref hBuffer)).Free();
			}
			if (((GCHandle)(ref hThis)).get_IsAllocated())
			{
				((GCHandle)(ref hThis)).Free();
			}
		}
	}
}

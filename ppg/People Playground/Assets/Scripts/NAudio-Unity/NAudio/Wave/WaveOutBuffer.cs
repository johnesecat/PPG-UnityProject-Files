using System;
using System.Runtime.InteropServices;

namespace NAudio.Wave
{
	internal class WaveOutBuffer : global::System.IDisposable
	{
		private readonly WaveHeader header;

		private readonly int bufferSize;

		private readonly byte[] buffer;

		private readonly IWaveProvider waveStream;

		private readonly object waveOutLock;

		private GCHandle hBuffer;

		private global::System.IntPtr hWaveOut;

		private GCHandle hHeader;

		private GCHandle hThis;

		public bool InQueue => (header.flags & WaveHeaderFlags.InQueue) == WaveHeaderFlags.InQueue;

		public int BufferSize => bufferSize;

		public WaveOutBuffer(global::System.IntPtr hWaveOut, int bufferSize, IWaveProvider bufferFillStream, object waveOutLock)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			this.bufferSize = bufferSize;
			buffer = new byte[bufferSize];
			hBuffer = GCHandle.Alloc((object)buffer, (GCHandleType)3);
			this.hWaveOut = hWaveOut;
			waveStream = bufferFillStream;
			this.waveOutLock = waveOutLock;
			header = new WaveHeader();
			hHeader = GCHandle.Alloc((object)header, (GCHandleType)3);
			header.dataBuffer = ((GCHandle)(ref hBuffer)).AddrOfPinnedObject();
			header.bufferLength = bufferSize;
			header.loops = 1;
			hThis = GCHandle.Alloc((object)this);
			header.userData = (global::System.IntPtr)hThis;
			lock (waveOutLock)
			{
				MmException.Try(WaveInterop.waveOutPrepareHeader(hWaveOut, header, Marshal.SizeOf((object)header)), "waveOutPrepareHeader");
			}
		}

		~WaveOutBuffer()
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
			if (hWaveOut != global::System.IntPtr.Zero)
			{
				lock (waveOutLock)
				{
					WaveInterop.waveOutUnprepareHeader(hWaveOut, header, Marshal.SizeOf((object)header));
				}
				hWaveOut = global::System.IntPtr.Zero;
			}
		}

		internal bool OnDone()
		{
			int num;
			lock (waveStream)
			{
				num = waveStream.Read(buffer, 0, buffer.Length);
			}
			if (num == 0)
			{
				return false;
			}
			for (int i = num; i < buffer.Length; i++)
			{
				buffer[i] = 0;
			}
			WriteToWaveOut();
			return true;
		}

		private void WriteToWaveOut()
		{
			MmResult mmResult;
			lock (waveOutLock)
			{
				mmResult = WaveInterop.waveOutWrite(hWaveOut, header, Marshal.SizeOf((object)header));
			}
			if (mmResult != 0)
			{
				throw new MmException(mmResult, "waveOutWrite");
			}
			GC.KeepAlive((object)this);
		}
	}
}

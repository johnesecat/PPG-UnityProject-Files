using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace NAudio.Wave
{
	internal class ComStream : Stream, IStream
	{
		private Stream stream;

		public override bool CanRead => stream.get_CanRead();

		public override bool CanSeek => stream.get_CanSeek();

		public override bool CanWrite => stream.get_CanWrite();

		public override long Length => stream.get_Length();

		public override long Position
		{
			get
			{
				return stream.get_Position();
			}
			set
			{
				stream.set_Position(value);
			}
		}

		public ComStream(Stream stream)
			: this(stream, synchronizeStream: true)
		{
		}

		internal ComStream(Stream stream, bool synchronizeStream)
			: this()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (synchronizeStream)
			{
				stream = Stream.Synchronized(stream);
			}
			this.stream = stream;
		}

		void IStream.Clone(out IStream ppstm)
		{
			ppstm = null;
		}

		void IStream.Commit(int grfCommitFlags)
		{
			stream.Flush();
		}

		void IStream.CopyTo(IStream pstm, long cb, global::System.IntPtr pcbRead, global::System.IntPtr pcbWritten)
		{
		}

		void IStream.LockRegion(long libOffset, long cb, int dwLockType)
		{
		}

		void IStream.Read(byte[] pv, int cb, global::System.IntPtr pcbRead)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (!((Stream)this).get_CanRead())
			{
				throw new InvalidOperationException("Stream is not readable.");
			}
			int num = ((Stream)this).Read(pv, 0, cb);
			if (pcbRead != global::System.IntPtr.Zero)
			{
				Marshal.WriteInt32(pcbRead, num);
			}
		}

		void IStream.Revert()
		{
		}

		void IStream.Seek(long dlibMove, int dwOrigin, global::System.IntPtr plibNewPosition)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			SeekOrigin val = (SeekOrigin)dwOrigin;
			long num = ((Stream)this).Seek(dlibMove, val);
			if (plibNewPosition != global::System.IntPtr.Zero)
			{
				Marshal.WriteInt64(plibNewPosition, num);
			}
		}

		void IStream.SetSize(long libNewSize)
		{
			((Stream)this).SetLength(libNewSize);
		}

		void IStream.Stat(out STATSTG pstatstg, int grfStatFlag)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			STATSTG val = default(STATSTG);
			val.type = 2;
			val.cbSize = ((Stream)this).get_Length();
			val.grfMode = 0;
			STATSTG val2 = val;
			if (((Stream)this).get_CanWrite() && ((Stream)this).get_CanRead())
			{
				val2.grfMode |= 2;
			}
			else if (((Stream)this).get_CanRead())
			{
				val2.grfMode |= 0;
			}
			else
			{
				if (!((Stream)this).get_CanWrite())
				{
					throw new ObjectDisposedException("Stream");
				}
				val2.grfMode |= 1;
			}
			pstatstg = val2;
		}

		void IStream.UnlockRegion(long libOffset, long cb, int dwLockType)
		{
		}

		void IStream.Write(byte[] pv, int cb, global::System.IntPtr pcbWritten)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (!((Stream)this).get_CanWrite())
			{
				throw new InvalidOperationException("Stream is not writeable.");
			}
			((Stream)this).Write(pv, 0, cb);
			if (pcbWritten != global::System.IntPtr.Zero)
			{
				Marshal.WriteInt32(pcbWritten, cb);
			}
		}

		public override void Flush()
		{
			stream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return stream.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return stream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			stream.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			stream.Write(buffer, offset, count);
		}

		protected override void Dispose(bool disposing)
		{
			((Stream)this).Dispose(disposing);
			if (stream != null)
			{
				stream.Dispose();
				stream = null;
			}
		}

		public override void Close()
		{
			((Stream)this).Close();
			if (stream != null)
			{
				stream.Close();
				stream = null;
			}
		}
	}
}

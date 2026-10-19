using System;
using System.IO;

namespace WatsonTcp
{
	public class WatsonStream : Stream
	{
		private readonly object _Lock = new object();

		private Stream _Stream;

		private long _Length;

		private long _Position;

		public override bool CanRead => true;

		public override bool CanSeek => false;

		public override bool CanWrite => false;

		public override long Length => _Length;

		public override long Position
		{
			get
			{
				return _Position;
			}
			set
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				throw new InvalidOperationException("Position may not be modified.");
			}
		}

		private long _BytesRemaining => _Length - _Position;

		internal WatsonStream(long contentLength, Stream stream)
			: this()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			if (contentLength < 0)
			{
				throw new ArgumentException("Content length must be zero or greater.");
			}
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.get_CanRead())
			{
				throw new ArgumentException("Cannot read from supplied stream.");
			}
			_Length = contentLength;
			_Stream = stream;
		}

		public override void Flush()
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentException("Offset must be zero or greater.");
			}
			if (offset >= buffer.Length)
			{
				throw new IndexOutOfRangeException("Offset must be less than the buffer length of " + buffer.Length + ".");
			}
			if (count < 0)
			{
				throw new ArgumentException("Count must be zero or greater.");
			}
			if (count == 0)
			{
				return 0;
			}
			if (count + offset > buffer.Length)
			{
				throw new ArgumentException("Offset and count must sum to a value less than the buffer length of " + buffer.Length + ".");
			}
			lock (_Lock)
			{
				byte[] array = null;
				if (_BytesRemaining == 0L)
				{
					return 0;
				}
				array = ((count <= _BytesRemaining) ? new byte[count] : new byte[_BytesRemaining]);
				int num = _Stream.Read(array, 0, array.Length);
				Buffer.BlockCopy((global::System.Array)array, 0, (global::System.Array)buffer, offset, num);
				_Position += num;
				return num;
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			throw new InvalidOperationException("Seek operations are not supported.");
		}

		public override void SetLength(long value)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			throw new InvalidOperationException("Length may not be modified.");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			throw new InvalidOperationException("Stream is not writeable.");
		}
	}
}

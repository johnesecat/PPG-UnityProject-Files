using System.IO;
using System.Runtime.CompilerServices;

namespace NAudio.Utils
{
	public class IgnoreDisposeStream : Stream
	{
		public Stream SourceStream
		{
			[CompilerGenerated]
			get
			{
				return <SourceStream>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<SourceStream>k__BackingField = value;
			}
		}

		public bool IgnoreDispose
		{
			[CompilerGenerated]
			get
			{
				return <IgnoreDispose>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<IgnoreDispose>k__BackingField = value;
			}
		}

		public override bool CanRead => SourceStream.get_CanRead();

		public override bool CanSeek => SourceStream.get_CanSeek();

		public override bool CanWrite => SourceStream.get_CanWrite();

		public override long Length => SourceStream.get_Length();

		public override long Position
		{
			get
			{
				return SourceStream.get_Position();
			}
			set
			{
				SourceStream.set_Position(value);
			}
		}

		public IgnoreDisposeStream(Stream sourceStream)
			: this()
		{
			SourceStream = sourceStream;
			IgnoreDispose = true;
		}

		public override void Flush()
		{
			SourceStream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return SourceStream.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return SourceStream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			SourceStream.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			SourceStream.Write(buffer, offset, count);
		}

		protected override void Dispose(bool disposing)
		{
			if (!IgnoreDispose)
			{
				SourceStream.Dispose();
				SourceStream = null;
			}
		}
	}
}

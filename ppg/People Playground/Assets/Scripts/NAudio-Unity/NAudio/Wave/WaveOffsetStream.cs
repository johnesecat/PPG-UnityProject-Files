using System;
using System.IO;

namespace NAudio.Wave
{
	public class WaveOffsetStream : WaveStream
	{
		private WaveStream sourceStream;

		private long audioStartPosition;

		private long sourceOffsetBytes;

		private long sourceLengthBytes;

		private long length;

		private readonly int bytesPerSample;

		private long position;

		private TimeSpan startTime;

		private TimeSpan sourceOffset;

		private TimeSpan sourceLength;

		private readonly object lockObject = new object();

		public TimeSpan StartTime
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return startTime;
			}
			set
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				lock (lockObject)
				{
					startTime = value;
					audioStartPosition = (long)(((TimeSpan)(ref startTime)).get_TotalSeconds() * (double)sourceStream.WaveFormat.SampleRate) * bytesPerSample;
					length = audioStartPosition + sourceLengthBytes;
					((Stream)this).set_Position(((Stream)this).get_Position());
				}
			}
		}

		public TimeSpan SourceOffset
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return sourceOffset;
			}
			set
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				lock (lockObject)
				{
					sourceOffset = value;
					sourceOffsetBytes = (long)(((TimeSpan)(ref sourceOffset)).get_TotalSeconds() * (double)sourceStream.WaveFormat.SampleRate) * bytesPerSample;
					((Stream)this).set_Position(((Stream)this).get_Position());
				}
			}
		}

		public TimeSpan SourceLength
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return sourceLength;
			}
			set
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				lock (lockObject)
				{
					sourceLength = value;
					sourceLengthBytes = (long)(((TimeSpan)(ref sourceLength)).get_TotalSeconds() * (double)sourceStream.WaveFormat.SampleRate) * bytesPerSample;
					length = audioStartPosition + sourceLengthBytes;
					((Stream)this).set_Position(((Stream)this).get_Position());
				}
			}
		}

		public override int BlockAlign => sourceStream.BlockAlign;

		public override long Length => length;

		public override long Position
		{
			get
			{
				return position;
			}
			set
			{
				lock (lockObject)
				{
					value -= value % BlockAlign;
					if (value < audioStartPosition)
					{
						((Stream)sourceStream).set_Position(sourceOffsetBytes);
					}
					else
					{
						((Stream)sourceStream).set_Position(sourceOffsetBytes + (value - audioStartPosition));
					}
					position = value;
				}
			}
		}

		public override WaveFormat WaveFormat => sourceStream.WaveFormat;

		public WaveOffsetStream(WaveStream sourceStream, TimeSpan startTime, TimeSpan sourceOffset, TimeSpan sourceLength)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			if (sourceStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
			{
				throw new ArgumentException("Only PCM supported");
			}
			this.sourceStream = sourceStream;
			position = 0L;
			bytesPerSample = sourceStream.WaveFormat.BitsPerSample / 8 * sourceStream.WaveFormat.Channels;
			StartTime = startTime;
			SourceOffset = sourceOffset;
			SourceLength = sourceLength;
		}

		public WaveOffsetStream(WaveStream sourceStream)
			: this(sourceStream, TimeSpan.Zero, TimeSpan.Zero, sourceStream.TotalTime)
		{
		}//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)


		public override int Read(byte[] destBuffer, int offset, int numBytes)
		{
			lock (lockObject)
			{
				int num = 0;
				if (position < audioStartPosition)
				{
					num = (int)Math.Min((long)numBytes, audioStartPosition - position);
					for (int i = 0; i < num; i++)
					{
						destBuffer[i + offset] = 0;
					}
				}
				if (num < numBytes)
				{
					int num2 = (int)Math.Min((long)(numBytes - num), sourceLengthBytes + sourceOffsetBytes - ((Stream)sourceStream).get_Position());
					int num3 = ((Stream)sourceStream).Read(destBuffer, num + offset, num2);
					num += num3;
				}
				for (int j = num; j < numBytes; j++)
				{
					destBuffer[offset + j] = 0;
				}
				position += numBytes;
				return numBytes;
			}
		}

		public override bool HasData(int count)
		{
			if (position + count < audioStartPosition)
			{
				return false;
			}
			if (position >= length)
			{
				return false;
			}
			return sourceStream.HasData(count);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && sourceStream != null)
			{
				((Stream)sourceStream).Dispose();
				sourceStream = null;
			}
			((Stream)this).Dispose(disposing);
		}
	}
}

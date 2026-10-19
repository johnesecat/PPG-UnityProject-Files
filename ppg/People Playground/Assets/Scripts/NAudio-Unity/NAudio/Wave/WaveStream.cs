using System;
using System.IO;

namespace NAudio.Wave
{
	public abstract class WaveStream : Stream, IWaveProvider
	{
		public abstract WaveFormat WaveFormat { get; }

		public override bool CanRead => true;

		public override bool CanSeek => true;

		public override bool CanWrite => false;

		public virtual int BlockAlign => WaveFormat.BlockAlign;

		public virtual TimeSpan CurrentTime
		{
			get
			{
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				return TimeSpan.FromSeconds((double)((Stream)this).get_Position() / (double)WaveFormat.AverageBytesPerSecond);
			}
			set
			{
				((Stream)this).set_Position((long)(((TimeSpan)(ref value)).get_TotalSeconds() * (double)WaveFormat.AverageBytesPerSecond));
			}
		}

		public virtual TimeSpan TotalTime => TimeSpan.FromSeconds((double)((Stream)this).get_Length() / (double)WaveFormat.AverageBytesPerSecond);

		public override void Flush()
		{
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Invalid comparison between Unknown and I4
			if ((int)origin == 0)
			{
				((Stream)this).set_Position(offset);
			}
			else if ((int)origin == 1)
			{
				((Stream)this).set_Position(((Stream)this).get_Position() + offset);
			}
			else
			{
				((Stream)this).set_Position(((Stream)this).get_Length() + offset);
			}
			return ((Stream)this).get_Position();
		}

		public override void SetLength(long length)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			throw new NotSupportedException("Can't set length of a WaveFormatString");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			throw new NotSupportedException("Can't write to a WaveFormatString");
		}

		public void Skip(int seconds)
		{
			long num = ((Stream)this).get_Position() + WaveFormat.AverageBytesPerSecond * seconds;
			if (num > ((Stream)this).get_Length())
			{
				((Stream)this).set_Position(((Stream)this).get_Length());
			}
			else if (num < 0)
			{
				((Stream)this).set_Position(0L);
			}
			else
			{
				((Stream)this).set_Position(num);
			}
		}

		public virtual bool HasData(int count)
		{
			return ((Stream)this).get_Position() < ((Stream)this).get_Length();
		}

		protected WaveStream()
			: this()
		{
		}
	}
}

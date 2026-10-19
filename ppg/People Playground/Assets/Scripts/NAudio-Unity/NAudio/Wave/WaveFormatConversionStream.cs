using System;
using System.IO;
using NAudio.Wave.Compression;

namespace NAudio.Wave
{
	public class WaveFormatConversionStream : WaveStream
	{
		private readonly WaveFormatConversionProvider conversionProvider;

		private readonly WaveFormat targetFormat;

		private readonly long length;

		private long position;

		private readonly WaveStream sourceStream;

		private bool isDisposed;

		public override long Position
		{
			get
			{
				return position;
			}
			set
			{
				value -= value % BlockAlign;
				long num = EstimateDestToSource(value);
				((Stream)sourceStream).set_Position(num);
				position = EstimateSourceToDest(((Stream)sourceStream).get_Position());
				conversionProvider.Reposition();
			}
		}

		public override long Length => length;

		public override WaveFormat WaveFormat => targetFormat;

		public WaveFormatConversionStream(WaveFormat targetFormat, WaveStream sourceStream)
		{
			this.sourceStream = sourceStream;
			this.targetFormat = targetFormat;
			conversionProvider = new WaveFormatConversionProvider(targetFormat, sourceStream);
			length = EstimateSourceToDest((int)((Stream)sourceStream).get_Length());
			position = 0L;
		}

		public static WaveStream CreatePcmStream(WaveStream sourceStream)
		{
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			if (sourceStream.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
			{
				return sourceStream;
			}
			WaveFormat waveFormat = AcmStream.SuggestPcmFormat(sourceStream.WaveFormat);
			if (waveFormat.SampleRate < 8000)
			{
				if (sourceStream.WaveFormat.Encoding != WaveFormatEncoding.G723)
				{
					throw new InvalidOperationException("Invalid suggested output format, please explicitly provide a target format");
				}
				waveFormat = new WaveFormat(8000, 16, 1);
			}
			return new WaveFormatConversionStream(waveFormat, sourceStream);
		}

		[Obsolete("can be unreliable, use of this method not encouraged")]
		public int SourceToDest(int source)
		{
			return (int)EstimateSourceToDest(source);
		}

		private long EstimateSourceToDest(long source)
		{
			long num = source * targetFormat.AverageBytesPerSecond / sourceStream.WaveFormat.AverageBytesPerSecond;
			return num - num % targetFormat.BlockAlign;
		}

		private long EstimateDestToSource(long dest)
		{
			long num = dest * sourceStream.WaveFormat.AverageBytesPerSecond / targetFormat.AverageBytesPerSecond;
			return (int)(num - num % sourceStream.WaveFormat.BlockAlign);
		}

		[Obsolete("can be unreliable, use of this method not encouraged")]
		public int DestToSource(int dest)
		{
			return (int)EstimateDestToSource(dest);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int num = conversionProvider.Read(buffer, offset, count);
			position += num;
			return num;
		}

		protected override void Dispose(bool disposing)
		{
			if (!isDisposed)
			{
				isDisposed = true;
				if (disposing)
				{
					((Stream)sourceStream).Dispose();
					conversionProvider.Dispose();
				}
			}
			((Stream)this).Dispose(disposing);
		}
	}
}

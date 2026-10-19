using System;
using System.Runtime.CompilerServices;
using NAudio.Wave.Compression;

namespace NAudio.Wave
{
	public class WaveFormatConversionProvider : IWaveProvider, global::System.IDisposable
	{
		private readonly AcmStream conversionStream;

		private readonly IWaveProvider sourceProvider;

		private readonly int preferredSourceReadSize;

		private int leftoverDestBytes;

		private int leftoverDestOffset;

		private int leftoverSourceBytes;

		private bool isDisposed;

		public WaveFormat WaveFormat
		{
			[CompilerGenerated]
			get
			{
				return <WaveFormat>k__BackingField;
			}
		}

		public WaveFormatConversionProvider(WaveFormat targetFormat, IWaveProvider sourceProvider)
		{
			this.sourceProvider = sourceProvider;
			<WaveFormat>k__BackingField = targetFormat;
			conversionStream = new AcmStream(sourceProvider.WaveFormat, targetFormat);
			preferredSourceReadSize = Math.Min(sourceProvider.WaveFormat.AverageBytesPerSecond, conversionStream.SourceBuffer.Length);
			preferredSourceReadSize -= preferredSourceReadSize % sourceProvider.WaveFormat.BlockAlign;
		}

		public void Reposition()
		{
			leftoverDestBytes = 0;
			leftoverDestOffset = 0;
			leftoverSourceBytes = 0;
			conversionStream.Reposition();
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			int i = 0;
			if (count % WaveFormat.BlockAlign != 0)
			{
				count -= count % WaveFormat.BlockAlign;
			}
			int num5;
			for (; i < count; i += num5)
			{
				int num = Math.Min(count - i, leftoverDestBytes);
				if (num > 0)
				{
					global::System.Array.Copy((global::System.Array)conversionStream.DestBuffer, leftoverDestOffset, (global::System.Array)buffer, offset + i, num);
					leftoverDestOffset += num;
					leftoverDestBytes -= num;
					i += num;
				}
				if (i >= count)
				{
					break;
				}
				int count2 = Math.Min(preferredSourceReadSize, conversionStream.SourceBuffer.Length - leftoverSourceBytes);
				int num2 = sourceProvider.Read(conversionStream.SourceBuffer, leftoverSourceBytes, count2) + leftoverSourceBytes;
				if (num2 == 0)
				{
					break;
				}
				int sourceBytesConverted;
				int num3 = conversionStream.Convert(num2, out sourceBytesConverted);
				if (sourceBytesConverted == 0)
				{
					break;
				}
				leftoverSourceBytes = num2 - sourceBytesConverted;
				if (leftoverSourceBytes > 0)
				{
					Buffer.BlockCopy((global::System.Array)conversionStream.SourceBuffer, sourceBytesConverted, (global::System.Array)conversionStream.SourceBuffer, 0, leftoverSourceBytes);
				}
				if (num3 <= 0)
				{
					break;
				}
				int num4 = count - i;
				num5 = Math.Min(num3, num4);
				if (num5 < num3)
				{
					leftoverDestBytes = num3 - num5;
					leftoverDestOffset = num5;
				}
				global::System.Array.Copy((global::System.Array)conversionStream.DestBuffer, 0, (global::System.Array)buffer, i + offset, num5);
			}
			return i;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!isDisposed)
			{
				isDisposed = true;
				conversionStream?.Dispose();
			}
		}

		public void Dispose()
		{
			GC.SuppressFinalize((object)this);
			Dispose(disposing: true);
		}

		~WaveFormatConversionProvider()
		{
			Dispose(disposing: false);
		}
	}
}

using System;
using NAudio.Wave.Compression;

namespace NAudio.Wave
{
	public class AcmMp3FrameDecompressor : IMp3FrameDecompressor, global::System.IDisposable
	{
		private readonly AcmStream conversionStream;

		private readonly WaveFormat pcmFormat;

		private bool disposed;

		public WaveFormat OutputFormat => pcmFormat;

		public AcmMp3FrameDecompressor(WaveFormat sourceFormat)
		{
			pcmFormat = AcmStream.SuggestPcmFormat(sourceFormat);
			try
			{
				conversionStream = new AcmStream(sourceFormat, pcmFormat);
			}
			catch (global::System.Exception)
			{
				disposed = true;
				GC.SuppressFinalize((object)this);
				throw;
			}
		}

		public int DecompressFrame(Mp3Frame frame, byte[] dest, int destOffset)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			if (frame == null)
			{
				throw new ArgumentNullException("frame", "You must provide a non-null Mp3Frame to decompress");
			}
			global::System.Array.Copy((global::System.Array)frame.RawData, (global::System.Array)conversionStream.SourceBuffer, frame.FrameLength);
			int sourceBytesConverted = 0;
			int num = conversionStream.Convert(frame.FrameLength, out sourceBytesConverted);
			if (sourceBytesConverted != frame.FrameLength)
			{
				throw new InvalidOperationException($"Couldn't convert the whole MP3 frame (converted {sourceBytesConverted}/{frame.FrameLength})");
			}
			global::System.Array.Copy((global::System.Array)conversionStream.DestBuffer, 0, (global::System.Array)dest, destOffset, num);
			return num;
		}

		public void Reset()
		{
			conversionStream.Reposition();
		}

		public void Dispose()
		{
			if (!disposed)
			{
				disposed = true;
				if (conversionStream != null)
				{
					conversionStream.Dispose();
				}
				GC.SuppressFinalize((object)this);
			}
		}

		~AcmMp3FrameDecompressor()
		{
			Dispose();
		}
	}
}

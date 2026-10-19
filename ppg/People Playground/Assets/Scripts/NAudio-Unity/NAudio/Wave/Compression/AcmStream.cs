using System;
using System.Runtime.InteropServices;

namespace NAudio.Wave.Compression
{
	public class AcmStream : global::System.IDisposable
	{
		private global::System.IntPtr streamHandle;

		private global::System.IntPtr driverHandle;

		private AcmStreamHeader streamHeader;

		private readonly WaveFormat sourceFormat;

		public byte[] SourceBuffer => streamHeader.SourceBuffer;

		public byte[] DestBuffer => streamHeader.DestBuffer;

		public AcmStream(WaveFormat sourceFormat, WaveFormat destFormat)
		{
			try
			{
				streamHandle = global::System.IntPtr.Zero;
				this.sourceFormat = sourceFormat;
				int num = Math.Max(65536, sourceFormat.AverageBytesPerSecond);
				num -= num % sourceFormat.BlockAlign;
				MmException.Try(AcmInterop.acmStreamOpen(out streamHandle, global::System.IntPtr.Zero, sourceFormat, destFormat, null, global::System.IntPtr.Zero, global::System.IntPtr.Zero, AcmStreamOpenFlags.NonRealTime), "acmStreamOpen");
				int destBufferLength = SourceToDest(num);
				streamHeader = new AcmStreamHeader(streamHandle, num, destBufferLength);
				driverHandle = global::System.IntPtr.Zero;
			}
			catch
			{
				Dispose();
				throw;
			}
		}

		public AcmStream(global::System.IntPtr driverId, WaveFormat sourceFormat, WaveFilter waveFilter)
		{
			int num = Math.Max(16384, sourceFormat.AverageBytesPerSecond);
			this.sourceFormat = sourceFormat;
			num -= num % sourceFormat.BlockAlign;
			MmException.Try(AcmInterop.acmDriverOpen(out driverHandle, driverId, 0), "acmDriverOpen");
			MmException.Try(AcmInterop.acmStreamOpen(out streamHandle, driverHandle, sourceFormat, sourceFormat, waveFilter, global::System.IntPtr.Zero, global::System.IntPtr.Zero, AcmStreamOpenFlags.NonRealTime), "acmStreamOpen");
			streamHeader = new AcmStreamHeader(streamHandle, num, SourceToDest(num));
		}

		public int SourceToDest(int source)
		{
			if (source == 0)
			{
				return 0;
			}
			MmException.Try(AcmInterop.acmStreamSize(streamHandle, source, out var outputBufferSize, AcmStreamSizeFlags.Source), "acmStreamSize");
			return outputBufferSize;
		}

		public int DestToSource(int dest)
		{
			if (dest == 0)
			{
				return 0;
			}
			MmException.Try(AcmInterop.acmStreamSize(streamHandle, dest, out var outputBufferSize, AcmStreamSizeFlags.Destination), "acmStreamSize");
			return outputBufferSize;
		}

		public static WaveFormat SuggestPcmFormat(WaveFormat compressedFormat)
		{
			WaveFormat waveFormat = new WaveFormat(compressedFormat.SampleRate, 16, compressedFormat.Channels);
			MmException.Try(AcmInterop.acmFormatSuggest(global::System.IntPtr.Zero, compressedFormat, waveFormat, Marshal.SizeOf((object)waveFormat), AcmFormatSuggestFlags.FormatTag), "acmFormatSuggest");
			return waveFormat;
		}

		public void Reposition()
		{
			streamHeader.Reposition();
		}

		public int Convert(int bytesToConvert, out int sourceBytesConverted)
		{
			if (bytesToConvert % sourceFormat.BlockAlign != 0)
			{
				bytesToConvert -= bytesToConvert % sourceFormat.BlockAlign;
			}
			return streamHeader.Convert(bytesToConvert, out sourceBytesConverted);
		}

		[Obsolete("Call the version returning sourceBytesConverted instead")]
		public int Convert(int bytesToConvert)
		{
			int sourceBytesConverted;
			int result = Convert(bytesToConvert, out sourceBytesConverted);
			if (sourceBytesConverted != bytesToConvert)
			{
				throw new MmException(MmResult.NotSupported, "AcmStreamHeader.Convert didn't convert everything");
			}
			return result;
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize((object)this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && streamHeader != null)
			{
				streamHeader.Dispose();
				streamHeader = null;
			}
			if (streamHandle != global::System.IntPtr.Zero)
			{
				MmResult mmResult = AcmInterop.acmStreamClose(streamHandle, 0);
				streamHandle = global::System.IntPtr.Zero;
				if (mmResult != 0)
				{
					throw new MmException(mmResult, "acmStreamClose");
				}
			}
			if (driverHandle != global::System.IntPtr.Zero)
			{
				AcmInterop.acmDriverClose(driverHandle, 0);
				driverHandle = global::System.IntPtr.Zero;
			}
		}

		~AcmStream()
		{
			Dispose(disposing: false);
		}
	}
}

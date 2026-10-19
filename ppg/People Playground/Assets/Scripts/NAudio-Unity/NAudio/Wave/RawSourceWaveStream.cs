using System.IO;

namespace NAudio.Wave
{
	public class RawSourceWaveStream : WaveStream
	{
		private readonly Stream sourceStream;

		private readonly WaveFormat waveFormat;

		public override WaveFormat WaveFormat => waveFormat;

		public override long Length => sourceStream.get_Length();

		public override long Position
		{
			get
			{
				return sourceStream.get_Position();
			}
			set
			{
				sourceStream.set_Position(value - value % waveFormat.BlockAlign);
			}
		}

		public RawSourceWaveStream(Stream sourceStream, WaveFormat waveFormat)
		{
			this.sourceStream = sourceStream;
			this.waveFormat = waveFormat;
		}

		public RawSourceWaveStream(byte[] byteStream, int offset, int count, WaveFormat waveFormat)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Expected O, but got Unknown
			sourceStream = (Stream)new MemoryStream(byteStream, offset, count);
			this.waveFormat = waveFormat;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			try
			{
				return sourceStream.Read(buffer, offset, count);
			}
			catch (EndOfStreamException)
			{
				return 0;
			}
		}
	}
}

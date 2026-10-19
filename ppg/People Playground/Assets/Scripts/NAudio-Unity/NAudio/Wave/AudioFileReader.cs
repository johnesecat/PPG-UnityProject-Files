using System;
using System.IO;
using System.Runtime.CompilerServices;
using NAudio.Wave.SampleProviders;

namespace NAudio.Wave
{
	public class AudioFileReader : WaveStream, ISampleProvider
	{
		private WaveStream readerStream;

		private readonly SampleChannel sampleChannel;

		private readonly int destBytesPerSample;

		private readonly int sourceBytesPerSample;

		private readonly long length;

		private readonly object lockObject;

		public string FileName
		{
			[CompilerGenerated]
			get
			{
				return <FileName>k__BackingField;
			}
		}

		public override WaveFormat WaveFormat => sampleChannel.WaveFormat;

		public override long Length => length;

		public override long Position
		{
			get
			{
				return SourceToDest(((Stream)readerStream).get_Position());
			}
			set
			{
				lock (lockObject)
				{
					((Stream)readerStream).set_Position(DestToSource(value));
				}
			}
		}

		public float Volume
		{
			get
			{
				return sampleChannel.Volume;
			}
			set
			{
				sampleChannel.Volume = value;
			}
		}

		public AudioFileReader(string fileName)
		{
			lockObject = new object();
			<FileName>k__BackingField = fileName;
			CreateReaderStream(fileName);
			sourceBytesPerSample = readerStream.WaveFormat.BitsPerSample / 8 * readerStream.WaveFormat.Channels;
			sampleChannel = new SampleChannel(readerStream, forceStereo: false);
			destBytesPerSample = 4 * sampleChannel.WaveFormat.Channels;
			length = SourceToDest(((Stream)readerStream).get_Length());
		}

		private void CreateReaderStream(string fileName)
		{
			if (fileName.EndsWith(".wav", (StringComparison)5))
			{
				readerStream = new WaveFileReader(fileName);
				if (readerStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm && readerStream.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
				{
					readerStream = WaveFormatConversionStream.CreatePcmStream(readerStream);
					readerStream = new BlockAlignReductionStream(readerStream);
				}
			}
			else if (fileName.EndsWith(".mp3", (StringComparison)5))
			{
				readerStream = new Mp3FileReader(fileName);
			}
			else if (fileName.EndsWith(".aiff", (StringComparison)5) || fileName.EndsWith(".aif", (StringComparison)5))
			{
				readerStream = new AiffFileReader(fileName);
			}
			else
			{
				readerStream = new MediaFoundationReader(fileName);
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			WaveBuffer waveBuffer = new WaveBuffer(buffer);
			int count2 = count / 4;
			return Read(waveBuffer.FloatBuffer, offset / 4, count2) * 4;
		}

		public int Read(float[] buffer, int offset, int count)
		{
			lock (lockObject)
			{
				return sampleChannel.Read(buffer, offset, count);
			}
		}

		private long SourceToDest(long sourceBytes)
		{
			return destBytesPerSample * (sourceBytes / sourceBytesPerSample);
		}

		private long DestToSource(long destBytes)
		{
			return sourceBytesPerSample * (destBytes / destBytesPerSample);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && readerStream != null)
			{
				((Stream)readerStream).Dispose();
				readerStream = null;
			}
			((Stream)this).Dispose(disposing);
		}
	}
}

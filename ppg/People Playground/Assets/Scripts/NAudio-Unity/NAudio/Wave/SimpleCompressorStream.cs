using System;
using System.IO;
using System.Runtime.CompilerServices;
using NAudio.Dsp;

namespace NAudio.Wave
{
	public class SimpleCompressorStream : WaveStream
	{
		private WaveStream sourceStream;

		private readonly SimpleCompressor simpleCompressor;

		private byte[] sourceBuffer;

		private readonly int channels;

		private readonly int bytesPerSample;

		private readonly object lockObject = new object();

		public double MakeUpGain
		{
			get
			{
				return simpleCompressor.MakeUpGain;
			}
			set
			{
				lock (lockObject)
				{
					simpleCompressor.MakeUpGain = value;
				}
			}
		}

		public double Threshold
		{
			get
			{
				return simpleCompressor.Threshold;
			}
			set
			{
				lock (lockObject)
				{
					simpleCompressor.Threshold = value;
				}
			}
		}

		public double Ratio
		{
			get
			{
				return simpleCompressor.Ratio;
			}
			set
			{
				lock (lockObject)
				{
					simpleCompressor.Ratio = value;
				}
			}
		}

		public double Attack
		{
			get
			{
				return simpleCompressor.Attack;
			}
			set
			{
				lock (lockObject)
				{
					simpleCompressor.Attack = value;
				}
			}
		}

		public double Release
		{
			get
			{
				return simpleCompressor.Release;
			}
			set
			{
				lock (lockObject)
				{
					simpleCompressor.Release = value;
				}
			}
		}

		public bool Enabled
		{
			[CompilerGenerated]
			get
			{
				return <Enabled>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<Enabled>k__BackingField = value;
			}
		}

		public override long Length => ((Stream)sourceStream).get_Length();

		public override long Position
		{
			get
			{
				return ((Stream)sourceStream).get_Position();
			}
			set
			{
				lock (lockObject)
				{
					((Stream)sourceStream).set_Position(value);
				}
			}
		}

		public override WaveFormat WaveFormat => sourceStream.WaveFormat;

		public override int BlockAlign => sourceStream.BlockAlign;

		public SimpleCompressorStream(WaveStream sourceStream)
		{
			this.sourceStream = sourceStream;
			channels = sourceStream.WaveFormat.Channels;
			bytesPerSample = sourceStream.WaveFormat.BitsPerSample / 8;
			simpleCompressor = new SimpleCompressor(5.0, 10.0, sourceStream.WaveFormat.SampleRate);
			simpleCompressor.Threshold = 16.0;
			simpleCompressor.Ratio = 6.0;
			simpleCompressor.MakeUpGain = 16.0;
		}

		public override bool HasData(int count)
		{
			return sourceStream.HasData(count);
		}

		private void ReadSamples(byte[] buffer, int start, out double left, out double right)
		{
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			if (bytesPerSample == 4)
			{
				left = BitConverter.ToSingle(buffer, start);
				if (channels > 1)
				{
					right = BitConverter.ToSingle(buffer, start + bytesPerSample);
				}
				else
				{
					right = left;
				}
				return;
			}
			if (bytesPerSample == 2)
			{
				left = (double)BitConverter.ToInt16(buffer, start) / 32768.0;
				if (channels > 1)
				{
					right = (double)BitConverter.ToInt16(buffer, start + bytesPerSample) / 32768.0;
				}
				else
				{
					right = left;
				}
				return;
			}
			throw new InvalidOperationException($"Unsupported bytes per sample: {bytesPerSample}");
		}

		private void WriteSamples(byte[] buffer, int start, double left, double right)
		{
			if (bytesPerSample == 4)
			{
				global::System.Array.Copy((global::System.Array)BitConverter.GetBytes((float)left), 0, (global::System.Array)buffer, start, bytesPerSample);
				if (channels > 1)
				{
					global::System.Array.Copy((global::System.Array)BitConverter.GetBytes((float)right), 0, (global::System.Array)buffer, start + bytesPerSample, bytesPerSample);
				}
			}
			else if (bytesPerSample == 2)
			{
				global::System.Array.Copy((global::System.Array)BitConverter.GetBytes((short)(left * 32768.0)), 0, (global::System.Array)buffer, start, bytesPerSample);
				if (channels > 1)
				{
					global::System.Array.Copy((global::System.Array)BitConverter.GetBytes((short)(right * 32768.0)), 0, (global::System.Array)buffer, start + bytesPerSample, bytesPerSample);
				}
			}
		}

		public override int Read(byte[] array, int offset, int count)
		{
			lock (lockObject)
			{
				if (Enabled)
				{
					if (sourceBuffer == null || sourceBuffer.Length < count)
					{
						sourceBuffer = new byte[count];
					}
					int num = ((Stream)sourceStream).Read(sourceBuffer, 0, count) / (bytesPerSample * channels);
					for (int i = 0; i < num; i++)
					{
						int num2 = i * bytesPerSample * channels;
						ReadSamples(sourceBuffer, num2, out var left, out var right);
						simpleCompressor.Process(ref left, ref right);
						WriteSamples(array, offset + num2, left, right);
					}
					return count;
				}
				return ((Stream)sourceStream).Read(array, offset, count);
			}
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

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NAudio.Wave
{
	[StructLayout(0, Pack = 2)]
	public class AdpcmWaveFormat : WaveFormat
	{
		private short samplesPerBlock;

		private short numCoeff;

		[MarshalAs(30, SizeConst = 14)]
		private short[] coefficients;

		public int SamplesPerBlock => samplesPerBlock;

		public int NumCoefficients => numCoeff;

		public short[] Coefficients => coefficients;

		private AdpcmWaveFormat()
			: this(8000, 1)
		{
		}

		public AdpcmWaveFormat(int sampleRate, int channels)
			: base(sampleRate, 0, channels)
		{
			waveFormatTag = WaveFormatEncoding.Adpcm;
			extraSize = 32;
			switch (base.sampleRate)
			{
			case 8000:
			case 11025:
				blockAlign = 256;
				break;
			case 22050:
				blockAlign = 512;
				break;
			default:
				blockAlign = 1024;
				break;
			}
			bitsPerSample = 4;
			samplesPerBlock = (short)((blockAlign - 7 * channels) * 8 / (bitsPerSample * channels) + 2);
			averageBytesPerSecond = base.SampleRate * blockAlign / samplesPerBlock;
			numCoeff = 7;
			short[] array = new short[14];
			RuntimeHelpers.InitializeArray((global::System.Array)array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
			coefficients = array;
		}

		public override void Serialize(BinaryWriter writer)
		{
			base.Serialize(writer);
			writer.Write(samplesPerBlock);
			writer.Write(numCoeff);
			short[] array = coefficients;
			foreach (short num in array)
			{
				writer.Write(num);
			}
		}

		public override string ToString()
		{
			return string.Format("Microsoft ADPCM {0} Hz {1} channels {2} bits per sample {3} samples per block", new object[4] { base.SampleRate, channels, bitsPerSample, samplesPerBlock });
		}
	}
}

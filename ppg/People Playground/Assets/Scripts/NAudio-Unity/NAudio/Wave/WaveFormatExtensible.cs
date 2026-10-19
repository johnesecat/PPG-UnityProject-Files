using System;
using System.IO;
using System.Runtime.InteropServices;
using NAudio.Dmo;

namespace NAudio.Wave
{
	[StructLayout(0, Pack = 2)]
	public class WaveFormatExtensible : WaveFormat
	{
		private short wValidBitsPerSample;

		private int dwChannelMask;

		private Guid subFormat;

		public Guid SubFormat => subFormat;

		private WaveFormatExtensible()
		{
		}

		public WaveFormatExtensible(int rate, int bits, int channels)
			: base(rate, bits, channels)
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			waveFormatTag = WaveFormatEncoding.Extensible;
			extraSize = 22;
			wValidBitsPerSample = (short)bits;
			for (int i = 0; i < channels; i++)
			{
				dwChannelMask |= 1 << i;
			}
			if (bits == 32)
			{
				subFormat = AudioMediaSubtypes.MEDIASUBTYPE_IEEE_FLOAT;
			}
			else
			{
				subFormat = AudioMediaSubtypes.MEDIASUBTYPE_PCM;
			}
		}

		public WaveFormat ToStandardWaveFormat()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			if (subFormat == AudioMediaSubtypes.MEDIASUBTYPE_IEEE_FLOAT && bitsPerSample == 32)
			{
				return WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
			}
			if (subFormat == AudioMediaSubtypes.MEDIASUBTYPE_PCM)
			{
				return new WaveFormat(sampleRate, bitsPerSample, channels);
			}
			return this;
		}

		public override void Serialize(BinaryWriter writer)
		{
			base.Serialize(writer);
			writer.Write(wValidBitsPerSample);
			writer.Write(dwChannelMask);
			byte[] array = ((Guid)(ref subFormat)).ToByteArray();
			writer.Write(array, 0, array.Length);
		}

		public override string ToString()
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			return string.Format("{0} wBitsPerSample:{1} dwChannelMask:{2} subFormat:{3} extraSize:{4}", new object[5]
			{
				base.ToString(),
				wValidBitsPerSample,
				dwChannelMask,
				subFormat,
				extraSize
			});
		}
	}
}

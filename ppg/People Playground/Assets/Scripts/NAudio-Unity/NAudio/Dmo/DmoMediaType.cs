using System;
using System.Runtime.InteropServices;
using NAudio.Wave;

namespace NAudio.Dmo
{
	public struct DmoMediaType
	{
		private Guid majortype;

		private Guid subtype;

		private bool bFixedSizeSamples;

		private bool bTemporalCompression;

		private int lSampleSize;

		private Guid formattype;

		private global::System.IntPtr pUnk;

		private int cbFormat;

		private global::System.IntPtr pbFormat;

		public Guid MajorType => majortype;

		public string MajorTypeName => MediaTypes.GetMediaTypeName(majortype);

		public Guid SubType => subtype;

		public string SubTypeName
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				if (majortype == MediaTypes.MEDIATYPE_Audio)
				{
					return AudioMediaSubtypes.GetAudioSubtypeName(subtype);
				}
				return ((object)(Guid)(ref subtype)).ToString();
			}
		}

		public bool FixedSizeSamples => bFixedSizeSamples;

		public int SampleSize => lSampleSize;

		public Guid FormatType => formattype;

		public string FormatTypeName
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				if (formattype == DmoMediaTypeGuids.FORMAT_None)
				{
					return "None";
				}
				if (formattype == Guid.Empty)
				{
					return "Null";
				}
				if (formattype == DmoMediaTypeGuids.FORMAT_WaveFormatEx)
				{
					return "WaveFormatEx";
				}
				Guid formatType = FormatType;
				return ((object)(Guid)(ref formatType)).ToString();
			}
		}

		public WaveFormat GetWaveFormat()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			if (formattype == DmoMediaTypeGuids.FORMAT_WaveFormatEx)
			{
				return WaveFormat.MarshalFromPtr(pbFormat);
			}
			throw new InvalidOperationException("Not a WaveFormat type");
		}

		public void SetWaveFormat(WaveFormat waveFormat)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			majortype = MediaTypes.MEDIATYPE_Audio;
			WaveFormatExtensible waveFormatExtensible = waveFormat as WaveFormatExtensible;
			if (waveFormatExtensible != null)
			{
				subtype = waveFormatExtensible.SubFormat;
			}
			else
			{
				switch (waveFormat.Encoding)
				{
				case WaveFormatEncoding.Pcm:
					subtype = AudioMediaSubtypes.MEDIASUBTYPE_PCM;
					break;
				case WaveFormatEncoding.IeeeFloat:
					subtype = AudioMediaSubtypes.MEDIASUBTYPE_IEEE_FLOAT;
					break;
				case WaveFormatEncoding.MpegLayer3:
					subtype = AudioMediaSubtypes.WMMEDIASUBTYPE_MP3;
					break;
				default:
					throw new ArgumentException($"Not a supported encoding {waveFormat.Encoding}");
				}
			}
			bFixedSizeSamples = SubType == AudioMediaSubtypes.MEDIASUBTYPE_PCM || SubType == AudioMediaSubtypes.MEDIASUBTYPE_IEEE_FLOAT;
			formattype = DmoMediaTypeGuids.FORMAT_WaveFormatEx;
			if (cbFormat < Marshal.SizeOf((object)waveFormat))
			{
				throw new InvalidOperationException("Not enough memory assigned for a WaveFormat structure");
			}
			Marshal.StructureToPtr((object)waveFormat, pbFormat, false);
		}
	}
}

using System;
using System.Runtime.InteropServices;
using NAudio.Utils;
using NAudio.Wave;

namespace NAudio.MediaFoundation
{
	public class MediaType
	{
		private readonly IMFMediaType mediaType;

		public int SampleRate
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return GetUInt32(MediaFoundationAttributes.MF_MT_AUDIO_SAMPLES_PER_SECOND);
			}
			set
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				mediaType.SetUINT32(MediaFoundationAttributes.MF_MT_AUDIO_SAMPLES_PER_SECOND, value);
			}
		}

		public int ChannelCount
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return GetUInt32(MediaFoundationAttributes.MF_MT_AUDIO_NUM_CHANNELS);
			}
			set
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				mediaType.SetUINT32(MediaFoundationAttributes.MF_MT_AUDIO_NUM_CHANNELS, value);
			}
		}

		public int BitsPerSample
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return GetUInt32(MediaFoundationAttributes.MF_MT_AUDIO_BITS_PER_SAMPLE);
			}
			set
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				mediaType.SetUINT32(MediaFoundationAttributes.MF_MT_AUDIO_BITS_PER_SAMPLE, value);
			}
		}

		public int AverageBytesPerSecond => GetUInt32(MediaFoundationAttributes.MF_MT_AUDIO_AVG_BYTES_PER_SECOND);

		public Guid SubType
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return GetGuid(MediaFoundationAttributes.MF_MT_SUBTYPE);
			}
			set
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				mediaType.SetGUID(MediaFoundationAttributes.MF_MT_SUBTYPE, value);
			}
		}

		public Guid MajorType
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return GetGuid(MediaFoundationAttributes.MF_MT_MAJOR_TYPE);
			}
			set
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				mediaType.SetGUID(MediaFoundationAttributes.MF_MT_MAJOR_TYPE, value);
			}
		}

		public IMFMediaType MediaFoundationObject => mediaType;

		public MediaType(IMFMediaType mediaType)
		{
			this.mediaType = mediaType;
		}

		public MediaType()
		{
			mediaType = MediaFoundationApi.CreateMediaType();
		}

		public MediaType(WaveFormat waveFormat)
		{
			mediaType = MediaFoundationApi.CreateMediaTypeFromWaveFormat(waveFormat);
		}

		private int GetUInt32(Guid key)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			mediaType.GetUINT32(key, out var punValue);
			return punValue;
		}

		private Guid GetGuid(Guid key)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			mediaType.GetGUID(key, out var pguidValue);
			return pguidValue;
		}

		public int TryGetUInt32(Guid key, int defaultValue = -1)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			int punValue = defaultValue;
			try
			{
				mediaType.GetUINT32(key, out punValue);
				return punValue;
			}
			catch (COMException val)
			{
				COMException exception = val;
				if (exception.GetHResult() != -1072875802)
				{
					if (exception.GetHResult() == -1072875843)
					{
						throw new ArgumentException("Not a UINT32 parameter");
					}
					throw;
				}
				return punValue;
			}
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NAudio.Dmo;
using NAudio.MediaFoundation;

namespace NAudio.Wave
{
	public class MediaFoundationResampler : MediaFoundationTransform
	{
		private int resamplerQuality;

		private static readonly Guid ResamplerClsid = new Guid("f447b69e-1884-4a7e-8055-346f74d6edb3");

		private static readonly Guid IMFTransformIid = new Guid("bf94c121-5b05-4e6f-8000-ba598961414d");

		private IMFActivate activate;

		public int ResamplerQuality
		{
			get
			{
				return resamplerQuality;
			}
			set
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				if (value < 1 || value > 60)
				{
					throw new ArgumentOutOfRangeException("Resampler Quality must be between 1 and 60");
				}
				resamplerQuality = value;
			}
		}

		private static bool IsPcmOrIeeeFloat(WaveFormat waveFormat)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			WaveFormatExtensible waveFormatExtensible = waveFormat as WaveFormatExtensible;
			if (waveFormat.Encoding != WaveFormatEncoding.Pcm && waveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
			{
				if (waveFormatExtensible != null)
				{
					if (!(waveFormatExtensible.SubFormat == AudioSubtypes.MFAudioFormat_PCM))
					{
						return waveFormatExtensible.SubFormat == AudioSubtypes.MFAudioFormat_Float;
					}
					return true;
				}
				return false;
			}
			return true;
		}

		public MediaFoundationResampler(IWaveProvider sourceProvider, WaveFormat outputFormat)
			: base(sourceProvider, outputFormat)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			if (!IsPcmOrIeeeFloat(sourceProvider.WaveFormat))
			{
				throw new ArgumentException("Input must be PCM or IEEE float", "sourceProvider");
			}
			if (!IsPcmOrIeeeFloat(outputFormat))
			{
				throw new ArgumentException("Output must be PCM or IEEE float", "outputFormat");
			}
			MediaFoundationApi.Startup();
			ResamplerQuality = 60;
			object comObject = CreateResamplerComObject();
			FreeComObject(comObject);
		}

		private void FreeComObject(object comObject)
		{
			if (activate != null)
			{
				activate.ShutdownObject();
			}
			Marshal.ReleaseComObject(comObject);
		}

		private object CreateResamplerComObject()
		{
			return new ResamplerMediaComObject();
		}

		private object CreateResamplerComObjectUsingActivator()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			global::System.Collections.Generic.IEnumerator<IMFActivate> enumerator = MediaFoundationApi.EnumerateTransforms(MediaFoundationTransformCategories.AudioEffect).GetEnumerator();
			try
			{
				while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
				{
					IMFActivate current = enumerator.get_Current();
					current.GetGUID(MediaFoundationAttributes.MFT_TRANSFORM_CLSID_Attribute, out var pguidValue);
					if (((Guid)(ref pguidValue)).Equals(ResamplerClsid))
					{
						current.ActivateObject(IMFTransformIid, out var ppv);
						activate = current;
						return ppv;
					}
				}
			}
			finally
			{
				((global::System.IDisposable)enumerator)?.Dispose();
			}
			return null;
		}

		public MediaFoundationResampler(IWaveProvider sourceProvider, int outputSampleRate)
			: this(sourceProvider, CreateOutputFormat(sourceProvider.WaveFormat, outputSampleRate))
		{
		}

		protected override IMFTransform CreateTransform()
		{
			object obj = CreateResamplerComObject();
			IMFTransform obj2 = (IMFTransform)obj;
			IMFMediaType iMFMediaType = MediaFoundationApi.CreateMediaTypeFromWaveFormat(sourceProvider.WaveFormat);
			obj2.SetInputType(0, iMFMediaType, _MFT_SET_TYPE_FLAGS.None);
			Marshal.ReleaseComObject((object)iMFMediaType);
			IMFMediaType iMFMediaType2 = MediaFoundationApi.CreateMediaTypeFromWaveFormat(outputWaveFormat);
			obj2.SetOutputType(0, iMFMediaType2, _MFT_SET_TYPE_FLAGS.None);
			Marshal.ReleaseComObject((object)iMFMediaType2);
			((IWMResamplerProps)obj).SetHalfFilterLength(ResamplerQuality);
			return obj2;
		}

		private static WaveFormat CreateOutputFormat(WaveFormat inputFormat, int outputSampleRate)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			if (inputFormat.Encoding == WaveFormatEncoding.Pcm)
			{
				return new WaveFormat(outputSampleRate, inputFormat.BitsPerSample, inputFormat.Channels);
			}
			if (inputFormat.Encoding == WaveFormatEncoding.IeeeFloat)
			{
				return WaveFormat.CreateIeeeFloatWaveFormat(outputSampleRate, inputFormat.Channels);
			}
			throw new ArgumentException("Can only resample PCM or IEEE float");
		}

		protected override void Dispose(bool disposing)
		{
			if (activate != null)
			{
				activate.ShutdownObject();
				activate = null;
			}
			base.Dispose(disposing);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NAudio.MediaFoundation;
using NAudio.Utils;

namespace NAudio.Wave
{
	public class MediaFoundationEncoder : global::System.IDisposable
	{
		private readonly MediaType outputMediaType;

		private bool disposed;

		public static int[] GetEncodeBitrates(Guid audioSubtype, int sampleRate, int channels)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			<>c__DisplayClass0_0 CS$<>8__locals0 = new <>c__DisplayClass0_0();
			CS$<>8__locals0.sampleRate = sampleRate;
			CS$<>8__locals0.channels = channels;
			return Enumerable.ToArray<int>((global::System.Collections.Generic.IEnumerable<int>)Enumerable.OrderBy<int, int>(Enumerable.Distinct<int>(Enumerable.Select<MediaType, int>(Enumerable.Where<MediaType>((global::System.Collections.Generic.IEnumerable<MediaType>)GetOutputMediaTypes(audioSubtype), (Func<MediaType, bool>)((MediaType mt) => mt.SampleRate == CS$<>8__locals0.sampleRate && mt.ChannelCount == CS$<>8__locals0.channels)), (Func<MediaType, int>)((MediaType mt) => mt.AverageBytesPerSecond * 8))), (Func<int, int>)((int br) => br)));
		}

		public static MediaType[] GetOutputMediaTypes(Guid audioSubtype)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Expected O, but got Unknown
			IMFCollection ppAvailableTypes;
			try
			{
				MediaFoundationInterop.MFTranscodeGetAudioOutputAvailableTypes(audioSubtype, _MFT_ENUM_FLAG.MFT_ENUM_FLAG_ALL, null, out ppAvailableTypes);
			}
			catch (COMException val)
			{
				if (HResult.GetHResult(val) == -1072875819)
				{
					return new MediaType[0];
				}
				throw;
			}
			ppAvailableTypes.GetElementCount(out var pcElements);
			List<MediaType> val2 = new List<MediaType>(pcElements);
			for (int i = 0; i < pcElements; i++)
			{
				ppAvailableTypes.GetElement(i, out var ppUnkElement);
				IMFMediaType mediaType = (IMFMediaType)ppUnkElement;
				val2.Add(new MediaType(mediaType));
			}
			Marshal.ReleaseComObject((object)ppAvailableTypes);
			return val2.ToArray();
		}

		public static void EncodeToWma(IWaveProvider inputProvider, string outputFile, int desiredBitRate = 192000)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			using (MediaFoundationEncoder mediaFoundationEncoder = new MediaFoundationEncoder(SelectMediaType(AudioSubtypes.MFAudioFormat_WMAudioV8, inputProvider.WaveFormat, desiredBitRate) ?? throw new InvalidOperationException("No suitable WMA encoders available")))
			{
				mediaFoundationEncoder.Encode(outputFile, inputProvider);
			}
		}

		public static void EncodeToMp3(IWaveProvider inputProvider, string outputFile, int desiredBitRate = 192000)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			using (MediaFoundationEncoder mediaFoundationEncoder = new MediaFoundationEncoder(SelectMediaType(AudioSubtypes.MFAudioFormat_MP3, inputProvider.WaveFormat, desiredBitRate) ?? throw new InvalidOperationException("No suitable MP3 encoders available")))
			{
				mediaFoundationEncoder.Encode(outputFile, inputProvider);
			}
		}

		public static void EncodeToAac(IWaveProvider inputProvider, string outputFile, int desiredBitRate = 192000)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			using (MediaFoundationEncoder mediaFoundationEncoder = new MediaFoundationEncoder(SelectMediaType(AudioSubtypes.MFAudioFormat_AAC, inputProvider.WaveFormat, desiredBitRate) ?? throw new InvalidOperationException("No suitable AAC encoders available")))
			{
				mediaFoundationEncoder.Encode(outputFile, inputProvider);
			}
		}

		public static MediaType SelectMediaType(Guid audioSubtype, WaveFormat inputFormat, int desiredBitRate)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			<>c__DisplayClass5_0 CS$<>8__locals0 = new <>c__DisplayClass5_0();
			CS$<>8__locals0.inputFormat = inputFormat;
			CS$<>8__locals0.desiredBitRate = desiredBitRate;
			return Enumerable.FirstOrDefault<MediaType>(Enumerable.Select<<>f__AnonymousType0<MediaType, int>, MediaType>((global::System.Collections.Generic.IEnumerable<<>f__AnonymousType0<MediaType, int>>)Enumerable.OrderBy<<>f__AnonymousType0<MediaType, int>, int>(Enumerable.Select<MediaType, <>f__AnonymousType0<MediaType, int>>(Enumerable.Where<MediaType>((global::System.Collections.Generic.IEnumerable<MediaType>)GetOutputMediaTypes(audioSubtype), (Func<MediaType, bool>)((MediaType mt) => mt.SampleRate == CS$<>8__locals0.inputFormat.SampleRate && mt.ChannelCount == CS$<>8__locals0.inputFormat.Channels)), (Func<MediaType, <>f__AnonymousType0<MediaType, int>>)((MediaType mt) => new <>f__AnonymousType0<MediaType, int>(mt, Math.Abs(CS$<>8__locals0.desiredBitRate - mt.AverageBytesPerSecond * 8)))), (Func<<>f__AnonymousType0<MediaType, int>, int>)((<>f__AnonymousType0<MediaType, int> mt) => mt.Delta)), (Func<<>f__AnonymousType0<MediaType, int>, MediaType>)((<>f__AnonymousType0<MediaType, int> mt) => mt.MediaType)));
		}

		public MediaFoundationEncoder(MediaType outputMediaType)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (outputMediaType == null)
			{
				throw new ArgumentNullException("outputMediaType");
			}
			this.outputMediaType = outputMediaType;
		}

		public void Encode(string outputFile, IWaveProvider inputProvider)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			if (inputProvider.WaveFormat.Encoding != WaveFormatEncoding.Pcm && inputProvider.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
			{
				throw new ArgumentException("Encode input format must be PCM or IEEE float");
			}
			MediaType mediaType = new MediaType(inputProvider.WaveFormat);
			IMFSinkWriter iMFSinkWriter = CreateSinkWriter(outputFile);
			try
			{
				iMFSinkWriter.AddStream(outputMediaType.MediaFoundationObject, out var pdwStreamIndex);
				iMFSinkWriter.SetInputMediaType(pdwStreamIndex, mediaType.MediaFoundationObject, null);
				PerformEncode(iMFSinkWriter, pdwStreamIndex, inputProvider);
			}
			finally
			{
				Marshal.ReleaseComObject((object)iMFSinkWriter);
				Marshal.ReleaseComObject((object)mediaType.MediaFoundationObject);
			}
		}

		private static IMFSinkWriter CreateSinkWriter(string outputFile)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Expected O, but got Unknown
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			IMFAttributes iMFAttributes = MediaFoundationApi.CreateAttributes(1);
			iMFAttributes.SetUINT32(MediaFoundationAttributes.MF_READWRITE_ENABLE_HARDWARE_TRANSFORMS, 1);
			try
			{
				MediaFoundationInterop.MFCreateSinkWriterFromURL(outputFile, null, iMFAttributes, out var ppSinkWriter);
				return ppSinkWriter;
			}
			catch (COMException val)
			{
				if (HResult.GetHResult(val) == -1072875819)
				{
					throw new ArgumentException("Was not able to create a sink writer for this file extension");
				}
				throw;
			}
			finally
			{
				Marshal.ReleaseComObject((object)iMFAttributes);
			}
		}

		private void PerformEncode(IMFSinkWriter writer, int streamIndex, IWaveProvider inputProvider)
		{
			byte[] managedBuffer = new byte[inputProvider.WaveFormat.AverageBytesPerSecond * 4];
			writer.BeginWriting();
			long num = 0L;
			long num2 = 0L;
			do
			{
				num2 = ConvertOneBuffer(writer, streamIndex, inputProvider, num, managedBuffer);
				num += num2;
			}
			while (num2 > 0);
			writer.DoFinalize();
		}

		private static long BytesToNsPosition(int bytes, WaveFormat waveFormat)
		{
			return 10000000L * (long)bytes / waveFormat.AverageBytesPerSecond;
		}

		private long ConvertOneBuffer(IMFSinkWriter writer, int streamIndex, IWaveProvider inputProvider, long position, byte[] managedBuffer)
		{
			long num = 0L;
			IMFMediaBuffer iMFMediaBuffer = MediaFoundationApi.CreateMemoryBuffer(managedBuffer.Length);
			iMFMediaBuffer.GetMaxLength(out var pcbMaxLength);
			IMFSample iMFSample = MediaFoundationApi.CreateSample();
			iMFSample.AddBuffer(iMFMediaBuffer);
			iMFMediaBuffer.Lock(out var ppbBuffer, out pcbMaxLength, out var _);
			int num2 = inputProvider.Read(managedBuffer, 0, pcbMaxLength);
			if (num2 > 0)
			{
				num = BytesToNsPosition(num2, inputProvider.WaveFormat);
				Marshal.Copy(managedBuffer, 0, ppbBuffer, num2);
				iMFMediaBuffer.SetCurrentLength(num2);
				iMFMediaBuffer.Unlock();
				iMFSample.SetSampleTime(position);
				iMFSample.SetSampleDuration(num);
				writer.WriteSample(streamIndex, iMFSample);
			}
			else
			{
				iMFMediaBuffer.Unlock();
			}
			Marshal.ReleaseComObject((object)iMFSample);
			Marshal.ReleaseComObject((object)iMFMediaBuffer);
			return num;
		}

		protected void Dispose(bool disposing)
		{
			Marshal.ReleaseComObject((object)outputMediaType.MediaFoundationObject);
		}

		public void Dispose()
		{
			if (!disposed)
			{
				disposed = true;
				Dispose(disposing: true);
			}
			GC.SuppressFinalize((object)this);
		}

		~MediaFoundationEncoder()
		{
			Dispose(disposing: false);
		}
	}
}

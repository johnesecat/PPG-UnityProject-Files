using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using NAudio.Wave;

namespace NAudio.MediaFoundation
{
	public static class MediaFoundationApi
	{
		private static bool initialized;

		public static void Startup()
		{
			if (!initialized)
			{
				int num = 2;
				OperatingSystem oSVersion = Environment.get_OSVersion();
				if (oSVersion.get_Version().get_Major() == 6 && oSVersion.get_Version().get_Minor() == 0)
				{
					num = 1;
				}
				MediaFoundationInterop.MFStartup((num << 16) | 0x70);
				initialized = true;
			}
		}

		public static global::System.Collections.Generic.IEnumerable<IMFActivate> EnumerateTransforms(Guid category)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			MediaFoundationInterop.MFTEnumEx(category, _MFT_ENUM_FLAG.MFT_ENUM_FLAG_ALL, null, null, out var interfacesPointer, out var pcMFTActivate);
			IMFActivate[] array = new IMFActivate[pcMFTActivate];
			for (int i = 0; i < pcMFTActivate; i++)
			{
				global::System.IntPtr intPtr = Marshal.ReadIntPtr(new global::System.IntPtr(interfacesPointer.ToInt64() + i * Marshal.SizeOf((object)(long)interfacesPointer)));
				array[i] = (IMFActivate)Marshal.GetObjectForIUnknown(intPtr);
			}
			IMFActivate[] array2 = array;
			for (int j = 0; j < array2.Length; j++)
			{
				yield return array2[j];
			}
			Marshal.FreeCoTaskMem(interfacesPointer);
		}

		public static void Shutdown()
		{
			if (initialized)
			{
				MediaFoundationInterop.MFShutdown();
				initialized = false;
			}
		}

		public static IMFMediaType CreateMediaType()
		{
			MediaFoundationInterop.MFCreateMediaType(out var ppMFType);
			return ppMFType;
		}

		public static IMFMediaType CreateMediaTypeFromWaveFormat(WaveFormat waveFormat)
		{
			IMFMediaType iMFMediaType = CreateMediaType();
			try
			{
				MediaFoundationInterop.MFInitMediaTypeFromWaveFormatEx(iMFMediaType, waveFormat, Marshal.SizeOf((object)waveFormat));
				return iMFMediaType;
			}
			catch (global::System.Exception)
			{
				Marshal.ReleaseComObject((object)iMFMediaType);
				throw;
			}
		}

		public static IMFMediaBuffer CreateMemoryBuffer(int bufferSize)
		{
			MediaFoundationInterop.MFCreateMemoryBuffer(bufferSize, out var ppBuffer);
			return ppBuffer;
		}

		public static IMFSample CreateSample()
		{
			MediaFoundationInterop.MFCreateSample(out var ppIMFSample);
			return ppIMFSample;
		}

		public static IMFAttributes CreateAttributes(int initialSize)
		{
			MediaFoundationInterop.MFCreateAttributes(out var ppMFAttributes, initialSize);
			return ppMFAttributes;
		}

		public static IMFByteStream CreateByteStream(object stream)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			if (stream is IStream)
			{
				MediaFoundationInterop.MFCreateMFByteStreamOnStream((IStream)((stream is IStream) ? stream : null), out var ppByteStream);
				return ppByteStream;
			}
			throw new ArgumentException("Stream must be IStream in desktop apps");
		}

		public static IMFSourceReader CreateSourceReaderFromByteStream(IMFByteStream byteStream)
		{
			MediaFoundationInterop.MFCreateSourceReaderFromByteStream(byteStream, null, out var ppSourceReader);
			return ppSourceReader;
		}
	}
}

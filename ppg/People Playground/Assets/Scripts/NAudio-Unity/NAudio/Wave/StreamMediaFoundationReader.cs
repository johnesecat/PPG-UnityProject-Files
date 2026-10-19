using System;
using System.IO;
using NAudio.MediaFoundation;

namespace NAudio.Wave
{
	public class StreamMediaFoundationReader : MediaFoundationReader
	{
		private readonly Stream stream;

		public StreamMediaFoundationReader(Stream stream, MediaFoundationReaderSettings settings = null)
		{
			this.stream = stream;
			Init(settings);
		}

		protected override IMFSourceReader CreateReader(MediaFoundationReaderSettings settings)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			IMFSourceReader iMFSourceReader = MediaFoundationApi.CreateSourceReaderFromByteStream(MediaFoundationApi.CreateByteStream(new ComStream(stream)));
			iMFSourceReader.SetStreamSelection(-2, pSelected: false);
			iMFSourceReader.SetStreamSelection(-3, pSelected: true);
			iMFSourceReader.SetCurrentMediaType(-3, global::System.IntPtr.Zero, new MediaType
			{
				MajorType = MediaTypes.MFMediaType_Audio,
				SubType = (settings.RequestFloatOutput ? AudioSubtypes.MFAudioFormat_Float : AudioSubtypes.MFAudioFormat_PCM)
			}.MediaFoundationObject);
			return iMFSourceReader;
		}
	}
}

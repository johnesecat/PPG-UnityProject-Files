using System;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.Dmo
{
	public class WindowsMediaMp3Decoder : global::System.IDisposable
	{
		private MediaObject mediaObject;

		private IPropertyStore propertyStoreInterface;

		private WindowsMediaMp3DecoderComObject mediaComObject;

		public MediaObject MediaObject => mediaObject;

		public WindowsMediaMp3Decoder()
		{
			mediaComObject = new WindowsMediaMp3DecoderComObject();
			mediaObject = new MediaObject((IMediaObject)mediaComObject);
			propertyStoreInterface = (IPropertyStore)mediaComObject;
		}

		public void Dispose()
		{
			if (propertyStoreInterface != null)
			{
				Marshal.ReleaseComObject((object)propertyStoreInterface);
				propertyStoreInterface = null;
			}
			if (mediaObject != null)
			{
				mediaObject.Dispose();
				mediaObject = null;
			}
			if (mediaComObject != null)
			{
				Marshal.ReleaseComObject((object)mediaComObject);
				mediaComObject = null;
			}
		}
	}
}

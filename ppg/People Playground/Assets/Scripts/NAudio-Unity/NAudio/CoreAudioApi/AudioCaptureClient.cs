using System;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi
{
	public class AudioCaptureClient : global::System.IDisposable
	{
		private IAudioCaptureClient audioCaptureClientInterface;

		internal AudioCaptureClient(IAudioCaptureClient audioCaptureClientInterface)
		{
			this.audioCaptureClientInterface = audioCaptureClientInterface;
		}

		public global::System.IntPtr GetBuffer(out int numFramesToRead, out AudioClientBufferFlags bufferFlags, out long devicePosition, out long qpcPosition)
		{
			Marshal.ThrowExceptionForHR(audioCaptureClientInterface.GetBuffer(out var dataBuffer, out numFramesToRead, out bufferFlags, out devicePosition, out qpcPosition));
			return dataBuffer;
		}

		public global::System.IntPtr GetBuffer(out int numFramesToRead, out AudioClientBufferFlags bufferFlags)
		{
			Marshal.ThrowExceptionForHR(audioCaptureClientInterface.GetBuffer(out var dataBuffer, out numFramesToRead, out bufferFlags, out var _, out var _));
			return dataBuffer;
		}

		public int GetNextPacketSize()
		{
			Marshal.ThrowExceptionForHR(audioCaptureClientInterface.GetNextPacketSize(out var numFramesInNextPacket));
			return numFramesInNextPacket;
		}

		public void ReleaseBuffer(int numFramesWritten)
		{
			Marshal.ThrowExceptionForHR(audioCaptureClientInterface.ReleaseBuffer(numFramesWritten));
		}

		public void Dispose()
		{
			if (audioCaptureClientInterface != null)
			{
				Marshal.ReleaseComObject((object)audioCaptureClientInterface);
				audioCaptureClientInterface = null;
				GC.SuppressFinalize((object)this);
			}
		}
	}
}

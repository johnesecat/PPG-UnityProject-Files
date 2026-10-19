using System;
using System.Runtime.InteropServices;

namespace NAudio.CoreAudioApi.Interfaces
{
	[Guid("C8ADBD64-E71E-48a0-A4DE-185C395CD317")]
	[InterfaceType(/*Could not decode attribute arguments.*/)]
	internal interface IAudioCaptureClient
	{
		int GetBuffer(out global::System.IntPtr dataBuffer, out int numFramesToRead, out AudioClientBufferFlags bufferFlags, out long devicePosition, out long qpcPosition);

		int ReleaseBuffer(int numFramesRead);

		int GetNextPacketSize(out int numFramesInNextPacket);
	}
}

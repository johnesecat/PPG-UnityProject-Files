using System;
using System.Runtime.InteropServices;

namespace NAudio.CoreAudioApi.Interfaces
{
	[ComImport]
	[Guid("F294ACFC-3146-4483-A7BF-ADDCA7C260E2")]
	[InterfaceType(/*Could not decode attribute arguments.*/)]
	internal interface IAudioRenderClient
	{
		int GetBuffer(int numFramesRequested, out global::System.IntPtr dataBufferPointer);

		int ReleaseBuffer(int numFramesWritten, AudioClientBufferFlags bufferFlags);
	}
}

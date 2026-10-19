using System;
using System.Runtime.InteropServices;

namespace NAudio.CoreAudioApi.Interfaces
{
	[Guid("BFA971F1-4D5E-40BB-935E-967039BFBEE4")]
	[InterfaceType(/*Could not decode attribute arguments.*/)]
	internal interface IAudioSessionManager
	{
		[PreserveSig]
		int GetAudioSessionControl([Optional][In][MarshalAs(43)] Guid sessionId, [In][MarshalAs(8)] uint streamFlags, [MarshalAs(28)] out IAudioSessionControl sessionControl);

		[PreserveSig]
		int GetSimpleAudioVolume([Optional][In][MarshalAs(43)] Guid sessionId, [In][MarshalAs(8)] uint streamFlags, [MarshalAs(28)] out ISimpleAudioVolume audioVolume);
	}
}

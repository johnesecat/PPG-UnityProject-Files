using System;
using System.Runtime.InteropServices;

namespace NAudio.CoreAudioApi.Interfaces
{
	[Guid("87CE5498-68D6-44E5-9215-6DA47EF883D8")]
	[InterfaceType(/*Could not decode attribute arguments.*/)]
	internal interface ISimpleAudioVolume
	{
		[PreserveSig]
		int SetMasterVolume([In][MarshalAs(11)] float levelNorm, [In][MarshalAs(43)] Guid eventContext);

		[PreserveSig]
		int GetMasterVolume([MarshalAs(11)] out float levelNorm);

		[PreserveSig]
		int SetMute([In][MarshalAs(2)] bool isMuted, [In][MarshalAs(43)] Guid eventContext);

		[PreserveSig]
		int GetMute([MarshalAs(2)] out bool isMuted);
	}
}

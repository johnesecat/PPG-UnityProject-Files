using System;
using System.Runtime.InteropServices;

namespace NAudio.CoreAudioApi.Interfaces
{
	[Guid("77AA99A0-1BD6-484F-8BC7-2C654C9A9B6F")]
	[InterfaceType(/*Could not decode attribute arguments.*/)]
	internal interface IAudioSessionManager2 : IAudioSessionManager
	{
		[PreserveSig]
		new int GetAudioSessionControl([Optional][In][MarshalAs(43)] Guid sessionId, [In][MarshalAs(8)] uint streamFlags, [MarshalAs(28)] out IAudioSessionControl sessionControl);

		[PreserveSig]
		new int GetSimpleAudioVolume([Optional][In][MarshalAs(43)] Guid sessionId, [In][MarshalAs(8)] uint streamFlags, [MarshalAs(28)] out ISimpleAudioVolume audioVolume);

		[PreserveSig]
		int GetSessionEnumerator(out IAudioSessionEnumerator sessionEnum);

		[PreserveSig]
		int RegisterSessionNotification(IAudioSessionNotification sessionNotification);

		[PreserveSig]
		int UnregisterSessionNotification(IAudioSessionNotification sessionNotification);

		[PreserveSig]
		int RegisterDuckNotification(string sessionID, IAudioSessionNotification audioVolumeDuckNotification);

		[PreserveSig]
		int UnregisterDuckNotification(global::System.IntPtr audioVolumeDuckNotification);
	}
}

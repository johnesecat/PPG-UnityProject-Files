using System;
using System.Runtime.InteropServices;

namespace NAudio.CoreAudioApi.Interfaces
{
	[Guid("bfb7ff88-7239-4fc9-8fa2-07c950be9c6d")]
	[InterfaceType(/*Could not decode attribute arguments.*/)]
	public interface IAudioSessionControl2 : IAudioSessionControl
	{
		[PreserveSig]
		new int GetState(out AudioSessionState state);

		[PreserveSig]
		new int GetDisplayName([MarshalAs(21)] out string displayName);

		[PreserveSig]
		new int SetDisplayName([In][MarshalAs(21)] string displayName, [In][MarshalAs(43)] Guid eventContext);

		[PreserveSig]
		new int GetIconPath([MarshalAs(21)] out string iconPath);

		[PreserveSig]
		new int SetIconPath([In][MarshalAs(21)] string iconPath, [In][MarshalAs(43)] Guid eventContext);

		[PreserveSig]
		new int GetGroupingParam(out Guid groupingId);

		[PreserveSig]
		new int SetGroupingParam([In][MarshalAs(43)] Guid groupingId, [In][MarshalAs(43)] Guid eventContext);

		[PreserveSig]
		new int RegisterAudioSessionNotification([In] IAudioSessionEvents client);

		[PreserveSig]
		new int UnregisterAudioSessionNotification([In] IAudioSessionEvents client);

		[PreserveSig]
		int GetSessionIdentifier([MarshalAs(21)] out string retVal);

		[PreserveSig]
		int GetSessionInstanceIdentifier([MarshalAs(21)] out string retVal);

		[PreserveSig]
		int GetProcessId(out uint retVal);

		[PreserveSig]
		int IsSystemSoundsSession();

		[PreserveSig]
		int SetDuckingPreference(bool optOut);
	}
}

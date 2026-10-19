using System;
using System.Runtime.InteropServices;

namespace NAudio.CoreAudioApi.Interfaces
{
	[Guid("F4B1A599-7266-4319-A8CA-E70ACB11E8CD")]
	[InterfaceType(/*Could not decode attribute arguments.*/)]
	public interface IAudioSessionControl
	{
		[PreserveSig]
		int GetState(out AudioSessionState state);

		[PreserveSig]
		int GetDisplayName([MarshalAs(21)] out string displayName);

		[PreserveSig]
		int SetDisplayName([In][MarshalAs(21)] string displayName, [In][MarshalAs(43)] Guid eventContext);

		[PreserveSig]
		int GetIconPath([MarshalAs(21)] out string iconPath);

		[PreserveSig]
		int SetIconPath([In][MarshalAs(21)] string iconPath, [In][MarshalAs(43)] Guid eventContext);

		[PreserveSig]
		int GetGroupingParam(out Guid groupingId);

		[PreserveSig]
		int SetGroupingParam([In][MarshalAs(43)] Guid groupingId, [In][MarshalAs(43)] Guid eventContext);

		[PreserveSig]
		int RegisterAudioSessionNotification([In] IAudioSessionEvents client);

		[PreserveSig]
		int UnregisterAudioSessionNotification([In] IAudioSessionEvents client);
	}
}

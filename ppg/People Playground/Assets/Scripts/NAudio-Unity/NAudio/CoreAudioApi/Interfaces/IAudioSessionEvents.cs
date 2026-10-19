using System;
using System.Runtime.InteropServices;

namespace NAudio.CoreAudioApi.Interfaces
{
	[Guid("24918ACC-64B3-37C1-8CA9-74A66E9957A8")]
	[InterfaceType(/*Could not decode attribute arguments.*/)]
	public interface IAudioSessionEvents
	{
		[PreserveSig]
		int OnDisplayNameChanged([In][MarshalAs(21)] string displayName, [In] ref Guid eventContext);

		[PreserveSig]
		int OnIconPathChanged([In][MarshalAs(21)] string iconPath, [In] ref Guid eventContext);

		[PreserveSig]
		int OnSimpleVolumeChanged([In][MarshalAs(11)] float volume, [In][MarshalAs(2)] bool isMuted, [In] ref Guid eventContext);

		[PreserveSig]
		int OnChannelVolumeChanged([In][MarshalAs(8)] uint channelCount, [In][MarshalAs(31)] global::System.IntPtr newVolumes, [In][MarshalAs(8)] uint channelIndex, [In] ref Guid eventContext);

		[PreserveSig]
		int OnGroupingParamChanged([In] ref Guid groupingId, [In] ref Guid eventContext);

		[PreserveSig]
		int OnStateChanged([In] AudioSessionState state);

		[PreserveSig]
		int OnSessionDisconnected([In] AudioSessionDisconnectReason disconnectReason);
	}
}

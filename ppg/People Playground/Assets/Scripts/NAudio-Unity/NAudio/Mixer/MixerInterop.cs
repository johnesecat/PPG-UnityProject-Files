using System;
using System.Runtime.InteropServices;

namespace NAudio.Mixer
{
	internal class MixerInterop
	{
		[StructLayout(0, CharSet = 4, Pack = 1)]
		public struct MIXERCONTROLDETAILS
		{
			public int cbStruct;

			public int dwControlID;

			public int cChannels;

			public global::System.IntPtr hwndOwner;

			public int cbDetails;

			public global::System.IntPtr paDetails;
		}

		[StructLayout(0, Pack = 1)]
		public struct MIXERCAPS
		{
			public ushort wMid;

			public ushort wPid;

			public uint vDriverVersion;

			[MarshalAs(23, SizeConst = 32)]
			public string szPname;

			public uint fdwSupport;

			public uint cDestinations;
		}

		[StructLayout(0, Pack = 1)]
		public struct MIXERLINECONTROLS
		{
			public int cbStruct;

			public int dwLineID;

			public int dwControlID;

			public int cControls;

			public int cbmxctrl;

			public global::System.IntPtr pamxctrl;
		}

		[Flags]
		public enum MIXERLINE_LINEF
		{
			MIXERLINE_LINEF_ACTIVE = 1,
			MIXERLINE_LINEF_DISCONNECTED = 0x8000,
			MIXERLINE_LINEF_SOURCE = -2147483648
		}

		[StructLayout(0, Pack = 1)]
		public struct MIXERLINE
		{
			public int cbStruct;

			public int dwDestination;

			public int dwSource;

			public int dwLineID;

			public MIXERLINE_LINEF fdwLine;

			public global::System.IntPtr dwUser;

			public MixerLineComponentType dwComponentType;

			public int cChannels;

			public int cConnections;

			public int cControls;

			[MarshalAs(23, SizeConst = 16)]
			public string szShortName;

			[MarshalAs(23, SizeConst = 64)]
			public string szName;

			public uint dwType;

			public uint dwDeviceID;

			public ushort wMid;

			public ushort wPid;

			public uint vDriverVersion;

			[MarshalAs(23, SizeConst = 32)]
			public string szPname;
		}

		[StructLayout(0, Pack = 1)]
		public struct Bounds
		{
			public int minimum;

			public int maximum;

			public int reserved2;

			public int reserved3;

			public int reserved4;

			public int reserved5;
		}

		[StructLayout(0, Pack = 1)]
		public struct Metrics
		{
			public int step;

			public int customData;

			public int reserved2;

			public int reserved3;

			public int reserved4;

			public int reserved5;
		}

		[StructLayout(0, Pack = 1)]
		public struct MIXERCONTROL
		{
			public uint cbStruct;

			public int dwControlID;

			public MixerControlType dwControlType;

			public uint fdwControl;

			public uint cMultipleItems;

			[MarshalAs(23, SizeConst = 16)]
			public string szShortName;

			[MarshalAs(23, SizeConst = 64)]
			public string szName;

			public Bounds Bounds;

			public Metrics Metrics;
		}

		public struct MIXERCONTROLDETAILS_BOOLEAN
		{
			public int fValue;
		}

		public struct MIXERCONTROLDETAILS_SIGNED
		{
			public int lValue;
		}

		[StructLayout(0, Pack = 1)]
		public struct MIXERCONTROLDETAILS_LISTTEXT
		{
			public uint dwParam1;

			public uint dwParam2;

			[MarshalAs(23, SizeConst = 64)]
			public string szName;
		}

		public struct MIXERCONTROLDETAILS_UNSIGNED
		{
			public uint dwValue;
		}

		public const uint MIXERCONTROL_CONTROLF_UNIFORM = 1u;

		public const uint MIXERCONTROL_CONTROLF_MULTIPLE = 2u;

		public const uint MIXERCONTROL_CONTROLF_DISABLED = 2147483648u;

		public const int MAXPNAMELEN = 32;

		public const int MIXER_SHORT_NAME_CHARS = 16;

		public const int MIXER_LONG_NAME_CHARS = 64;

		[DllImport("winmm.dll", CharSet = 2)]
		public static extern int mixerGetNumDevs();

		[DllImport("winmm.dll", CharSet = 2)]
		public static extern MmResult mixerOpen(out global::System.IntPtr hMixer, int uMxId, global::System.IntPtr dwCallback, global::System.IntPtr dwInstance, MixerFlags dwOpenFlags);

		[DllImport("winmm.dll", CharSet = 2)]
		public static extern MmResult mixerClose(global::System.IntPtr hMixer);

		[DllImport("winmm.dll", CharSet = 2)]
		public static extern MmResult mixerGetControlDetails(global::System.IntPtr hMixer, ref MIXERCONTROLDETAILS mixerControlDetails, MixerFlags dwDetailsFlags);

		[DllImport("winmm.dll", CharSet = 2)]
		public static extern MmResult mixerGetDevCaps(global::System.IntPtr nMixerID, ref MIXERCAPS mixerCaps, int mixerCapsSize);

		[DllImport("winmm.dll", CharSet = 2)]
		public static extern MmResult mixerGetID(global::System.IntPtr hMixer, out int mixerID, MixerFlags dwMixerIDFlags);

		[DllImport("winmm.dll", CharSet = 2)]
		public static extern MmResult mixerGetLineControls(global::System.IntPtr hMixer, ref MIXERLINECONTROLS mixerLineControls, MixerFlags dwControlFlags);

		[DllImport("winmm.dll", CharSet = 2)]
		public static extern MmResult mixerGetLineInfo(global::System.IntPtr hMixer, ref MIXERLINE mixerLine, MixerFlags dwInfoFlags);

		[DllImport("winmm.dll", CharSet = 2)]
		public static extern MmResult mixerMessage(global::System.IntPtr hMixer, uint nMessage, global::System.IntPtr dwParam1, global::System.IntPtr dwParam2);

		[DllImport("winmm.dll", CharSet = 2)]
		public static extern MmResult mixerSetControlDetails(global::System.IntPtr hMixer, ref MIXERCONTROLDETAILS mixerControlDetails, MixerFlags dwDetailsFlags);
	}
}

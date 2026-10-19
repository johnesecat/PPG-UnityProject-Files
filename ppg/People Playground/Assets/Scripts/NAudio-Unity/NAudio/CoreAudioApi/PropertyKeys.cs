using System;

namespace NAudio.CoreAudioApi
{
	public static class PropertyKeys
	{
		public static readonly PropertyKey PKEY_DeviceInterface_FriendlyName = new PropertyKey(new Guid(40784238, (short)(-18412), (short)16715, (byte)131, (byte)205, (byte)133, (byte)109, (byte)111, (byte)239, (byte)72, (byte)34), 2);

		public static readonly PropertyKey PKEY_AudioEndpoint_FormFactor = new PropertyKey(new Guid(497408003, (short)(-11118), (short)20189, (byte)140, (byte)35, (byte)224, (byte)192, (byte)255, (byte)238, (byte)127, (byte)14), 0);

		public static readonly PropertyKey PKEY_AudioEndpoint_ControlPanelPageProvider = new PropertyKey(new Guid(497408003, (short)(-11118), (short)20189, (byte)140, (byte)35, (byte)224, (byte)192, (byte)255, (byte)238, (byte)127, (byte)14), 1);

		public static readonly PropertyKey PKEY_AudioEndpoint_Association = new PropertyKey(new Guid(497408003, (short)(-11118), (short)20189, (byte)140, (byte)35, (byte)224, (byte)192, (byte)255, (byte)238, (byte)127, (byte)14), 2);

		public static readonly PropertyKey PKEY_AudioEndpoint_PhysicalSpeakers = new PropertyKey(new Guid(497408003, (short)(-11118), (short)20189, (byte)140, (byte)35, (byte)224, (byte)192, (byte)255, (byte)238, (byte)127, (byte)14), 3);

		public static readonly PropertyKey PKEY_AudioEndpoint_GUID = new PropertyKey(new Guid(497408003, (short)(-11118), (short)20189, (byte)140, (byte)35, (byte)224, (byte)192, (byte)255, (byte)238, (byte)127, (byte)14), 4);

		public static readonly PropertyKey PKEY_AudioEndpoint_Disable_SysFx = new PropertyKey(new Guid(497408003, (short)(-11118), (short)20189, (byte)140, (byte)35, (byte)224, (byte)192, (byte)255, (byte)238, (byte)127, (byte)14), 5);

		public static readonly PropertyKey PKEY_AudioEndpoint_FullRangeSpeakers = new PropertyKey(new Guid(497408003, (short)(-11118), (short)20189, (byte)140, (byte)35, (byte)224, (byte)192, (byte)255, (byte)238, (byte)127, (byte)14), 6);

		public static readonly PropertyKey PKEY_AudioEndpoint_Supports_EventDriven_Mode = new PropertyKey(new Guid(497408003, (short)(-11118), (short)20189, (byte)140, (byte)35, (byte)224, (byte)192, (byte)255, (byte)238, (byte)127, (byte)14), 7);

		public static readonly PropertyKey PKEY_AudioEndpoint_JackSubType = new PropertyKey(new Guid(497408003, (short)(-11118), (short)20189, (byte)140, (byte)35, (byte)224, (byte)192, (byte)255, (byte)238, (byte)127, (byte)14), 8);

		public static readonly PropertyKey PKEY_AudioEngine_DeviceFormat = new PropertyKey(new Guid(-241236403, (short)2092, (short)20007, (byte)188, (byte)115, (byte)104, (byte)130, (byte)161, (byte)187, (byte)142, (byte)76), 0);

		public static readonly PropertyKey PKEY_AudioEngine_OEMFormat = new PropertyKey(new Guid(-460911066, (short)15557, (short)19666, (byte)186, (byte)70, (byte)202, (byte)10, (byte)154, (byte)112, (byte)237, (byte)4), 3);

		public static readonly PropertyKey PKEY_Device_FriendlyName = new PropertyKey(new Guid(-1537465010, (short)(-8420), (short)20221, (byte)128, (byte)32, (byte)103, (byte)209, (byte)70, (byte)168, (byte)80, (byte)224), 14);

		public static readonly PropertyKey PKEY_Device_IconPath = new PropertyKey(new Guid(630898684, (short)20647, (short)18382, (byte)175, (byte)8, (byte)104, (byte)201, (byte)167, (byte)215, (byte)51, (byte)102), 12);

		public static readonly PropertyKey PKEY_Device_DeviceDesc = new PropertyKey(new Guid(-1537465010, (short)(-8420), (short)20221, (byte)128, (byte)32, (byte)103, (byte)209, (byte)70, (byte)168, (byte)80, (byte)224), 2);

		public static readonly PropertyKey PKEY_Device_ControllerDeviceId = new PropertyKey(new Guid(-1275528621, (short)4, (short)17294, (byte)144, (byte)3, (byte)81, (byte)164, (byte)110, (byte)19, (byte)155, (byte)252), 2);

		public static readonly PropertyKey PKEY_Device_InterfaceKey = new PropertyKey(new Guid(590439624, (short)6956, (short)19581, (byte)188, (byte)104, (byte)182, (byte)113, (byte)104, (byte)122, (byte)37, (byte)103), 1);
	}
}

using System.Runtime.InteropServices;

namespace NAudio.CoreAudioApi.Interfaces
{
	[Guid("7991EEC9-7E89-4D85-8390-6C703CEC60C0")]
	[InterfaceType(/*Could not decode attribute arguments.*/)]
	public interface IMMNotificationClient
	{
		void OnDeviceStateChanged([MarshalAs(21)] string deviceId, [MarshalAs(7)] DeviceState newState);

		void OnDeviceAdded([MarshalAs(21)] string pwstrDeviceId);

		void OnDeviceRemoved([MarshalAs(21)] string deviceId);

		void OnDefaultDeviceChanged(DataFlow flow, Role role, [MarshalAs(21)] string defaultDeviceId);

		void OnPropertyValueChanged([MarshalAs(21)] string pwstrDeviceId, PropertyKey key);
	}
}

using System;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi
{
	public class MMDeviceEnumerator : global::System.IDisposable
	{
		private IMMDeviceEnumerator realEnumerator;

		public MMDeviceEnumerator()
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			if (Environment.get_OSVersion().get_Version().get_Major() < 6)
			{
				throw new NotSupportedException("This functionality is only supported on Windows Vista or newer.");
			}
			realEnumerator = new MMDeviceEnumeratorComObject() as IMMDeviceEnumerator;
		}

		public MMDeviceCollection EnumerateAudioEndPoints(DataFlow dataFlow, DeviceState dwStateMask)
		{
			Marshal.ThrowExceptionForHR(realEnumerator.EnumAudioEndpoints(dataFlow, dwStateMask, out var devices));
			return new MMDeviceCollection(devices);
		}

		public MMDevice GetDefaultAudioEndpoint(DataFlow dataFlow, Role role)
		{
			IMMDevice endpoint = null;
			Marshal.ThrowExceptionForHR(realEnumerator.GetDefaultAudioEndpoint(dataFlow, role, out endpoint));
			return new MMDevice(endpoint);
		}

		public bool HasDefaultAudioEndpoint(DataFlow dataFlow, Role role)
		{
			IMMDevice endpoint = null;
			int defaultAudioEndpoint = realEnumerator.GetDefaultAudioEndpoint(dataFlow, role, out endpoint);
			switch (defaultAudioEndpoint)
			{
			case 0:
				Marshal.ReleaseComObject((object)endpoint);
				return true;
			case -2147023728:
				return false;
			default:
				Marshal.ThrowExceptionForHR(defaultAudioEndpoint);
				return false;
			}
		}

		public MMDevice GetDevice(string id)
		{
			IMMDevice deviceName = null;
			Marshal.ThrowExceptionForHR(realEnumerator.GetDevice(id, out deviceName));
			return new MMDevice(deviceName);
		}

		public int RegisterEndpointNotificationCallback([In][MarshalAs(28)] IMMNotificationClient client)
		{
			return realEnumerator.RegisterEndpointNotificationCallback(client);
		}

		public int UnregisterEndpointNotificationCallback([In][MarshalAs(28)] IMMNotificationClient client)
		{
			return realEnumerator.UnregisterEndpointNotificationCallback(client);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize((object)this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && realEnumerator != null)
			{
				Marshal.ReleaseComObject((object)realEnumerator);
				realEnumerator = null;
			}
		}
	}
}

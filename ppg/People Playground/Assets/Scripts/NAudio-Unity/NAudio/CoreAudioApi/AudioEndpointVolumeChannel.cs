using System;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi
{
	public class AudioEndpointVolumeChannel
	{
		private readonly uint channel;

		private readonly IAudioEndpointVolume audioEndpointVolume;

		private Guid notificationGuid = Guid.Empty;

		public Guid NotificationGuid
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return notificationGuid;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				notificationGuid = value;
			}
		}

		public float VolumeLevel
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioEndpointVolume.GetChannelVolumeLevel(channel, out var pfLevelDB));
				return pfLevelDB;
			}
			set
			{
				Marshal.ThrowExceptionForHR(audioEndpointVolume.SetChannelVolumeLevel(channel, value, ref notificationGuid));
			}
		}

		public float VolumeLevelScalar
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioEndpointVolume.GetChannelVolumeLevelScalar(channel, out var pfLevel));
				return pfLevel;
			}
			set
			{
				Marshal.ThrowExceptionForHR(audioEndpointVolume.SetChannelVolumeLevelScalar(channel, value, ref notificationGuid));
			}
		}

		internal AudioEndpointVolumeChannel(IAudioEndpointVolume parent, int channel)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			this.channel = (uint)channel;
			audioEndpointVolume = parent;
		}
	}
}

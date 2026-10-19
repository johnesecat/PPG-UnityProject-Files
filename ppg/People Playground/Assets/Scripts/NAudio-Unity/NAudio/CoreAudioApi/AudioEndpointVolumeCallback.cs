using System;
using NAudio.CoreAudioApi.Interfaces;
using NAudio.Utils;

namespace NAudio.CoreAudioApi
{
	internal class AudioEndpointVolumeCallback : IAudioEndpointVolumeCallback
	{
		private readonly AudioEndpointVolume parent;

		internal AudioEndpointVolumeCallback(AudioEndpointVolume parent)
		{
			this.parent = parent;
		}

		public void OnNotify(global::System.IntPtr notifyData)
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			AudioVolumeNotificationDataStruct audioVolumeNotificationDataStruct = MarshalHelpers.PtrToStructure<AudioVolumeNotificationDataStruct>(notifyData);
			global::System.IntPtr intPtr = MarshalHelpers.OffsetOf<AudioVolumeNotificationDataStruct>("ChannelVolume");
			global::System.IntPtr pointer = (global::System.IntPtr)((long)notifyData + (long)intPtr);
			float[] array = new float[audioVolumeNotificationDataStruct.nChannels];
			for (int i = 0; i < audioVolumeNotificationDataStruct.nChannels; i++)
			{
				array[i] = MarshalHelpers.PtrToStructure<float>(pointer);
			}
			AudioVolumeNotificationData notificationData = new AudioVolumeNotificationData(audioVolumeNotificationDataStruct.guidEventContext, audioVolumeNotificationDataStruct.bMuted, audioVolumeNotificationDataStruct.fMasterVolume, array, audioVolumeNotificationDataStruct.guidEventContext);
			parent.FireNotification(notificationData);
		}
	}
}

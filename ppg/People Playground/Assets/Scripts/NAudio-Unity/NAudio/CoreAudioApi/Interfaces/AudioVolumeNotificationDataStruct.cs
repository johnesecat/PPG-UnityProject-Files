using System;

namespace NAudio.CoreAudioApi.Interfaces
{
	internal struct AudioVolumeNotificationDataStruct
	{
		public Guid guidEventContext;

		public bool bMuted;

		public float fMasterVolume;

		public uint nChannels;

		public float ChannelVolume;

		private void FixCS0649()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			guidEventContext = Guid.Empty;
			bMuted = false;
			fMasterVolume = 0f;
			nChannels = 0u;
			ChannelVolume = 0f;
		}
	}
}

using System;

namespace NAudio.CoreAudioApi
{
	public class AudioVolumeNotificationData
	{
		private readonly Guid eventContext;

		private readonly bool muted;

		private readonly float masterVolume;

		private readonly int channels;

		private readonly float[] channelVolume;

		private readonly Guid guid;

		public Guid EventContext => eventContext;

		public bool Muted => muted;

		public Guid Guid => guid;

		public float MasterVolume => masterVolume;

		public int Channels => channels;

		public float[] ChannelVolume => channelVolume;

		public AudioVolumeNotificationData(Guid eventContext, bool muted, float masterVolume, float[] channelVolume, Guid guid)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			this.eventContext = eventContext;
			this.muted = muted;
			this.masterVolume = masterVolume;
			channels = channelVolume.Length;
			this.channelVolume = channelVolume;
			this.guid = guid;
		}
	}
}

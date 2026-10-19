using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi
{
	public class AudioEndpointVolume : global::System.IDisposable
	{
		private readonly IAudioEndpointVolume audioEndPointVolume;

		private readonly AudioEndpointVolumeChannels channels;

		private readonly AudioEndpointVolumeStepInformation stepInformation;

		private readonly AudioEndpointVolumeVolumeRange volumeRange;

		private readonly EEndpointHardwareSupport hardwareSupport;

		private AudioEndpointVolumeCallback callBack;

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

		public AudioEndpointVolumeVolumeRange VolumeRange => volumeRange;

		public EEndpointHardwareSupport HardwareSupport => hardwareSupport;

		public AudioEndpointVolumeStepInformation StepInformation => stepInformation;

		public AudioEndpointVolumeChannels Channels => channels;

		public float MasterVolumeLevel
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioEndPointVolume.GetMasterVolumeLevel(out var pfLevelDB));
				return pfLevelDB;
			}
			set
			{
				Marshal.ThrowExceptionForHR(audioEndPointVolume.SetMasterVolumeLevel(value, ref notificationGuid));
			}
		}

		public float MasterVolumeLevelScalar
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioEndPointVolume.GetMasterVolumeLevelScalar(out var pfLevel));
				return pfLevel;
			}
			set
			{
				Marshal.ThrowExceptionForHR(audioEndPointVolume.SetMasterVolumeLevelScalar(value, ref notificationGuid));
			}
		}

		public bool Mute
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioEndPointVolume.GetMute(out var pbMute));
				return pbMute;
			}
			set
			{
				Marshal.ThrowExceptionForHR(audioEndPointVolume.SetMute(value, ref notificationGuid));
			}
		}

		public event AudioEndpointVolumeNotificationDelegate OnVolumeNotification
		{
			[CompilerGenerated]
			add
			{
				AudioEndpointVolumeNotificationDelegate audioEndpointVolumeNotificationDelegate = this.OnVolumeNotification;
				AudioEndpointVolumeNotificationDelegate audioEndpointVolumeNotificationDelegate2;
				do
				{
					audioEndpointVolumeNotificationDelegate2 = audioEndpointVolumeNotificationDelegate;
					AudioEndpointVolumeNotificationDelegate audioEndpointVolumeNotificationDelegate3 = (AudioEndpointVolumeNotificationDelegate)global::System.Delegate.Combine((global::System.Delegate)audioEndpointVolumeNotificationDelegate2, (global::System.Delegate)value);
					audioEndpointVolumeNotificationDelegate = Interlocked.CompareExchange<AudioEndpointVolumeNotificationDelegate>(ref this.OnVolumeNotification, audioEndpointVolumeNotificationDelegate3, audioEndpointVolumeNotificationDelegate2);
				}
				while (audioEndpointVolumeNotificationDelegate != audioEndpointVolumeNotificationDelegate2);
			}
			[CompilerGenerated]
			remove
			{
				AudioEndpointVolumeNotificationDelegate audioEndpointVolumeNotificationDelegate = this.OnVolumeNotification;
				AudioEndpointVolumeNotificationDelegate audioEndpointVolumeNotificationDelegate2;
				do
				{
					audioEndpointVolumeNotificationDelegate2 = audioEndpointVolumeNotificationDelegate;
					AudioEndpointVolumeNotificationDelegate audioEndpointVolumeNotificationDelegate3 = (AudioEndpointVolumeNotificationDelegate)global::System.Delegate.Remove((global::System.Delegate)audioEndpointVolumeNotificationDelegate2, (global::System.Delegate)value);
					audioEndpointVolumeNotificationDelegate = Interlocked.CompareExchange<AudioEndpointVolumeNotificationDelegate>(ref this.OnVolumeNotification, audioEndpointVolumeNotificationDelegate3, audioEndpointVolumeNotificationDelegate2);
				}
				while (audioEndpointVolumeNotificationDelegate != audioEndpointVolumeNotificationDelegate2);
			}
		}

		public void VolumeStepUp()
		{
			Marshal.ThrowExceptionForHR(audioEndPointVolume.VolumeStepUp(ref notificationGuid));
		}

		public void VolumeStepDown()
		{
			Marshal.ThrowExceptionForHR(audioEndPointVolume.VolumeStepDown(ref notificationGuid));
		}

		internal AudioEndpointVolume(IAudioEndpointVolume realEndpointVolume)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			audioEndPointVolume = realEndpointVolume;
			channels = new AudioEndpointVolumeChannels(audioEndPointVolume);
			stepInformation = new AudioEndpointVolumeStepInformation(audioEndPointVolume);
			Marshal.ThrowExceptionForHR(audioEndPointVolume.QueryHardwareSupport(out var pdwHardwareSupportMask));
			hardwareSupport = (EEndpointHardwareSupport)pdwHardwareSupportMask;
			volumeRange = new AudioEndpointVolumeVolumeRange(audioEndPointVolume);
			callBack = new AudioEndpointVolumeCallback(this);
			Marshal.ThrowExceptionForHR(audioEndPointVolume.RegisterControlChangeNotify(callBack));
		}

		internal void FireNotification(AudioVolumeNotificationData notificationData)
		{
			this.OnVolumeNotification?.Invoke(notificationData);
		}

		public void Dispose()
		{
			if (callBack != null)
			{
				Marshal.ThrowExceptionForHR(audioEndPointVolume.UnregisterControlChangeNotify(callBack));
				callBack = null;
			}
			Marshal.ReleaseComObject((object)audioEndPointVolume);
			GC.SuppressFinalize((object)this);
		}

		~AudioEndpointVolume()
		{
			Dispose();
		}
	}
}

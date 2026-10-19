using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi
{
	public class AudioSessionControl : global::System.IDisposable
	{
		private readonly IAudioSessionControl audioSessionControlInterface;

		private readonly IAudioSessionControl2 audioSessionControlInterface2;

		private AudioSessionEventsCallback audioSessionEventCallback;

		public AudioMeterInformation AudioMeterInformation
		{
			[CompilerGenerated]
			get
			{
				return <AudioMeterInformation>k__BackingField;
			}
		}

		public SimpleAudioVolume SimpleAudioVolume
		{
			[CompilerGenerated]
			get
			{
				return <SimpleAudioVolume>k__BackingField;
			}
		}

		public AudioSessionState State
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioSessionControlInterface.GetState(out var state));
				return state;
			}
		}

		public string DisplayName
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioSessionControlInterface.GetDisplayName(out var displayName));
				return displayName;
			}
			set
			{
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				if (value != string.Empty)
				{
					Marshal.ThrowExceptionForHR(audioSessionControlInterface.SetDisplayName(value, Guid.Empty));
				}
			}
		}

		public string IconPath
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioSessionControlInterface.GetIconPath(out var iconPath));
				return iconPath;
			}
			set
			{
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				if (value != string.Empty)
				{
					Marshal.ThrowExceptionForHR(audioSessionControlInterface.SetIconPath(value, Guid.Empty));
				}
			}
		}

		public string GetSessionIdentifier
		{
			get
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				if (audioSessionControlInterface2 == null)
				{
					throw new InvalidOperationException("Not supported on this version of Windows");
				}
				Marshal.ThrowExceptionForHR(audioSessionControlInterface2.GetSessionIdentifier(out var retVal));
				return retVal;
			}
		}

		public string GetSessionInstanceIdentifier
		{
			get
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				if (audioSessionControlInterface2 == null)
				{
					throw new InvalidOperationException("Not supported on this version of Windows");
				}
				Marshal.ThrowExceptionForHR(audioSessionControlInterface2.GetSessionInstanceIdentifier(out var retVal));
				return retVal;
			}
		}

		public uint GetProcessID
		{
			get
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				if (audioSessionControlInterface2 == null)
				{
					throw new InvalidOperationException("Not supported on this version of Windows");
				}
				Marshal.ThrowExceptionForHR(audioSessionControlInterface2.GetProcessId(out var retVal));
				return retVal;
			}
		}

		public bool IsSystemSoundsSession
		{
			get
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				if (audioSessionControlInterface2 == null)
				{
					throw new InvalidOperationException("Not supported on this version of Windows");
				}
				return audioSessionControlInterface2.IsSystemSoundsSession() == 0;
			}
		}

		public AudioSessionControl(IAudioSessionControl audioSessionControl)
		{
			audioSessionControlInterface = audioSessionControl;
			audioSessionControlInterface2 = audioSessionControl as IAudioSessionControl2;
			IAudioMeterInformation audioMeterInformation = audioSessionControlInterface as IAudioMeterInformation;
			ISimpleAudioVolume simpleAudioVolume = audioSessionControlInterface as ISimpleAudioVolume;
			if (audioMeterInformation != null)
			{
				<AudioMeterInformation>k__BackingField = new AudioMeterInformation(audioMeterInformation);
			}
			if (simpleAudioVolume != null)
			{
				<SimpleAudioVolume>k__BackingField = new SimpleAudioVolume(simpleAudioVolume);
			}
		}

		public void Dispose()
		{
			if (audioSessionEventCallback != null)
			{
				Marshal.ThrowExceptionForHR(audioSessionControlInterface.UnregisterAudioSessionNotification(audioSessionEventCallback));
				audioSessionEventCallback = null;
			}
			GC.SuppressFinalize((object)this);
		}

		~AudioSessionControl()
		{
			Dispose();
		}

		public Guid GetGroupingParam()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			Marshal.ThrowExceptionForHR(audioSessionControlInterface.GetGroupingParam(out var groupingId));
			return groupingId;
		}

		public void SetGroupingParam(Guid groupingId, Guid context)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			Marshal.ThrowExceptionForHR(audioSessionControlInterface.SetGroupingParam(groupingId, context));
		}

		public void RegisterEventClient(IAudioSessionEventsHandler eventClient)
		{
			audioSessionEventCallback = new AudioSessionEventsCallback(eventClient);
			Marshal.ThrowExceptionForHR(audioSessionControlInterface.RegisterAudioSessionNotification(audioSessionEventCallback));
		}

		public void UnRegisterEventClient(IAudioSessionEventsHandler eventClient)
		{
			if (audioSessionEventCallback != null)
			{
				Marshal.ThrowExceptionForHR(audioSessionControlInterface.UnregisterAudioSessionNotification(audioSessionEventCallback));
				audioSessionEventCallback = null;
			}
		}
	}
}

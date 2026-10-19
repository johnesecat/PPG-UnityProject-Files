using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi
{
	public class AudioSessionManager
	{
		public delegate void SessionCreatedDelegate(object sender, IAudioSessionControl newSession);

		private readonly IAudioSessionManager audioSessionInterface;

		private readonly IAudioSessionManager2 audioSessionInterface2;

		private AudioSessionNotification audioSessionNotification;

		private SessionCollection sessions;

		private SimpleAudioVolume simpleAudioVolume;

		private AudioSessionControl audioSessionControl;

		public SimpleAudioVolume SimpleAudioVolume
		{
			get
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				if (simpleAudioVolume == null)
				{
					audioSessionInterface.GetSimpleAudioVolume(Guid.Empty, 0u, out var audioVolume);
					simpleAudioVolume = new SimpleAudioVolume(audioVolume);
				}
				return simpleAudioVolume;
			}
		}

		public AudioSessionControl AudioSessionControl
		{
			get
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				if (audioSessionControl == null)
				{
					audioSessionInterface.GetAudioSessionControl(Guid.Empty, 0u, out var sessionControl);
					audioSessionControl = new AudioSessionControl(sessionControl);
				}
				return audioSessionControl;
			}
		}

		public SessionCollection Sessions => sessions;

		public event SessionCreatedDelegate OnSessionCreated
		{
			[CompilerGenerated]
			add
			{
				SessionCreatedDelegate sessionCreatedDelegate = this.OnSessionCreated;
				SessionCreatedDelegate sessionCreatedDelegate2;
				do
				{
					sessionCreatedDelegate2 = sessionCreatedDelegate;
					SessionCreatedDelegate sessionCreatedDelegate3 = (SessionCreatedDelegate)global::System.Delegate.Combine((global::System.Delegate)sessionCreatedDelegate2, (global::System.Delegate)value);
					sessionCreatedDelegate = Interlocked.CompareExchange<SessionCreatedDelegate>(ref this.OnSessionCreated, sessionCreatedDelegate3, sessionCreatedDelegate2);
				}
				while (sessionCreatedDelegate != sessionCreatedDelegate2);
			}
			[CompilerGenerated]
			remove
			{
				SessionCreatedDelegate sessionCreatedDelegate = this.OnSessionCreated;
				SessionCreatedDelegate sessionCreatedDelegate2;
				do
				{
					sessionCreatedDelegate2 = sessionCreatedDelegate;
					SessionCreatedDelegate sessionCreatedDelegate3 = (SessionCreatedDelegate)global::System.Delegate.Remove((global::System.Delegate)sessionCreatedDelegate2, (global::System.Delegate)value);
					sessionCreatedDelegate = Interlocked.CompareExchange<SessionCreatedDelegate>(ref this.OnSessionCreated, sessionCreatedDelegate3, sessionCreatedDelegate2);
				}
				while (sessionCreatedDelegate != sessionCreatedDelegate2);
			}
		}

		internal AudioSessionManager(IAudioSessionManager audioSessionManager)
		{
			audioSessionInterface = audioSessionManager;
			audioSessionInterface2 = audioSessionManager as IAudioSessionManager2;
			RefreshSessions();
		}

		internal void FireSessionCreated(IAudioSessionControl newSession)
		{
			this.OnSessionCreated?.Invoke(this, newSession);
		}

		public void RefreshSessions()
		{
			UnregisterNotifications();
			if (audioSessionInterface2 != null)
			{
				Marshal.ThrowExceptionForHR(audioSessionInterface2.GetSessionEnumerator(out var sessionEnum));
				sessions = new SessionCollection(sessionEnum);
				audioSessionNotification = new AudioSessionNotification(this);
				Marshal.ThrowExceptionForHR(audioSessionInterface2.RegisterSessionNotification(audioSessionNotification));
			}
		}

		public void Dispose()
		{
			UnregisterNotifications();
			GC.SuppressFinalize((object)this);
		}

		private void UnregisterNotifications()
		{
			sessions = null;
			if (audioSessionNotification != null && audioSessionInterface2 != null)
			{
				Marshal.ThrowExceptionForHR(audioSessionInterface2.UnregisterSessionNotification(audioSessionNotification));
				audioSessionNotification = null;
			}
		}

		~AudioSessionManager()
		{
			Dispose();
		}
	}
}

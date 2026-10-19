using System;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi
{
	public class SimpleAudioVolume : global::System.IDisposable
	{
		private readonly ISimpleAudioVolume simpleAudioVolume;

		public float Volume
		{
			get
			{
				Marshal.ThrowExceptionForHR(simpleAudioVolume.GetMasterVolume(out var levelNorm));
				return levelNorm;
			}
			set
			{
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				if ((double)value >= 0.0 && (double)value <= 1.0)
				{
					Marshal.ThrowExceptionForHR(simpleAudioVolume.SetMasterVolume(value, Guid.Empty));
				}
			}
		}

		public bool Mute
		{
			get
			{
				Marshal.ThrowExceptionForHR(simpleAudioVolume.GetMute(out var isMuted));
				return isMuted;
			}
			set
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				Marshal.ThrowExceptionForHR(simpleAudioVolume.SetMute(value, Guid.Empty));
			}
		}

		internal SimpleAudioVolume(ISimpleAudioVolume realSimpleVolume)
		{
			simpleAudioVolume = realSimpleVolume;
		}

		public void Dispose()
		{
			GC.SuppressFinalize((object)this);
		}

		~SimpleAudioVolume()
		{
			Dispose();
		}
	}
}

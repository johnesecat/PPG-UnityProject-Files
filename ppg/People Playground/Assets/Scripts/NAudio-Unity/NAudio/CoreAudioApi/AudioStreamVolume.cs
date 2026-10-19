using System;
using System.Globalization;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi
{
	public class AudioStreamVolume : global::System.IDisposable
	{
		private IAudioStreamVolume audioStreamVolumeInterface;

		public int ChannelCount
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioStreamVolumeInterface.GetChannelCount(out var dwCount));
				return (int)dwCount;
			}
		}

		internal AudioStreamVolume(IAudioStreamVolume audioStreamVolumeInterface)
		{
			this.audioStreamVolumeInterface = audioStreamVolumeInterface;
		}

		private void CheckChannelIndex(int channelIndex, string parameter)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			int channelCount = ChannelCount;
			if (channelIndex >= channelCount)
			{
				throw new ArgumentOutOfRangeException(parameter, "You must supply a valid channel index < current count of channels: " + channelCount);
			}
		}

		public float[] GetAllVolumes()
		{
			Marshal.ThrowExceptionForHR(audioStreamVolumeInterface.GetChannelCount(out var dwCount));
			float[] array = new float[dwCount];
			Marshal.ThrowExceptionForHR(audioStreamVolumeInterface.GetAllVolumes(dwCount, array));
			return array;
		}

		public float GetChannelVolume(int channelIndex)
		{
			CheckChannelIndex(channelIndex, "channelIndex");
			Marshal.ThrowExceptionForHR(audioStreamVolumeInterface.GetChannelVolume((uint)channelIndex, out var fLevel));
			return fLevel;
		}

		public void SetAllVolumes(float[] levels)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			int channelCount = ChannelCount;
			if (levels == null)
			{
				throw new ArgumentNullException("levels");
			}
			if (levels.Length != channelCount)
			{
				throw new ArgumentOutOfRangeException("levels", string.Format((IFormatProvider)(object)CultureInfo.get_InvariantCulture(), "SetAllVolumes MUST be supplied with a volume level for ALL channels. The AudioStream has {0} channels and you supplied {1} channels.", new object[2] { channelCount, levels.Length }));
			}
			for (int i = 0; i < levels.Length; i++)
			{
				float num = levels[i];
				if (num < 0f)
				{
					throw new ArgumentOutOfRangeException("levels", "All volumes must be between 0.0 and 1.0. Invalid volume at index: " + i);
				}
				if (num > 1f)
				{
					throw new ArgumentOutOfRangeException("levels", "All volumes must be between 0.0 and 1.0. Invalid volume at index: " + i);
				}
			}
			Marshal.ThrowExceptionForHR(audioStreamVolumeInterface.SetAllVoumes((uint)channelCount, levels));
		}

		public void SetChannelVolume(int index, float level)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			CheckChannelIndex(index, "index");
			if (level < 0f)
			{
				throw new ArgumentOutOfRangeException("level", "Volume must be between 0.0 and 1.0");
			}
			if (level > 1f)
			{
				throw new ArgumentOutOfRangeException("level", "Volume must be between 0.0 and 1.0");
			}
			Marshal.ThrowExceptionForHR(audioStreamVolumeInterface.SetChannelVolume((uint)index, level));
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize((object)this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && audioStreamVolumeInterface != null)
			{
				Marshal.ReleaseComObject((object)audioStreamVolumeInterface);
				audioStreamVolumeInterface = null;
			}
		}
	}
}

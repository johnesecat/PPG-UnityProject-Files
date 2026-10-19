using System;
using System.Reflection;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi
{
	[DefaultMember("Item")]
	public class AudioMeterInformationChannels
	{
		private readonly IAudioMeterInformation audioMeterInformation;

		public int Count
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioMeterInformation.GetMeteringChannelCount(out var pnChannelCount));
				return pnChannelCount;
			}
		}

		public float this[int index]
		{
			get
			{
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				int count = Count;
				if (index >= count)
				{
					throw new ArgumentOutOfRangeException("index", $"Peak index cannot be greater than number of channels ({count})");
				}
				float[] array = new float[Count];
				GCHandle val = GCHandle.Alloc((object)array, (GCHandleType)3);
				Marshal.ThrowExceptionForHR(audioMeterInformation.GetChannelsPeakValues(array.Length, ((GCHandle)(ref val)).AddrOfPinnedObject()));
				((GCHandle)(ref val)).Free();
				return array[index];
			}
		}

		internal AudioMeterInformationChannels(IAudioMeterInformation parent)
		{
			audioMeterInformation = parent;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NAudio.Mixer
{
	public class Mixer
	{
		private MixerInterop.MIXERCAPS caps;

		private global::System.IntPtr mixerHandle;

		private MixerFlags mixerHandleType;

		public static int NumberOfDevices => MixerInterop.mixerGetNumDevs();

		public int DestinationCount => (int)caps.cDestinations;

		public string Name => caps.szPname;

		public Manufacturers Manufacturer => (Manufacturers)caps.wMid;

		public int ProductID => caps.wPid;

		public global::System.Collections.Generic.IEnumerable<MixerLine> Destinations
		{
			get
			{
				for (int destination = 0; destination < DestinationCount; destination++)
				{
					yield return GetDestination(destination);
				}
			}
		}

		public static global::System.Collections.Generic.IEnumerable<Mixer> Mixers
		{
			get
			{
				for (int device = 0; device < NumberOfDevices; device++)
				{
					yield return new Mixer(device);
				}
			}
		}

		public Mixer(int mixerIndex)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			if (mixerIndex < 0 || mixerIndex >= NumberOfDevices)
			{
				throw new ArgumentOutOfRangeException("mixerID");
			}
			caps = default(MixerInterop.MIXERCAPS);
			MmException.Try(MixerInterop.mixerGetDevCaps((global::System.IntPtr)mixerIndex, ref caps, Marshal.SizeOf((object)caps)), "mixerGetDevCaps");
			mixerHandle = (global::System.IntPtr)mixerIndex;
			mixerHandleType = MixerFlags.Mixer;
		}

		public MixerLine GetDestination(int destinationIndex)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			if (destinationIndex < 0 || destinationIndex >= DestinationCount)
			{
				throw new ArgumentOutOfRangeException("destinationIndex");
			}
			return new MixerLine(mixerHandle, destinationIndex, mixerHandleType);
		}
	}
}

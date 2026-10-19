using System;
using System.Runtime.InteropServices;

namespace NAudio.Mixer
{
	public class UnsignedMixerControl : MixerControl
	{
		private MixerInterop.MIXERCONTROLDETAILS_UNSIGNED[] unsignedDetails;

		public uint Value
		{
			get
			{
				GetControlDetails();
				return unsignedDetails[0].dwValue;
			}
			set
			{
				int num = Marshal.SizeOf((object)unsignedDetails[0]);
				mixerControlDetails.paDetails = Marshal.AllocHGlobal(num * nChannels);
				for (int i = 0; i < nChannels; i++)
				{
					unsignedDetails[i].dwValue = value;
					long num2 = mixerControlDetails.paDetails.ToInt64() + num * i;
					Marshal.StructureToPtr((object)unsignedDetails[i], (global::System.IntPtr)num2, false);
				}
				MmException.Try(MixerInterop.mixerSetControlDetails(mixerHandle, ref mixerControlDetails, MixerFlags.Mixer | mixerHandleType), "mixerSetControlDetails");
				Marshal.FreeHGlobal(mixerControlDetails.paDetails);
			}
		}

		public uint MinValue => (uint)mixerControl.Bounds.minimum;

		public uint MaxValue => (uint)mixerControl.Bounds.maximum;

		public double Percent
		{
			get
			{
				return 100.0 * (double)(Value - MinValue) / (double)(MaxValue - MinValue);
			}
			set
			{
				Value = (uint)((double)MinValue + value / 100.0 * (double)(MaxValue - MinValue));
			}
		}

		internal UnsignedMixerControl(MixerInterop.MIXERCONTROL mixerControl, global::System.IntPtr mixerHandle, MixerFlags mixerHandleType, int nChannels)
		{
			base.mixerControl = mixerControl;
			base.mixerHandle = mixerHandle;
			base.mixerHandleType = mixerHandleType;
			base.nChannels = nChannels;
			mixerControlDetails = default(MixerInterop.MIXERCONTROLDETAILS);
			GetControlDetails();
		}

		protected override void GetDetails(global::System.IntPtr pDetails)
		{
			unsignedDetails = new MixerInterop.MIXERCONTROLDETAILS_UNSIGNED[nChannels];
			for (int i = 0; i < nChannels; i++)
			{
				unsignedDetails[i] = (MixerInterop.MIXERCONTROLDETAILS_UNSIGNED)Marshal.PtrToStructure(mixerControlDetails.paDetails, typeof(MixerInterop.MIXERCONTROLDETAILS_UNSIGNED));
			}
		}

		public override string ToString()
		{
			return $"{base.ToString()} {Percent}%";
		}
	}
}

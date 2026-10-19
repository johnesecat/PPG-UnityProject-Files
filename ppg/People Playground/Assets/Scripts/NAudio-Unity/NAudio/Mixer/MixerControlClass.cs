using System;

namespace NAudio.Mixer
{
	[Flags]
	internal enum MixerControlClass
	{
		Custom = 0,
		Meter = 0x10000000,
		Switch = 0x20000000,
		Number = 805306368,
		Slider = 0x40000000,
		Fader = 1342177280,
		Time = 1610612736,
		List = 1879048192,
		Mask = 1879048192
	}
}

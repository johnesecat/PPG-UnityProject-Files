using System;

namespace NAudio.Mixer
{
	[Flags]
	internal enum MixerControlUnits
	{
		Custom = 0,
		Boolean = 0x10000,
		Signed = 0x20000,
		Unsigned = 196608,
		Decibels = 0x40000,
		Percent = 327680,
		Mask = 16711680
	}
}

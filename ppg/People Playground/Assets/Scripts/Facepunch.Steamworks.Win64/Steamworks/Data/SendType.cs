using System;

namespace Steamworks.Data
{
	[Flags]
	public enum SendType
	{
		Unreliable = 0x0,
		NoNagle = 0x1,
		NoDelay = 0x4,
		Reliable = 0x8
	}
}

using System;

namespace MonoMod.Utils
{
	[Flags]
	internal enum Platform
	{
		OS = 0x1,
		Bits64 = 0x2,
		NT = 0x4,
		Unix = 0x8,
		ARM = 0x10000,
		Wine = 0x20000,
		Unknown = 0x11,
		Windows = 0x25,
		MacOS = 0x49,
		Linux = 0x89,
		Android = 0x189,
		iOS = 0x249
	}
}

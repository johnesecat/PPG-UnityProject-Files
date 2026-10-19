using System;

namespace Activations
{
	[Flags]
	public enum Channel : byte
	{
		None = 0x0,
		Green = 0x1,
		Red = 0x2,
		Blue = 0x4,
		All = byte.MaxValue
	}
}

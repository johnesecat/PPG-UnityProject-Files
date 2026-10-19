using System;

namespace Ceras
{
	[Flags]
	public enum VersionToleranceMode
	{
		Disabled = 0x0,
		Standard = 0x1,
		Extended = 0x2
	}
}

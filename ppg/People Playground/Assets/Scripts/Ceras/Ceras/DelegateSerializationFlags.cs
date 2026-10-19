using System;

namespace Ceras
{
	[Flags]
	public enum DelegateSerializationFlags
	{
		Off = 0x0,
		AllowStatic = 0x1,
		AllowInstance = 0x2
	}
}

using System;

namespace Ceras
{
	[Flags]
	public enum TargetMember
	{
		None = 0x0,
		PublicFields = 0x1,
		PrivateFields = 0x2,
		PublicProperties = 0x4,
		PrivateProperties = 0x8,
		AllPublic = 0x5,
		AllPrivate = 0xA,
		AllFields = 0x3,
		AllProperties = 0xC,
		All = 0xF
	}
}

using System;

namespace Novell.Directory.Ldap.Utilclass
{
	[Serializable]
	public enum TokenTypes
	{
		EOL = 10,
		EOF = -1,
		NUMBER = -2,
		WORD = -3,
		REAL = -4,
		STRING = -5
	}
}

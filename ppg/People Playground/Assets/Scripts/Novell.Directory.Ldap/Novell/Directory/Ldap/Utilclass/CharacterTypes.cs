using System;

namespace Novell.Directory.Ldap.Utilclass
{
	[Serializable]
	[CLSCompliant(false)]
	public enum CharacterTypes : sbyte
	{
		WHITESPACE = 1,
		NUMERIC = 2,
		ALPHABETIC = 4,
		STRINGQUOTE = 8,
		COMMENTCHAR = 0x10
	}
}

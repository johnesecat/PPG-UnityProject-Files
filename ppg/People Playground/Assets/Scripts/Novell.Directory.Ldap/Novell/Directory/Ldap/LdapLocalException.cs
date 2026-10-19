using System;

namespace Novell.Directory.Ldap
{
	public class LdapLocalException : LdapException
	{
		public LdapLocalException()
		{
		}

		public LdapLocalException(string messageOrKey, int resultCode)
			: base(messageOrKey, resultCode, null)
		{
		}

		public LdapLocalException(string messageOrKey, object[] arguments, int resultCode)
			: base(messageOrKey, arguments, resultCode, null)
		{
		}

		public LdapLocalException(string messageOrKey, int resultCode, Exception rootException)
			: base(messageOrKey, resultCode, null, rootException)
		{
		}

		public LdapLocalException(string messageOrKey, object[] arguments, int resultCode, Exception rootException)
			: base(messageOrKey, arguments, resultCode, null, rootException)
		{
		}

		public override string ToString()
		{
			return getExceptionString("LdapLocalException");
		}
	}
}

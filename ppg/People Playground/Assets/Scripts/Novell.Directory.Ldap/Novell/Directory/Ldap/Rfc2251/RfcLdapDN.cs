using System;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcLdapDN : RfcLdapString
	{
		public RfcLdapDN(string s)
			: base(s)
		{
		}

		[CLSCompliant(false)]
		public RfcLdapDN(sbyte[] s)
			: base(s)
		{
		}
	}
}

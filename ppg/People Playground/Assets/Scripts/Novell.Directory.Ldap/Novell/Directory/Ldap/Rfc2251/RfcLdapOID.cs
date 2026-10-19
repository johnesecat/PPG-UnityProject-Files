using System;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcLdapOID : Asn1OctetString
	{
		public RfcLdapOID(string s)
			: base(s)
		{
		}

		[CLSCompliant(false)]
		public RfcLdapOID(sbyte[] s)
			: base(s)
		{
		}
	}
}

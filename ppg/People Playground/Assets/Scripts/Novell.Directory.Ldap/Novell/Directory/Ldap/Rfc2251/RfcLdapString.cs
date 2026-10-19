using System;
using System.IO;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcLdapString : Asn1OctetString
	{
		public RfcLdapString(string s)
			: base(s)
		{
		}

		[CLSCompliant(false)]
		public RfcLdapString(sbyte[] ba)
			: base(ba)
		{
		}

		[CLSCompliant(false)]
		public RfcLdapString(Asn1Decoder dec, Stream in_Renamed, int len)
			: base(dec, in_Renamed, len)
		{
		}
	}
}

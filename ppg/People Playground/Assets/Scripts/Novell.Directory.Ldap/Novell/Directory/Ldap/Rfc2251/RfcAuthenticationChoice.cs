using System;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcAuthenticationChoice : Asn1Choice
	{
		public RfcAuthenticationChoice(Asn1Tagged choice)
			: base(choice)
		{
		}

		[CLSCompliant(false)]
		public RfcAuthenticationChoice(string mechanism, sbyte[] credentials)
			: base(new Asn1Tagged(new Asn1Identifier(2, constructed: true, 3), new RfcSaslCredentials(new RfcLdapString(mechanism), (credentials != null) ? new Asn1OctetString(credentials) : null), explicit_Renamed: false))
		{
		}
	}
}

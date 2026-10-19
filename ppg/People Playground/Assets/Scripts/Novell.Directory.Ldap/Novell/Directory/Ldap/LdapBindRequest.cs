using System;
using Novell.Directory.Ldap.Asn1;
using Novell.Directory.Ldap.Rfc2251;

namespace Novell.Directory.Ldap
{
	public class LdapBindRequest : LdapMessage
	{
		public virtual string AuthenticationDN => Asn1Object.RequestDN;

		[CLSCompliant(false)]
		public LdapBindRequest(int version, string dn, sbyte[] passwd, LdapControl[] cont)
			: base(0, new RfcBindRequest(new Asn1Integer(version), new RfcLdapDN(dn), new RfcAuthenticationChoice(new Asn1Tagged(new Asn1Identifier(2, constructed: false, 0), new Asn1OctetString(passwd), explicit_Renamed: false))), cont)
		{
		}

		[CLSCompliant(false)]
		public LdapBindRequest(int version, string dn, string mechanism, sbyte[] credentials, LdapControl[] cont)
			: base(0, new RfcBindRequest(version, dn, mechanism, credentials), cont)
		{
		}

		public override string ToString()
		{
			return Asn1Object.ToString();
		}
	}
}

using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcSaslCredentials : Asn1Sequence
	{
		public RfcSaslCredentials(RfcLdapString mechanism)
			: this(mechanism, null)
		{
		}

		public RfcSaslCredentials(RfcLdapString mechanism, Asn1OctetString credentials)
			: base(2)
		{
			add(mechanism);
			if (credentials != null)
			{
				add(credentials);
			}
		}
	}
}

using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public interface RfcResponse
	{
		Asn1Enumerated getResultCode();

		RfcLdapDN getMatchedDN();

		RfcLdapString getErrorMessage();

		RfcReferral getReferral();
	}
}

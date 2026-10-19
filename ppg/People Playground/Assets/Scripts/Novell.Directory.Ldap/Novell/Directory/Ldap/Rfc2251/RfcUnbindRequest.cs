using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcUnbindRequest : Asn1Null, RfcRequest
	{
		public override Asn1Identifier getIdentifier()
		{
			return new Asn1Identifier(1, constructed: false, 2);
		}

		public RfcRequest dupRequest(string base_Renamed, string filter, bool request)
		{
			throw new LdapException("NO_DUP_REQUEST", new object[1] { "unbind" }, 92, null);
		}

		public string getRequestDN()
		{
			return null;
		}
	}
}

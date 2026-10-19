using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	internal class RfcAbandonRequest : RfcMessageID, RfcRequest
	{
		public RfcAbandonRequest(int msgId)
			: base(msgId)
		{
		}

		public override Asn1Identifier getIdentifier()
		{
			return new Asn1Identifier(1, constructed: false, 16);
		}

		public RfcRequest dupRequest(string base_Renamed, string filter, bool reference)
		{
			throw new LdapException("NO_DUP_REQUEST", new object[1] { "Abandon" }, 92, null);
		}

		public string getRequestDN()
		{
			return null;
		}
	}
}

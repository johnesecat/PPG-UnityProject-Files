using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcExtendedRequest : Asn1Sequence, RfcRequest
	{
		public const int REQUEST_NAME = 0;

		public const int REQUEST_VALUE = 1;

		public RfcExtendedRequest(RfcLdapOID requestName)
			: this(requestName, null)
		{
		}

		public RfcExtendedRequest(RfcLdapOID requestName, Asn1OctetString requestValue)
			: base(2)
		{
			add(new Asn1Tagged(new Asn1Identifier(2, constructed: false, 0), requestName, explicit_Renamed: false));
			if (requestValue != null)
			{
				add(new Asn1Tagged(new Asn1Identifier(2, constructed: false, 1), requestValue, explicit_Renamed: false));
			}
		}

		public RfcExtendedRequest(Asn1Object[] origRequest)
			: base(origRequest, origRequest.Length)
		{
		}

		public override Asn1Identifier getIdentifier()
		{
			return new Asn1Identifier(1, constructed: true, 23);
		}

		public RfcRequest dupRequest(string base_Renamed, string filter, bool request)
		{
			return new RfcExtendedRequest(toArray());
		}

		public string getRequestDN()
		{
			return null;
		}
	}
}

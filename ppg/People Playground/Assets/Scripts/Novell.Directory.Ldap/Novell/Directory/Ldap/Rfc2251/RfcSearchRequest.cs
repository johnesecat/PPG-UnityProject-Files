using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcSearchRequest : Asn1Sequence, RfcRequest
	{
		public RfcSearchRequest(RfcLdapDN baseObject, Asn1Enumerated scope, Asn1Enumerated derefAliases, Asn1Integer sizeLimit, Asn1Integer timeLimit, Asn1Boolean typesOnly, RfcFilter filter, RfcAttributeDescriptionList attributes)
			: base(8)
		{
			add(baseObject);
			add(scope);
			add(derefAliases);
			add(sizeLimit);
			add(timeLimit);
			add(typesOnly);
			add(filter);
			add(attributes);
		}

		internal RfcSearchRequest(Asn1Object[] origRequest, string base_Renamed, string filter, bool request)
			: base(origRequest, origRequest.Length)
		{
			if (base_Renamed != null)
			{
				set_Renamed(0, new RfcLdapDN(base_Renamed));
			}
			if (request && ((Asn1Enumerated)origRequest[1]).intValue() == 1)
			{
				set_Renamed(1, new Asn1Enumerated(0));
			}
			if (filter != null)
			{
				set_Renamed(6, new RfcFilter(filter));
			}
		}

		public override Asn1Identifier getIdentifier()
		{
			return new Asn1Identifier(1, constructed: true, 3);
		}

		public RfcRequest dupRequest(string base_Renamed, string filter, bool request)
		{
			return new RfcSearchRequest(toArray(), base_Renamed, filter, request);
		}

		public string getRequestDN()
		{
			return ((RfcLdapDN)get_Renamed(0)).stringValue();
		}
	}
}

using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcAddRequest : Asn1Sequence, RfcRequest
	{
		public virtual RfcAttributeList Attributes => (RfcAttributeList)get_Renamed(1);

		public RfcAddRequest(RfcLdapDN entry, RfcAttributeList attributes)
			: base(2)
		{
			add(entry);
			add(attributes);
		}

		internal RfcAddRequest(Asn1Object[] origRequest, string base_Renamed)
			: base(origRequest, origRequest.Length)
		{
			if (base_Renamed != null)
			{
				set_Renamed(0, new RfcLdapDN(base_Renamed));
			}
		}

		public override Asn1Identifier getIdentifier()
		{
			return new Asn1Identifier(1, constructed: true, 8);
		}

		public RfcRequest dupRequest(string base_Renamed, string filter, bool request)
		{
			return new RfcAddRequest(toArray(), base_Renamed);
		}

		public string getRequestDN()
		{
			return ((RfcLdapDN)get_Renamed(0)).stringValue();
		}
	}
}

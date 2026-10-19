using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcModifyDNRequest : Asn1Sequence, RfcRequest
	{
		public RfcModifyDNRequest(RfcLdapDN entry, RfcRelativeLdapDN newrdn, Asn1Boolean deleteoldrdn)
			: this(entry, newrdn, deleteoldrdn, null)
		{
		}

		public RfcModifyDNRequest(RfcLdapDN entry, RfcRelativeLdapDN newrdn, Asn1Boolean deleteoldrdn, RfcLdapSuperDN newSuperior)
			: base(4)
		{
			add(entry);
			add(newrdn);
			add(deleteoldrdn);
			if (newSuperior != null)
			{
				newSuperior.setIdentifier(new Asn1Identifier(2, constructed: false, 0));
				add(newSuperior);
			}
		}

		internal RfcModifyDNRequest(Asn1Object[] origRequest, string base_Renamed)
			: base(origRequest, origRequest.Length)
		{
			if (base_Renamed != null)
			{
				set_Renamed(0, new RfcLdapDN(base_Renamed));
			}
		}

		public override Asn1Identifier getIdentifier()
		{
			return new Asn1Identifier(1, constructed: true, 12);
		}

		public RfcRequest dupRequest(string base_Renamed, string filter, bool request)
		{
			return new RfcModifyDNRequest(toArray(), base_Renamed);
		}

		public string getRequestDN()
		{
			return ((RfcLdapDN)get_Renamed(0)).stringValue();
		}
	}
}

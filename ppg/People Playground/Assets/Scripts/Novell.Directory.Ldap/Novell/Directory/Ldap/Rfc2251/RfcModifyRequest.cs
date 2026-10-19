using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcModifyRequest : Asn1Sequence, RfcRequest
	{
		public virtual Asn1SequenceOf Modifications => (Asn1SequenceOf)get_Renamed(1);

		public RfcModifyRequest(RfcLdapDN object_Renamed, Asn1SequenceOf modification)
			: base(2)
		{
			add(object_Renamed);
			add(modification);
		}

		internal RfcModifyRequest(Asn1Object[] origRequest, string base_Renamed)
			: base(origRequest, origRequest.Length)
		{
			if (base_Renamed != null)
			{
				set_Renamed(0, new RfcLdapDN(base_Renamed));
			}
		}

		public override Asn1Identifier getIdentifier()
		{
			return new Asn1Identifier(1, constructed: true, 6);
		}

		public RfcRequest dupRequest(string base_Renamed, string filter, bool request)
		{
			return new RfcModifyRequest(toArray(), base_Renamed);
		}

		public string getRequestDN()
		{
			return ((RfcLdapDN)get_Renamed(0)).stringValue();
		}
	}
}

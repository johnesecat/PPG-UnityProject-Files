using System;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcCompareRequest : Asn1Sequence, RfcRequest
	{
		public virtual RfcAttributeValueAssertion AttributeValueAssertion => (RfcAttributeValueAssertion)get_Renamed(1);

		public RfcCompareRequest(RfcLdapDN entry, RfcAttributeValueAssertion ava)
			: base(2)
		{
			add(entry);
			add(ava);
			if (ava.AssertionValue == null)
			{
				throw new ArgumentException("compare: Attribute must have an assertion value");
			}
		}

		internal RfcCompareRequest(Asn1Object[] origRequest, string base_Renamed)
			: base(origRequest, origRequest.Length)
		{
			if (base_Renamed != null)
			{
				set_Renamed(0, new RfcLdapDN(base_Renamed));
			}
		}

		public override Asn1Identifier getIdentifier()
		{
			return new Asn1Identifier(1, constructed: true, 14);
		}

		public RfcRequest dupRequest(string base_Renamed, string filter, bool request)
		{
			return new RfcCompareRequest(toArray(), base_Renamed);
		}

		public string getRequestDN()
		{
			return ((RfcLdapDN)get_Renamed(0)).stringValue();
		}
	}
}

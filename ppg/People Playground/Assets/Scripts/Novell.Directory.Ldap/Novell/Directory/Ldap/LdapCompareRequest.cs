using System;
using Novell.Directory.Ldap.Rfc2251;

namespace Novell.Directory.Ldap
{
	public class LdapCompareRequest : LdapMessage
	{
		public virtual string AttributeDescription => ((RfcCompareRequest)Asn1Object.getRequest()).AttributeValueAssertion.AttributeDescription;

		[CLSCompliant(false)]
		public virtual sbyte[] AssertionValue => ((RfcCompareRequest)Asn1Object.getRequest()).AttributeValueAssertion.AssertionValue;

		public virtual string DN => Asn1Object.RequestDN;

		[CLSCompliant(false)]
		public LdapCompareRequest(string dn, string name, sbyte[] value_Renamed, LdapControl[] cont)
			: base(14, new RfcCompareRequest(new RfcLdapDN(dn), new RfcAttributeValueAssertion(new RfcAttributeDescription(name), new RfcAssertionValue(value_Renamed))), cont)
		{
		}
	}
}

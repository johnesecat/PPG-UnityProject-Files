using System;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcAttributeValueAssertion : Asn1Sequence
	{
		public virtual string AttributeDescription => ((RfcAttributeDescription)get_Renamed(0)).stringValue();

		[CLSCompliant(false)]
		public virtual sbyte[] AssertionValue => ((RfcAssertionValue)get_Renamed(1)).byteValue();

		public RfcAttributeValueAssertion(RfcAttributeDescription ad, RfcAssertionValue av)
			: base(2)
		{
			add(ad);
			add(av);
		}
	}
}

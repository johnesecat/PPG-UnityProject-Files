using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcMatchingRuleAssertion : Asn1Sequence
	{
		public RfcMatchingRuleAssertion(RfcAssertionValue matchValue)
			: this(null, null, matchValue, null)
		{
		}

		public RfcMatchingRuleAssertion(RfcMatchingRuleId matchingRule, RfcAttributeDescription type, RfcAssertionValue matchValue, Asn1Boolean dnAttributes)
			: base(4)
		{
			if (matchingRule != null)
			{
				add(new Asn1Tagged(new Asn1Identifier(2, constructed: false, 1), matchingRule, explicit_Renamed: false));
			}
			if (type != null)
			{
				add(new Asn1Tagged(new Asn1Identifier(2, constructed: false, 2), type, explicit_Renamed: false));
			}
			add(new Asn1Tagged(new Asn1Identifier(2, constructed: false, 3), matchValue, explicit_Renamed: false));
			if (dnAttributes != null && dnAttributes.booleanValue())
			{
				add(new Asn1Tagged(new Asn1Identifier(2, constructed: false, 4), dnAttributes, explicit_Renamed: false));
			}
		}
	}
}

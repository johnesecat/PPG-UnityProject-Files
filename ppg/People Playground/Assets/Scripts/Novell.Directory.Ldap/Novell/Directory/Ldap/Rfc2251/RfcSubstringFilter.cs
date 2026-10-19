using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcSubstringFilter : Asn1Sequence
	{
		public RfcSubstringFilter(RfcAttributeDescription type, Asn1SequenceOf substrings)
			: base(2)
		{
			add(type);
			add(substrings);
		}
	}
}

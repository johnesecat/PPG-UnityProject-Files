using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcAttributeTypeAndValues : Asn1Sequence
	{
		public RfcAttributeTypeAndValues(RfcAttributeDescription type, Asn1SetOf vals)
			: base(2)
		{
			add(type);
			add(vals);
		}
	}
}

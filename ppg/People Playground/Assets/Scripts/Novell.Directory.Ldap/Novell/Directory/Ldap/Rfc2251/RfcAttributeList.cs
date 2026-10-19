using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcAttributeList : Asn1SequenceOf
	{
		public RfcAttributeList(int size)
			: base(size)
		{
		}
	}
}

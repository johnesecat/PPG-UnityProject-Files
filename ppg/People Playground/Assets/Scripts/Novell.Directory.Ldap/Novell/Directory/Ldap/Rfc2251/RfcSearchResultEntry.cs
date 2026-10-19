using System;
using System.IO;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcSearchResultEntry : Asn1Sequence
	{
		public virtual Asn1OctetString ObjectName => (Asn1OctetString)get_Renamed(0);

		public virtual Asn1Sequence Attributes => (Asn1Sequence)get_Renamed(1);

		[CLSCompliant(false)]
		public RfcSearchResultEntry(Asn1Decoder dec, Stream in_Renamed, int len)
			: base(dec, in_Renamed, len)
		{
		}

		public override Asn1Identifier getIdentifier()
		{
			return new Asn1Identifier(1, constructed: true, 4);
		}
	}
}

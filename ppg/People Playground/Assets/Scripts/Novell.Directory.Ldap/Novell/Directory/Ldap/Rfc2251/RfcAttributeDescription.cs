using System;
using System.IO;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcAttributeDescription : RfcLdapString
	{
		public RfcAttributeDescription(string s)
			: base(s)
		{
		}

		[CLSCompliant(false)]
		public RfcAttributeDescription(Asn1Decoder dec, Stream in_Renamed, int len)
			: base(dec, in_Renamed, len)
		{
		}
	}
}

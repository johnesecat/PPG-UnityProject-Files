using System;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcAttributeValue : Asn1OctetString
	{
		public RfcAttributeValue(string value_Renamed)
			: base(value_Renamed)
		{
		}

		[CLSCompliant(false)]
		public RfcAttributeValue(sbyte[] value_Renamed)
			: base(value_Renamed)
		{
		}
	}
}

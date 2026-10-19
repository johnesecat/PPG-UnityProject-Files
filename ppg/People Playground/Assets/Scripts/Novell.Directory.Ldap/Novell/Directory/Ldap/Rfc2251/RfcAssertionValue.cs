using System;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcAssertionValue : Asn1OctetString
	{
		[CLSCompliant(false)]
		public RfcAssertionValue(sbyte[] value_Renamed)
			: base(value_Renamed)
		{
		}
	}
}

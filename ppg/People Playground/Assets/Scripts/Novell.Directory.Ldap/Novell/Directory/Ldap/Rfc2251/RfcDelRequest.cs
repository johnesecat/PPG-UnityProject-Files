using System;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcDelRequest : RfcLdapDN, RfcRequest
	{
		public RfcDelRequest(string dn)
			: base(dn)
		{
		}

		[CLSCompliant(false)]
		public RfcDelRequest(sbyte[] dn)
			: base(dn)
		{
		}

		public override Asn1Identifier getIdentifier()
		{
			return new Asn1Identifier(1, constructed: false, 10);
		}

		public RfcRequest dupRequest(string base_Renamed, string filter, bool request)
		{
			if (base_Renamed == null)
			{
				return new RfcDelRequest(byteValue());
			}
			return new RfcDelRequest(base_Renamed);
		}

		public string getRequestDN()
		{
			return stringValue();
		}
	}
}

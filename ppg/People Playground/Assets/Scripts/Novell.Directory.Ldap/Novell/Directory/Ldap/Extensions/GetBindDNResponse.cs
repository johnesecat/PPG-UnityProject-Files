using System.IO;
using Novell.Directory.Ldap.Asn1;
using Novell.Directory.Ldap.Rfc2251;

namespace Novell.Directory.Ldap.Extensions
{
	public class GetBindDNResponse : LdapExtendedResponse
	{
		private string identity;

		public virtual string Identity => identity;

		public GetBindDNResponse(RfcLdapMessage rfcMessage)
			: base(rfcMessage)
		{
			if (ResultCode == 0)
			{
				sbyte[] value = Value;
				if (value == null)
				{
					throw new IOException("No returned value");
				}
				Asn1OctetString asn1OctetString = (Asn1OctetString)(new LBERDecoder() ?? throw new IOException("Decoding error")).decode(value);
				if (asn1OctetString == null)
				{
					throw new IOException("Decoding error");
				}
				identity = asn1OctetString.stringValue();
				if (identity == null)
				{
					throw new IOException("Decoding error");
				}
			}
			else
			{
				identity = "";
			}
		}
	}
}

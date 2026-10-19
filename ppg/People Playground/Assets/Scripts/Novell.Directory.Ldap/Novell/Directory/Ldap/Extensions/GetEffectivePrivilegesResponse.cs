using System.IO;
using Novell.Directory.Ldap.Asn1;
using Novell.Directory.Ldap.Rfc2251;

namespace Novell.Directory.Ldap.Extensions
{
	public class GetEffectivePrivilegesResponse : LdapExtendedResponse
	{
		private int privileges;

		public virtual int Privileges => privileges;

		public GetEffectivePrivilegesResponse(RfcLdapMessage rfcMessage)
			: base(rfcMessage)
		{
			if (ResultCode == 0)
			{
				sbyte[] value = Value;
				if (value == null)
				{
					throw new IOException("No returned value");
				}
				Asn1Integer asn1Integer = (Asn1Integer)(new LBERDecoder() ?? throw new IOException("Decoding error")).decode(value);
				if (asn1Integer == null)
				{
					throw new IOException("Decoding error");
				}
				privileges = asn1Integer.intValue();
			}
			else
			{
				privileges = 0;
			}
		}
	}
}

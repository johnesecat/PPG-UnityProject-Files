using System.IO;
using Novell.Directory.Ldap.Asn1;
using Novell.Directory.Ldap.Rfc2251;

namespace Novell.Directory.Ldap.Extensions
{
	public class PartitionEntryCountResponse : LdapExtendedResponse
	{
		private int count;

		public virtual int Count => count;

		public PartitionEntryCountResponse(RfcLdapMessage rfcMessage)
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
				count = asn1Integer.intValue();
			}
			else
			{
				count = -1;
			}
		}
	}
}

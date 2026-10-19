using System.IO;
using Novell.Directory.Ldap.Asn1;
using Novell.Directory.Ldap.Rfc2251;

namespace Novell.Directory.Ldap.Extensions
{
	public class GetReplicationFilterResponse : LdapExtendedResponse
	{
		internal string[][] returnedFilter;

		public virtual string[][] ReplicationFilter => returnedFilter;

		public GetReplicationFilterResponse(RfcLdapMessage rfcMessage)
			: base(rfcMessage)
		{
			if (ResultCode != 0)
			{
				returnedFilter = new string[0][];
				for (int i = 0; i < 0; i++)
				{
					returnedFilter[i] = new string[0];
				}
				return;
			}
			sbyte[] value = Value;
			if (value == null)
			{
				throw new IOException("No returned value");
			}
			Asn1Sequence asn1Sequence = (Asn1Sequence)(new LBERDecoder() ?? throw new IOException("Decoding error")).decode(value);
			if (asn1Sequence == null)
			{
				throw new IOException("Decoding error");
			}
			int num = asn1Sequence.size();
			returnedFilter = new string[num][];
			for (int j = 0; j < num; j++)
			{
				Asn1Sequence asn1Sequence2 = (Asn1Sequence)asn1Sequence.get_Renamed(j);
				if (asn1Sequence2 == null)
				{
					throw new IOException("Decoding error");
				}
				Asn1OctetString asn1OctetString = (Asn1OctetString)asn1Sequence2.get_Renamed(0);
				if (asn1OctetString == null)
				{
					break;
				}
				Asn1Sequence asn1Sequence3 = (Asn1Sequence)asn1Sequence2.get_Renamed(1);
				if (asn1Sequence3 == null)
				{
					throw new IOException("Decoding error");
				}
				int num2 = asn1Sequence3.size();
				returnedFilter[j] = new string[num2 + 1];
				returnedFilter[j][0] = asn1OctetString.stringValue();
				if (returnedFilter[j][0] == null)
				{
					throw new IOException("Decoding error");
				}
				for (int k = 0; k < num2; k++)
				{
					Asn1OctetString asn1OctetString2 = (Asn1OctetString)asn1Sequence3.get_Renamed(k);
					if (asn1OctetString2 == null)
					{
						throw new IOException("Decoding error");
					}
					returnedFilter[j][k + 1] = asn1OctetString2.stringValue();
					if (returnedFilter[j][k + 1] == null)
					{
						throw new IOException("Decoding error");
					}
				}
			}
		}
	}
}

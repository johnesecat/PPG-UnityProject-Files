using System;
using System.IO;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Extensions
{
	public class SetReplicationFilterRequest : LdapExtendedOperation
	{
		public SetReplicationFilterRequest(string serverDN, string[][] replicationFilter)
			: base("2.16.840.1.113719.1.27.100.35", null)
		{
			try
			{
				if (serverDN == null)
				{
					throw new ArgumentException("PARAM_ERROR");
				}
				MemoryStream memoryStream = new MemoryStream();
				LBEREncoder enc = new LBEREncoder();
				new Asn1OctetString(serverDN).encode(enc, memoryStream);
				Asn1SequenceOf asn1SequenceOf = new Asn1SequenceOf();
				if (replicationFilter == null)
				{
					asn1SequenceOf.encode(enc, memoryStream);
					setValue(SupportClass.ToSByteArray(memoryStream.ToArray()));
					return;
				}
				for (int i = 0; i < replicationFilter.Length && replicationFilter[i] != null; i++)
				{
					Asn1Sequence asn1Sequence = new Asn1Sequence();
					asn1Sequence.add(new Asn1OctetString(replicationFilter[i][0]));
					Asn1SequenceOf asn1SequenceOf2 = new Asn1SequenceOf();
					for (int j = 1; j < replicationFilter[i].Length && replicationFilter[i][j] != null; j++)
					{
						asn1SequenceOf2.add(new Asn1OctetString(replicationFilter[i][j]));
					}
					asn1Sequence.add(asn1SequenceOf2);
					asn1SequenceOf.add(asn1Sequence);
				}
				asn1SequenceOf.encode(enc, memoryStream);
				setValue(SupportClass.ToSByteArray(memoryStream.ToArray()));
			}
			catch (IOException)
			{
				throw new LdapException("ENCODING_ERROR", 83, null);
			}
		}
	}
}

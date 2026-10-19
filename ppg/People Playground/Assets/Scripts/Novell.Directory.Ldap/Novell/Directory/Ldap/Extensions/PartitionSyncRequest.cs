using System;
using System.IO;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Extensions
{
	public class PartitionSyncRequest : LdapExtendedOperation
	{
		public PartitionSyncRequest(string serverName, string partitionRoot, int delay)
			: base("2.16.840.1.113719.1.27.100.25", null)
		{
			try
			{
				if (serverName == null || partitionRoot == null)
				{
					throw new ArgumentException("PARAM_ERROR");
				}
				MemoryStream memoryStream = new MemoryStream();
				LBEREncoder enc = new LBEREncoder();
				Asn1OctetString asn1OctetString = new Asn1OctetString(serverName);
				Asn1OctetString asn1OctetString2 = new Asn1OctetString(partitionRoot);
				Asn1Integer asn1Integer = new Asn1Integer(delay);
				asn1OctetString.encode(enc, memoryStream);
				asn1OctetString2.encode(enc, memoryStream);
				asn1Integer.encode(enc, memoryStream);
				setValue(SupportClass.ToSByteArray(memoryStream.ToArray()));
			}
			catch (IOException)
			{
				throw new LdapException("ENCODING_ERROR", 83, null);
			}
		}
	}
}

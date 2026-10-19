using System;
using System.IO;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Extensions
{
	public class ReceiveAllUpdatesRequest : LdapExtendedOperation
	{
		public ReceiveAllUpdatesRequest(string partitionRoot, string toServerDN, string fromServerDN)
			: base("2.16.840.1.113719.1.27.100.21", null)
		{
			try
			{
				if (partitionRoot == null || toServerDN == null || fromServerDN == null)
				{
					throw new ArgumentException("PARAM_ERROR");
				}
				MemoryStream memoryStream = new MemoryStream();
				LBEREncoder enc = new LBEREncoder();
				Asn1OctetString asn1OctetString = new Asn1OctetString(partitionRoot);
				Asn1OctetString asn1OctetString2 = new Asn1OctetString(toServerDN);
				Asn1OctetString asn1OctetString3 = new Asn1OctetString(fromServerDN);
				asn1OctetString.encode(enc, memoryStream);
				asn1OctetString2.encode(enc, memoryStream);
				asn1OctetString3.encode(enc, memoryStream);
				setValue(SupportClass.ToSByteArray(memoryStream.ToArray()));
			}
			catch (IOException)
			{
				throw new LdapException("ENCODING_ERROR", 83, null);
			}
		}
	}
}

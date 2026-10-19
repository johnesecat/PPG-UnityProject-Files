using System;
using System.IO;
using System.Text;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Extensions
{
	public class LdapRestoreRequest : LdapExtendedOperation
	{
		public LdapRestoreRequest(string objectDN, byte[] passwd, int bufferLength, string chunkSizesString, byte[] returnedBuffer)
			: base("2.16.840.1.113719.1.27.100.98", null)
		{
			try
			{
				if (objectDN == null || bufferLength == 0 || chunkSizesString == null || returnedBuffer == null)
				{
					throw new ArgumentException("PARAM_ERROR");
				}
				if (passwd == null)
				{
					passwd = Encoding.UTF8.GetBytes("");
				}
				int[] array = null;
				int num = chunkSizesString.IndexOf(';');
				int num2;
				try
				{
					num2 = int.Parse(chunkSizesString.Substring(0, num));
				}
				catch (FormatException)
				{
					throw new LdapLocalException("Invalid data buffer send in the request", 83);
				}
				if (num2 == 0)
				{
					throw new ArgumentException("PARAM_ERROR");
				}
				chunkSizesString = chunkSizesString.Substring(num + 1);
				array = new int[num2];
				for (int i = 0; i < num2; i++)
				{
					int num3 = chunkSizesString.IndexOf(';');
					if (num3 == -1)
					{
						array[i] = int.Parse(chunkSizesString);
						break;
					}
					array[i] = int.Parse(chunkSizesString.Substring(0, num3));
					chunkSizesString = chunkSizesString.Substring(num3 + 1);
				}
				MemoryStream memoryStream = new MemoryStream();
				LBEREncoder enc = new LBEREncoder();
				Asn1OctetString asn1OctetString = new Asn1OctetString(objectDN);
				Asn1OctetString asn1OctetString2 = new Asn1OctetString(SupportClass.ToSByteArray(passwd));
				Asn1Integer asn1Integer = new Asn1Integer(bufferLength);
				Asn1OctetString asn1OctetString3 = new Asn1OctetString(SupportClass.ToSByteArray(returnedBuffer));
				Asn1Sequence asn1Sequence = new Asn1Sequence();
				asn1Sequence.add(new Asn1Integer(num2));
				Asn1Set asn1Set = new Asn1Set();
				for (int j = 0; j < num2; j++)
				{
					Asn1Integer value_Renamed = new Asn1Integer(array[j]);
					Asn1Sequence asn1Sequence2 = new Asn1Sequence();
					asn1Sequence2.add(value_Renamed);
					asn1Set.add(asn1Sequence2);
				}
				asn1Sequence.add(asn1Set);
				asn1OctetString.encode(enc, memoryStream);
				asn1OctetString2.encode(enc, memoryStream);
				asn1Integer.encode(enc, memoryStream);
				asn1OctetString3.encode(enc, memoryStream);
				asn1Sequence.encode(enc, memoryStream);
				setValue(SupportClass.ToSByteArray(memoryStream.ToArray()));
			}
			catch (IOException)
			{
				throw new LdapException("ENCODING_ERROR", 83, null);
			}
		}
	}
}

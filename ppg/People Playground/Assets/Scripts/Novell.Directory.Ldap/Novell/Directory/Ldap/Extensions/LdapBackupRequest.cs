using System;
using System.IO;
using System.Text;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Extensions
{
	public class LdapBackupRequest : LdapExtendedOperation
	{
		static LdapBackupRequest()
		{
			try
			{
				LdapExtendedResponse.register("2.16.840.1.113719.1.27.100.97", Type.GetType("Novell.Directory.Ldap.Extensions.LdapBackupResponse"));
			}
			catch (TypeLoadException)
			{
				Console.Error.WriteLine("Could not register Extended Response - Class not found");
			}
			catch (Exception ex2)
			{
				Console.Error.WriteLine(ex2.StackTrace);
			}
		}

		public LdapBackupRequest(string objectDN, byte[] passwd, string stateInfo)
			: base("2.16.840.1.113719.1.27.100.96", null)
		{
			try
			{
				if (objectDN == null)
				{
					throw new ArgumentException("PARAM_ERROR");
				}
				if (passwd == null)
				{
					passwd = Encoding.UTF8.GetBytes("");
				}
				int content;
				int content2;
				if (stateInfo == null)
				{
					content = 0;
					content2 = 0;
				}
				else
				{
					stateInfo = stateInfo.Trim();
					int num = stateInfo.IndexOf('+');
					if (num == -1)
					{
						throw new ArgumentException("PARAM_ERROR");
					}
					string s = stateInfo.Substring(0, num);
					string s2 = stateInfo.Substring(num + 1);
					try
					{
						content = int.Parse(s);
					}
					catch (FormatException)
					{
						throw new LdapLocalException("Invalid Modification Timestamp send in the request", 83);
					}
					try
					{
						content2 = int.Parse(s2);
					}
					catch (FormatException)
					{
						throw new LdapLocalException("Invalid Revision send in the request", 83);
					}
				}
				MemoryStream memoryStream = new MemoryStream();
				LBEREncoder enc = new LBEREncoder();
				Asn1OctetString asn1OctetString = new Asn1OctetString(objectDN);
				Asn1Integer asn1Integer = new Asn1Integer(content);
				Asn1Integer asn1Integer2 = new Asn1Integer(content2);
				Asn1OctetString asn1OctetString2 = new Asn1OctetString(SupportClass.ToSByteArray(passwd));
				asn1OctetString.encode(enc, memoryStream);
				asn1Integer.encode(enc, memoryStream);
				asn1Integer2.encode(enc, memoryStream);
				asn1OctetString2.encode(enc, memoryStream);
				setValue(SupportClass.ToSByteArray(memoryStream.ToArray()));
			}
			catch (IOException)
			{
				throw new LdapException("ENCODING_ERROR", 83, null);
			}
		}
	}
}

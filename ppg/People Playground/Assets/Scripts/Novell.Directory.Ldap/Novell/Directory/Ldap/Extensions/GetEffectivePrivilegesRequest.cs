using System;
using System.IO;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Extensions
{
	public class GetEffectivePrivilegesRequest : LdapExtendedOperation
	{
		static GetEffectivePrivilegesRequest()
		{
			try
			{
				LdapExtendedResponse.register("2.16.840.1.113719.1.27.100.34", Type.GetType("Novell.Directory.Ldap.Extensions.GetEffectivePrivilegesResponse"));
			}
			catch (Exception)
			{
				Console.Error.WriteLine("Could not register Extended Response - Class not found");
			}
		}

		public GetEffectivePrivilegesRequest(string dn, string trusteeDN, string attrName)
			: base("2.16.840.1.113719.1.27.100.33", null)
		{
			try
			{
				if (dn == null)
				{
					throw new ArgumentException("PARAM_ERROR");
				}
				MemoryStream memoryStream = new MemoryStream();
				LBEREncoder enc = new LBEREncoder();
				Asn1OctetString asn1OctetString = new Asn1OctetString(dn);
				Asn1OctetString asn1OctetString2 = new Asn1OctetString(trusteeDN);
				Asn1OctetString asn1OctetString3 = new Asn1OctetString(attrName);
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

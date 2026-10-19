using System;
using System.Reflection;
using Novell.Directory.Ldap.Rfc2251;

namespace Novell.Directory.Ldap.Utilclass
{
	public class ExtResponseFactory
	{
		public static LdapExtendedResponse convertToExtendedResponse(RfcLdapMessage inResponse)
		{
			LdapExtendedResponse ldapExtendedResponse = new LdapExtendedResponse(inResponse);
			string iD = ldapExtendedResponse.ID;
			RespExtensionSet registeredResponses = LdapExtendedResponse.RegisteredResponses;
			try
			{
				Type type = registeredResponses.findResponseExtension(iD);
				if (type == null)
				{
					return ldapExtendedResponse;
				}
				Type[] types = new Type[1] { typeof(RfcLdapMessage) };
				object[] parameters = new object[1] { inResponse };
				try
				{
					ConstructorInfo constructor = type.GetConstructor(types);
					try
					{
						return (LdapExtendedResponse)constructor.Invoke(parameters);
					}
					catch (UnauthorizedAccessException)
					{
						return ldapExtendedResponse;
					}
					catch (TargetInvocationException)
					{
						return ldapExtendedResponse;
					}
					catch (Exception)
					{
						return ldapExtendedResponse;
					}
				}
				catch (MethodAccessException)
				{
					return ldapExtendedResponse;
				}
			}
			catch (FieldAccessException)
			{
				return ldapExtendedResponse;
			}
		}
	}
}

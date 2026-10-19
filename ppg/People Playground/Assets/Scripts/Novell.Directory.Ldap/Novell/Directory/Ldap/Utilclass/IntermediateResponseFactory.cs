using System;
using System.Reflection;
using Novell.Directory.Ldap.Rfc2251;

namespace Novell.Directory.Ldap.Utilclass
{
	public class IntermediateResponseFactory
	{
		public static LdapIntermediateResponse convertToIntermediateResponse(RfcLdapMessage inResponse)
		{
			LdapIntermediateResponse ldapIntermediateResponse = new LdapIntermediateResponse(inResponse);
			string iD = ldapIntermediateResponse.getID();
			RespExtensionSet registeredResponses = LdapIntermediateResponse.getRegisteredResponses();
			try
			{
				Type type = registeredResponses.findResponseExtension(iD);
				if (type == null)
				{
					return ldapIntermediateResponse;
				}
				Type[] types = new Type[1] { typeof(RfcLdapMessage) };
				object[] parameters = new object[1] { inResponse };
				try
				{
					ConstructorInfo constructor = type.GetConstructor(types);
					try
					{
						return (LdapIntermediateResponse)constructor.Invoke(parameters);
					}
					catch (UnauthorizedAccessException)
					{
						return ldapIntermediateResponse;
					}
					catch (TargetInvocationException)
					{
						return ldapIntermediateResponse;
					}
				}
				catch (MissingMethodException)
				{
					return ldapIntermediateResponse;
				}
			}
			catch (MissingFieldException)
			{
				return ldapIntermediateResponse;
			}
		}
	}
}

using System;
using Novell.Directory.Ldap.Rfc2251;
using Novell.Directory.Ldap.Utilclass;

namespace Novell.Directory.Ldap
{
	public class LdapIntermediateResponse : LdapResponse
	{
		private static RespExtensionSet registeredResponses = new RespExtensionSet();

		public static void register(string oid, Type extendedResponseClass)
		{
			registeredResponses.registerResponseExtension(oid, extendedResponseClass);
		}

		public static RespExtensionSet getRegisteredResponses()
		{
			return registeredResponses;
		}

		public LdapIntermediateResponse(RfcLdapMessage message)
			: base(message)
		{
		}

		public string getID()
		{
			return ((RfcIntermediateResponse)message.Response).getResponseName()?.stringValue();
		}

		[CLSCompliant(false)]
		public sbyte[] getValue()
		{
			return ((RfcIntermediateResponse)message.Response).getResponse()?.byteValue();
		}
	}
}

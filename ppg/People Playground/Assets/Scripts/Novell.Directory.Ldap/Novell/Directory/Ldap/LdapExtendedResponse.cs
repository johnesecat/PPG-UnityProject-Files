using System;
using Novell.Directory.Ldap.Rfc2251;
using Novell.Directory.Ldap.Utilclass;

namespace Novell.Directory.Ldap
{
	public class LdapExtendedResponse : LdapResponse
	{
		private static RespExtensionSet registeredResponses;

		public virtual string ID => ((RfcExtendedResponse)message.Response).ResponseName?.stringValue();

		public static RespExtensionSet RegisteredResponses => registeredResponses;

		[CLSCompliant(false)]
		public virtual sbyte[] Value => ((RfcExtendedResponse)message.Response).Response?.byteValue();

		static LdapExtendedResponse()
		{
			registeredResponses = new RespExtensionSet();
		}

		public LdapExtendedResponse(RfcLdapMessage message)
			: base(message)
		{
		}

		public static void register(string oid, Type extendedResponseClass)
		{
			registeredResponses.registerResponseExtension(oid, extendedResponseClass);
		}
	}
}

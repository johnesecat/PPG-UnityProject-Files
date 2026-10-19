using System;

namespace Novell.Directory.Ldap.Extensions
{
	public class GetBindDNRequest : LdapExtendedOperation
	{
		static GetBindDNRequest()
		{
			try
			{
				LdapExtendedResponse.register("2.16.840.1.113719.1.27.100.32", Type.GetType("Novell.Directory.Ldap.Extensions.GetBindDNResponse"));
			}
			catch (Exception)
			{
				Console.Error.WriteLine("Could not register Extended Response - Class not found");
			}
		}

		public GetBindDNRequest()
			: base("2.16.840.1.113719.1.27.100.31", null)
		{
		}
	}
}

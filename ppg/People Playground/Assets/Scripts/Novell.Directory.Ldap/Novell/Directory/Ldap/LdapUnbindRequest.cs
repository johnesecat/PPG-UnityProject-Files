using Novell.Directory.Ldap.Rfc2251;

namespace Novell.Directory.Ldap
{
	public class LdapUnbindRequest : LdapMessage
	{
		public LdapUnbindRequest(LdapControl[] cont)
			: base(2, new RfcUnbindRequest(), cont)
		{
		}
	}
}

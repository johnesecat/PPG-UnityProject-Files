using Novell.Directory.Ldap.Rfc2251;

namespace Novell.Directory.Ldap
{
	public class LdapAbandonRequest : LdapMessage
	{
		public LdapAbandonRequest(int id, LdapControl[] cont)
			: base(16, new RfcAbandonRequest(id), cont)
		{
		}
	}
}

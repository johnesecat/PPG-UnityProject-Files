namespace Novell.Directory.Ldap.Extensions
{
	public class RefreshLdapServerRequest : LdapExtendedOperation
	{
		public RefreshLdapServerRequest()
			: base("2.16.840.1.113719.1.27.100.9", null)
		{
		}
	}
}

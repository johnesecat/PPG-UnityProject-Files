namespace Novell.Directory.Ldap
{
	public interface LdapAuthHandler : LdapReferralHandler
	{
		LdapAuthProvider getAuthProvider(string host, int port);
	}
}

namespace Novell.Directory.Ldap
{
	public interface LdapBindHandler : LdapReferralHandler
	{
		LdapConnection Bind(string[] ldapurl, LdapConnection conn);
	}
}

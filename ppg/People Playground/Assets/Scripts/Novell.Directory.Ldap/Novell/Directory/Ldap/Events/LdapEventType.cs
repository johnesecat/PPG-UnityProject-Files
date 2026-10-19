namespace Novell.Directory.Ldap.Events
{
	public enum LdapEventType
	{
		TYPE_UNKNOWN = -1,
		LDAP_PSEARCH_ADD = 1,
		LDAP_PSEARCH_DELETE = 2,
		LDAP_PSEARCH_MODIFY = 4,
		LDAP_PSEARCH_MODDN = 8,
		LDAP_PSEARCH_ANY = 0xF
	}
}

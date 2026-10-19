namespace Novell.Directory.Ldap.Events
{
	public class DirectoryExceptionEventArgs : BaseEventArgs
	{
		protected LdapException ldap_exception_object;

		public LdapException LdapExceptionObject => ldap_exception_object;

		public DirectoryExceptionEventArgs(LdapMessage message, LdapException ldapException)
			: base(message)
		{
			ldap_exception_object = ldapException;
		}
	}
}

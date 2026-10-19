namespace Novell.Directory.Ldap
{
	public interface LdapUnsolicitedNotificationListener
	{
		void messageReceived(LdapExtendedResponse msg);
	}
}

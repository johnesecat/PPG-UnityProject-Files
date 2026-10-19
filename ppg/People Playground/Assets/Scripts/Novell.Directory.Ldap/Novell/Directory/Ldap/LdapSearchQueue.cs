namespace Novell.Directory.Ldap
{
	public class LdapSearchQueue : LdapMessageQueue
	{
		internal LdapSearchQueue(MessageAgent agent)
			: base("LdapSearchQueue", agent)
		{
		}

		public virtual void merge(LdapMessageQueue queue2)
		{
			LdapSearchQueue ldapSearchQueue = (LdapSearchQueue)queue2;
			agent.merge(ldapSearchQueue.MessageAgent);
		}
	}
}

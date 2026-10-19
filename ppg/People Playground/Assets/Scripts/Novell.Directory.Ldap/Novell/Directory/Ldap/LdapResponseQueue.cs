namespace Novell.Directory.Ldap
{
	public class LdapResponseQueue : LdapMessageQueue
	{
		internal LdapResponseQueue(MessageAgent agent)
			: base("LdapResponseQueue", agent)
		{
		}

		public virtual void merge(LdapMessageQueue queue2)
		{
			LdapResponseQueue ldapResponseQueue = (LdapResponseQueue)queue2;
			agent.merge(ldapResponseQueue.MessageAgent);
		}
	}
}

using Novell.Directory.Ldap.Rfc2251;
using Novell.Directory.Ldap.Utilclass;

namespace Novell.Directory.Ldap
{
	public abstract class LdapMessageQueue
	{
		internal MessageAgent agent;

		internal string name = "";

		internal static object nameLock;

		internal static int queueNum;

		internal virtual string DebugName => name;

		internal virtual MessageAgent MessageAgent => agent;

		public virtual int[] MessageIDs => agent.MessageIDs;

		internal LdapMessageQueue(string myname, MessageAgent agent)
		{
			this.agent = agent;
		}

		public virtual LdapMessage getResponse()
		{
			return getResponse(null);
		}

		public virtual LdapMessage getResponse(int msgid)
		{
			return getResponse(new Integer32(msgid));
		}

		private LdapMessage getResponse(Integer32 msgid)
		{
			object ldapMessage;
			if ((ldapMessage = agent.getLdapMessage(msgid)) == null)
			{
				return null;
			}
			if (ldapMessage is LdapResponse)
			{
				return (LdapMessage)ldapMessage;
			}
			RfcLdapMessage rfcLdapMessage = (RfcLdapMessage)ldapMessage;
			switch (rfcLdapMessage.Type)
			{
			case 4:
				return new LdapSearchResult(rfcLdapMessage);
			case 19:
				return new LdapSearchResultReference(rfcLdapMessage);
			case 24:
				new ExtResponseFactory();
				return ExtResponseFactory.convertToExtendedResponse(rfcLdapMessage);
			case 25:
				return IntermediateResponseFactory.convertToIntermediateResponse(rfcLdapMessage);
			default:
				return new LdapResponse(rfcLdapMessage);
			}
		}

		public virtual bool isResponseReceived()
		{
			return agent.isResponseReceived();
		}

		public virtual bool isResponseReceived(int msgid)
		{
			return agent.isResponseReceived(msgid);
		}

		public virtual bool isComplete(int msgid)
		{
			return agent.isComplete(msgid);
		}

		static LdapMessageQueue()
		{
			nameLock = new object();
		}
	}
}

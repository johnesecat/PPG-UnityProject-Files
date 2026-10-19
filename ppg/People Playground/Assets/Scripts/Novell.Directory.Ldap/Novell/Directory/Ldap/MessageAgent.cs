using System;
using System.Threading;
using Novell.Directory.Ldap.Utilclass;

namespace Novell.Directory.Ldap
{
	internal class MessageAgent
	{
		private MessageVector messages;

		private int indexLastRead;

		private static object nameLock;

		private static int agentNum;

		private string name;

		internal virtual object[] MessageArray => messages.ObjectArray;

		internal virtual int[] MessageIDs
		{
			get
			{
				int count = messages.Count;
				int[] array = new int[count];
				for (int i = 0; i < count; i++)
				{
					Message message = (Message)messages[i];
					array[i] = message.MessageID;
				}
				return array;
			}
		}

		internal virtual string AgentName => name;

		internal virtual int Count
		{
			get
			{
				int num = 0;
				for (int i = 0; i < messages.Count; i++)
				{
					Message message = (Message)messages[i];
					num += message.Count;
				}
				return num;
			}
		}

		private void InitBlock()
		{
			messages = new MessageVector(5, 5);
		}

		internal MessageAgent()
		{
			InitBlock();
		}

		internal void merge(MessageAgent fromAgent)
		{
			object[] messageArray = fromAgent.MessageArray;
			for (int i = 0; i < messageArray.Length; i++)
			{
				messages.Add(messageArray[i]);
				((Message)messageArray[i]).Agent = this;
			}
			lock (messages.SyncRoot)
			{
				if (messageArray.Length > 1)
				{
					Monitor.PulseAll(messages.SyncRoot);
				}
				else if (messageArray.Length == 1)
				{
					Monitor.Pulse(messages.SyncRoot);
				}
			}
		}

		internal void sleepersAwake(bool all)
		{
			lock (messages.SyncRoot)
			{
				if (all)
				{
					Monitor.PulseAll(messages.SyncRoot);
				}
				else
				{
					Monitor.Pulse(messages.SyncRoot);
				}
			}
		}

		internal bool isResponseReceived()
		{
			int count = messages.Count;
			int num = indexLastRead + 1;
			for (int i = 0; i < count; i++)
			{
				if (num == count)
				{
					num = 0;
				}
				if (((Message)messages[num]).hasReplies())
				{
					return true;
				}
			}
			return false;
		}

		internal bool isResponseReceived(int msgId)
		{
			try
			{
				return messages.findMessageById(msgId).hasReplies();
			}
			catch (FieldAccessException)
			{
				return false;
			}
		}

		internal void Abandon(int msgId, LdapConstraints cons)
		{
			Message message = null;
			try
			{
				message = messages.findMessageById(msgId);
				SupportClass.VectorRemoveElement(messages, message);
				message.Abandon(cons, null);
			}
			catch (FieldAccessException)
			{
			}
		}

		internal void AbandonAll()
		{
			int count = messages.Count;
			for (int i = 0; i < count; i++)
			{
				Message message = (Message)messages[i];
				SupportClass.VectorRemoveElement(messages, message);
				message.Abandon(null, null);
			}
		}

		internal bool isComplete(int msgid)
		{
			try
			{
				if (!messages.findMessageById(msgid).Complete)
				{
					return false;
				}
			}
			catch (FieldAccessException)
			{
			}
			return true;
		}

		internal Message getMessage(int msgid)
		{
			return messages.findMessageById(msgid);
		}

		internal void sendMessage(Connection conn, LdapMessage msg, int timeOut, LdapMessageQueue queue, BindProperties bindProps)
		{
			Message message = new Message(msg, timeOut, conn, this, queue, bindProps);
			messages.Add(message);
			message.sendMessage();
		}

		internal object getLdapMessage(int msgId)
		{
			return getLdapMessage(new Integer32(msgId));
		}

		internal object getLdapMessage(Integer32 msgId)
		{
			if (messages.Count == 0)
			{
				return null;
			}
			if (msgId != null)
			{
				try
				{
					Message message = messages.findMessageById(msgId.intValue);
					object result = message.waitForReply();
					if (!message.acceptsReplies() && !message.hasReplies())
					{
						SupportClass.VectorRemoveElement(messages, message);
						message.Abandon(null, null);
					}
					return result;
				}
				catch (FieldAccessException)
				{
					return null;
				}
			}
			lock (messages.SyncRoot)
			{
				while (true)
				{
					int num = indexLastRead + 1;
					for (int i = 0; i < messages.Count; i++)
					{
						if (num >= messages.Count)
						{
							num = 0;
						}
						Message message2 = (Message)messages[num];
						indexLastRead = num++;
						object result = message2.Reply;
						if (!message2.acceptsReplies() && !message2.hasReplies())
						{
							SupportClass.VectorRemoveElement(messages, message2);
							message2.Abandon(null, null);
							i--;
						}
						if (result != null)
						{
							return result;
						}
					}
					if (messages.Count == 0)
					{
						break;
					}
					try
					{
						Monitor.Wait(messages.SyncRoot);
					}
					catch (ThreadInterruptedException)
					{
					}
				}
				return null;
			}
		}

		private void debugDisplayMessages()
		{
		}

		static MessageAgent()
		{
			nameLock = new object();
		}
	}
}

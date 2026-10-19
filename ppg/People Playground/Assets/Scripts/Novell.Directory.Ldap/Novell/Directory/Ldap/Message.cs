using System;
using System.Threading;
using Novell.Directory.Ldap.Rfc2251;
using Novell.Directory.Ldap.Utilclass;

namespace Novell.Directory.Ldap
{
	internal class Message
	{
		private sealed class Timeout : SupportClass.ThreadClass
		{
			private Message enclosingInstance;

			private int timeToWait;

			private Message message;

			public Message Enclosing_Instance => enclosingInstance;

			private void InitBlock(Message enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}

			internal Timeout(Message enclosingInstance, int interval, Message msg)
			{
				InitBlock(enclosingInstance);
				timeToWait = interval;
				message = msg;
			}

			public override void Run()
			{
				try
				{
					Thread.Sleep(new TimeSpan(10000 * timeToWait));
					message.acceptReplies = false;
					message.Abandon(null, new InterThreadException("Client request timed out", null, 85, null, message));
				}
				catch (ThreadInterruptedException)
				{
				}
			}
		}

		private LdapMessage msg;

		private Connection conn;

		private MessageAgent agent;

		private LdapMessageQueue queue;

		private int mslimit;

		private SupportClass.ThreadClass timer;

		private MessageVector replies;

		private int msgId;

		private bool acceptReplies = true;

		private bool waitForReply_Renamed_Field = true;

		private bool complete;

		private string name;

		private BindProperties bindprops;

		internal virtual int Count
		{
			get
			{
				int count = replies.Count;
				if (complete)
				{
					if (count <= 0)
					{
						return count;
					}
					return count - 1;
				}
				return count;
			}
		}

		internal virtual MessageAgent Agent
		{
			set
			{
				agent = value;
			}
		}

		internal virtual int MessageType
		{
			get
			{
				if (msg == null)
				{
					return -1;
				}
				return msg.Type;
			}
		}

		internal virtual int MessageID => msgId;

		internal virtual bool Complete => complete;

		internal virtual object Reply
		{
			get
			{
				if (replies == null)
				{
					return null;
				}
				object result;
				lock (replies.SyncRoot)
				{
					if (replies.Count == 0)
					{
						return null;
					}
					object obj = replies[0];
					replies.RemoveAt(0);
					result = obj;
				}
				if (conn != null && (complete || !acceptReplies) && replies.Count == 0)
				{
					conn.removeMessage(this);
				}
				return result;
			}
		}

		internal virtual LdapMessage Request => msg;

		internal virtual bool BindRequest => bindprops != null;

		internal virtual MessageAgent MessageAgent => agent;

		private void InitBlock()
		{
			replies = new MessageVector(5, 5);
		}

		internal virtual bool hasReplies()
		{
			if (replies == null)
			{
				return false;
			}
			return replies.Count > 0;
		}

		internal virtual object waitForReply()
		{
			if (replies == null)
			{
				return null;
			}
			lock (replies.SyncRoot)
			{
				object obj = null;
				while (waitForReply_Renamed_Field)
				{
					if (replies.Count == 0)
					{
						try
						{
							Monitor.Wait(replies.SyncRoot);
						}
						catch (ThreadInterruptedException)
						{
						}
						if (!waitForReply_Renamed_Field)
						{
							break;
						}
						continue;
					}
					object obj2 = replies[0];
					replies.RemoveAt(0);
					obj = obj2;
					if ((complete || !acceptReplies) && replies.Count == 0)
					{
						conn.removeMessage(this);
					}
					return obj;
				}
				return null;
			}
		}

		internal virtual bool acceptsReplies()
		{
			return acceptReplies;
		}

		internal Message(LdapMessage msg, int mslimit, Connection conn, MessageAgent agent, LdapMessageQueue queue, BindProperties bindprops)
		{
			InitBlock();
			this.msg = msg;
			this.conn = conn;
			this.agent = agent;
			this.queue = queue;
			this.mslimit = mslimit;
			msgId = msg.MessageID;
			this.bindprops = bindprops;
		}

		internal void sendMessage()
		{
			conn.writeMessage(this);
			if (mslimit != 0)
			{
				int type = msg.Type;
				if (type == 2 || type == 16)
				{
					mslimit = 0;
					return;
				}
				timer = new Timeout(this, mslimit, this);
				timer.IsBackground = true;
				timer.Start();
			}
		}

		internal virtual void Abandon(LdapConstraints cons, InterThreadException informUserEx)
		{
			if (!waitForReply_Renamed_Field)
			{
				return;
			}
			acceptReplies = false;
			waitForReply_Renamed_Field = false;
			if (!complete)
			{
				try
				{
					if (bindprops != null)
					{
						int bindSemId;
						if (conn.BindSemIdClear)
						{
							bindSemId = msgId;
						}
						else
						{
							bindSemId = conn.BindSemId;
							conn.clearBindSemId();
						}
						conn.freeWriteSemaphore(bindSemId);
					}
					LdapControl[] cont = null;
					if (cons != null)
					{
						cont = cons.getControls();
					}
					LdapMessage ldapMessage = new LdapAbandonRequest(msgId, cont);
					conn.writeMessage(ldapMessage);
				}
				catch (LdapException)
				{
				}
				if (informUserEx == null)
				{
					agent.Abandon(msgId, null);
				}
				conn.removeMessage(this);
			}
			if (informUserEx != null)
			{
				replies.Add(new LdapResponse(informUserEx, conn.ActiveReferral));
				stopTimer();
				sleepersAwake();
			}
			else
			{
				sleepersAwake();
				cleanup();
			}
		}

		private void cleanup()
		{
			stopTimer();
			try
			{
				acceptReplies = false;
				if (conn != null)
				{
					conn.removeMessage(this);
				}
				if (replies != null)
				{
					while (replies.Count != 0)
					{
						_ = replies[0];
						replies.RemoveAt(0);
					}
				}
			}
			catch (Exception)
			{
			}
			conn = null;
			msg = null;
			queue = null;
			bindprops = null;
		}

		~Message()
		{
			cleanup();
		}

		internal virtual void putReply(RfcLdapMessage message)
		{
			if (!acceptReplies)
			{
				return;
			}
			lock (replies)
			{
				replies.Add(message);
			}
			message.RequestingMessage = msg;
			int type = message.Type;
			if (type != 4 && type != 19 && type != 25)
			{
				stopTimer();
				acceptReplies = false;
				complete = true;
				if (bindprops != null)
				{
					int num = ((RfcResponse)message.Response).getResultCode().intValue();
					if (num != 14)
					{
						if (num == 0)
						{
							conn.BindProperties = bindprops;
						}
						int bindSemId;
						if (conn.BindSemIdClear)
						{
							bindSemId = msgId;
						}
						else
						{
							bindSemId = conn.BindSemId;
							conn.clearBindSemId();
						}
						conn.freeWriteSemaphore(bindSemId);
					}
				}
			}
			sleepersAwake();
		}

		internal virtual void stopTimer()
		{
			if (timer != null)
			{
				timer.Interrupt();
			}
		}

		private void sleepersAwake()
		{
			lock (replies.SyncRoot)
			{
				Monitor.Pulse(replies.SyncRoot);
			}
			agent.sleepersAwake(all: false);
		}
	}
}

using System;
using System.Threading;

namespace Novell.Directory.Ldap.Events
{
	public abstract class LdapEventSource
	{
		protected enum LISTENERS_COUNT
		{
			ZERO,
			ONE,
			MORE_THAN_ONE
		}

		public delegate void DirectoryEventHandler(object source, DirectoryEventArgs objDirectoryEventArgs);

		public delegate void DirectoryExceptionEventHandler(object source, DirectoryExceptionEventArgs objDirectoryExceptionEventArgs);

		protected class EventsGenerator
		{
			private LdapEventSource m_objLdapEventSource;

			private LdapMessageQueue searchqueue;

			private int messageid;

			private LdapConnection ldapconnection;

			private volatile bool isrunning = true;

			private int sleep_time;

			public int SleepTime
			{
				get
				{
					return sleep_time;
				}
				set
				{
					sleep_time = value;
				}
			}

			public EventsGenerator(LdapEventSource objEventSource, LdapMessageQueue queue, LdapConnection conn, int msgid)
			{
				m_objLdapEventSource = objEventSource;
				searchqueue = queue;
				ldapconnection = conn;
				messageid = msgid;
				sleep_time = 1000;
			}

			protected void Run()
			{
				while (isrunning)
				{
					LdapMessage ldapMessage = null;
					try
					{
						while (isrunning && !searchqueue.isResponseReceived(messageid))
						{
							try
							{
								Thread.Sleep(sleep_time);
							}
							catch (ThreadInterruptedException arg)
							{
								Console.WriteLine("EventsGenerator::Run Got ThreadInterruptedException e = {0}", arg);
							}
						}
						if (isrunning)
						{
							ldapMessage = searchqueue.getResponse(messageid);
						}
						if (ldapMessage != null)
						{
							processmessage(ldapMessage);
						}
					}
					catch (LdapException ldapException)
					{
						m_objLdapEventSource.NotifyExceptionListeners(ldapMessage, ldapException);
					}
				}
			}

			protected void processmessage(LdapMessage response)
			{
				if (response is LdapResponse)
				{
					try
					{
						((LdapResponse)response).chkResultCode();
						m_objLdapEventSource.NotifyEventListeners(response, EventClassifiers.CLASSIFICATION_UNKNOWN, -1);
					}
					catch (LdapException ldapException)
					{
						m_objLdapEventSource.NotifyExceptionListeners(response, ldapException);
					}
				}
				else
				{
					m_objLdapEventSource.NotifyEventListeners(response, EventClassifiers.CLASSIFICATION_UNKNOWN, -1);
				}
			}

			public void StartEventPolling()
			{
				isrunning = true;
				new Thread(Run).Start();
			}

			public void StopEventPolling()
			{
				isrunning = false;
			}
		}

		protected internal const int EVENT_TYPE_UNKNOWN = -1;

		protected const int DEFAULT_SLEEP_TIME = 1000;

		protected int sleep_interval = 1000;

		protected DirectoryEventHandler directory_event;

		protected DirectoryExceptionEventHandler directory_exception_event;

		protected EventsGenerator m_objEventsGenerator;

		public int SleepInterval
		{
			get
			{
				return sleep_interval;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("SleepInterval", "cannot take the negative or zero values ");
				}
				sleep_interval = value;
			}
		}

		public event DirectoryEventHandler DirectoryEvent
		{
			add
			{
				directory_event = (DirectoryEventHandler)Delegate.Combine(directory_event, value);
				ListenerAdded();
			}
			remove
			{
				directory_event = (DirectoryEventHandler)Delegate.Remove(directory_event, value);
				ListenerRemoved();
			}
		}

		public event DirectoryExceptionEventHandler DirectoryExceptionEvent
		{
			add
			{
				directory_exception_event = (DirectoryExceptionEventHandler)Delegate.Combine(directory_exception_event, value);
				ListenerAdded();
			}
			remove
			{
				directory_exception_event = (DirectoryExceptionEventHandler)Delegate.Remove(directory_exception_event, value);
				ListenerRemoved();
			}
		}

		protected abstract int GetListeners();

		protected LISTENERS_COUNT GetCurrentListenersState()
		{
			int num = 0;
			num += GetListeners();
			if (directory_event != null)
			{
				num += directory_event.GetInvocationList().Length;
			}
			if (directory_exception_event != null)
			{
				num += directory_exception_event.GetInvocationList().Length;
			}
			if (num == 0)
			{
				return LISTENERS_COUNT.ZERO;
			}
			if (1 == num)
			{
				return LISTENERS_COUNT.ONE;
			}
			return LISTENERS_COUNT.MORE_THAN_ONE;
		}

		protected void ListenerAdded()
		{
			switch (GetCurrentListenersState())
			{
			case LISTENERS_COUNT.ONE:
				StartSearchAndPolling();
				break;
			case LISTENERS_COUNT.ZERO:
			case LISTENERS_COUNT.MORE_THAN_ONE:
				break;
			}
		}

		protected void ListenerRemoved()
		{
			LISTENERS_COUNT currentListenersState = GetCurrentListenersState();
			if (currentListenersState != 0)
			{
				_ = currentListenersState - 1;
				_ = 1;
			}
			else
			{
				StopSearchAndPolling();
			}
		}

		protected abstract void StartSearchAndPolling();

		protected abstract void StopSearchAndPolling();

		protected void StartEventPolling(LdapMessageQueue queue, LdapConnection conn, int msgid)
		{
			if (queue == null || conn == null)
			{
				throw new ArgumentException("No parameter can be Null.");
			}
			if (m_objEventsGenerator == null)
			{
				m_objEventsGenerator = new EventsGenerator(this, queue, conn, msgid);
				m_objEventsGenerator.SleepTime = sleep_interval;
				m_objEventsGenerator.StartEventPolling();
			}
		}

		protected void StopEventPolling()
		{
			if (m_objEventsGenerator != null)
			{
				m_objEventsGenerator.StopEventPolling();
				m_objEventsGenerator = null;
			}
		}

		protected abstract bool NotifyEventListeners(LdapMessage sourceMessage, EventClassifiers aClassification, int nType);

		protected void NotifyListeners(LdapMessage sourceMessage, EventClassifiers aClassification, int nType)
		{
			if (!NotifyEventListeners(sourceMessage, aClassification, nType))
			{
				NotifyDirectoryListeners(sourceMessage, aClassification);
			}
		}

		protected void NotifyDirectoryListeners(LdapMessage sourceMessage, EventClassifiers aClassification)
		{
			NotifyDirectoryListeners(new DirectoryEventArgs(sourceMessage, aClassification));
		}

		protected void NotifyDirectoryListeners(DirectoryEventArgs objDirectoryEventArgs)
		{
			if (directory_event != null)
			{
				directory_event(this, objDirectoryEventArgs);
			}
		}

		protected void NotifyExceptionListeners(LdapMessage sourceMessage, LdapException ldapException)
		{
			if (directory_exception_event != null)
			{
				directory_exception_event(this, new DirectoryExceptionEventArgs(sourceMessage, ldapException));
			}
		}
	}
}

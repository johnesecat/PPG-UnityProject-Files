using System;

namespace Novell.Directory.Ldap.Events.Edir
{
	public class EdirEventSource : LdapEventSource
	{
		public delegate void EdirEventHandler(object source, EdirEventArgs objEdirEventArgs);

		protected EdirEventHandler edir_event;

		protected LdapConnection mConnection;

		protected MonitorEventRequest mRequestOperation;

		protected LdapResponseQueue mQueue;

		public event EdirEventHandler EdirEvent
		{
			add
			{
				edir_event = (EdirEventHandler)Delegate.Combine(edir_event, value);
				ListenerAdded();
			}
			remove
			{
				edir_event = (EdirEventHandler)Delegate.Remove(edir_event, value);
				ListenerRemoved();
			}
		}

		protected override int GetListeners()
		{
			int result = 0;
			if (edir_event != null)
			{
				result = edir_event.GetInvocationList().Length;
			}
			return result;
		}

		public EdirEventSource(EdirEventSpecifier[] specifier, LdapConnection conn)
		{
			if (specifier == null || conn == null)
			{
				throw new ArgumentException("Null argument specified");
			}
			mRequestOperation = new MonitorEventRequest(specifier);
			mConnection = conn;
		}

		protected override void StartSearchAndPolling()
		{
			mQueue = mConnection.ExtendedOperation(mRequestOperation, null, null);
			int[] messageIDs = mQueue.MessageIDs;
			if (messageIDs.Length != 1)
			{
				throw new LdapException(null, 82, "Unable to Obtain Message Id");
			}
			StartEventPolling(mQueue, mConnection, messageIDs[0]);
		}

		protected override void StopSearchAndPolling()
		{
			mConnection.Abandon(mQueue);
			StopEventPolling();
		}

		protected override bool NotifyEventListeners(LdapMessage sourceMessage, EventClassifiers aClassification, int nType)
		{
			bool result = false;
			if (edir_event != null && sourceMessage != null && sourceMessage.Type == 25 && sourceMessage is EdirEventIntermediateResponse)
			{
				edir_event(this, new EdirEventArgs(sourceMessage, EventClassifiers.CLASSIFICATION_EDIR_EVENT));
				result = true;
			}
			return result;
		}
	}
}

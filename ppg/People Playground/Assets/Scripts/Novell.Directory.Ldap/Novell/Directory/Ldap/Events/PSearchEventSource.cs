using System;
using Novell.Directory.Ldap.Controls;

namespace Novell.Directory.Ldap.Events
{
	public class PSearchEventSource : LdapEventSource
	{
		public delegate void SearchResultEventHandler(object source, SearchResultEventArgs objArgs);

		public delegate void SearchReferralEventHandler(object source, SearchReferralEventArgs objArgs);

		protected SearchResultEventHandler search_result_event;

		protected SearchReferralEventHandler search_referral_event;

		protected LdapConnection mConnection;

		protected string mSearchBase;

		protected int mScope;

		protected string[] mAttrs;

		protected string mFilter;

		protected bool mTypesOnly;

		protected LdapSearchConstraints mSearchConstraints;

		protected LdapEventType mEventChangeType;

		protected LdapSearchQueue mQueue;

		public event SearchResultEventHandler SearchResultEvent
		{
			add
			{
				search_result_event = (SearchResultEventHandler)Delegate.Combine(search_result_event, value);
				ListenerAdded();
			}
			remove
			{
				search_result_event = (SearchResultEventHandler)Delegate.Remove(search_result_event, value);
				ListenerRemoved();
			}
		}

		public event SearchReferralEventHandler SearchReferralEvent
		{
			add
			{
				search_referral_event = (SearchReferralEventHandler)Delegate.Combine(search_referral_event, value);
				ListenerAdded();
			}
			remove
			{
				search_referral_event = (SearchReferralEventHandler)Delegate.Remove(search_referral_event, value);
				ListenerRemoved();
			}
		}

		protected override int GetListeners()
		{
			int num = 0;
			if (search_result_event != null)
			{
				num = search_result_event.GetInvocationList().Length;
			}
			if (search_referral_event != null)
			{
				num += search_referral_event.GetInvocationList().Length;
			}
			return num;
		}

		public PSearchEventSource(LdapConnection conn, string searchBase, int scope, string filter, string[] attrs, bool typesOnly, LdapSearchConstraints constraints, LdapEventType eventchangetype, bool changeonly)
		{
			if (conn == null || searchBase == null || filter == null || attrs == null)
			{
				throw new ArgumentException("Null argument specified");
			}
			mConnection = conn;
			mSearchBase = searchBase;
			mScope = scope;
			mFilter = filter;
			mAttrs = attrs;
			mTypesOnly = typesOnly;
			mEventChangeType = eventchangetype;
			if (constraints == null)
			{
				mSearchConstraints = new LdapSearchConstraints();
			}
			else
			{
				mSearchConstraints = constraints;
			}
			LdapPersistSearchControl controls = new LdapPersistSearchControl((int)eventchangetype, changeonly, returnControls: true, isCritical: true);
			mSearchConstraints.setControls(controls);
		}

		protected override void StartSearchAndPolling()
		{
			mQueue = mConnection.Search(mSearchBase, mScope, mFilter, mAttrs, mTypesOnly, null, mSearchConstraints);
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
			if (sourceMessage == null)
			{
				return result;
			}
			switch (sourceMessage.Type)
			{
			case 19:
				if (search_referral_event != null)
				{
					search_referral_event(this, new SearchReferralEventArgs(sourceMessage, aClassification, (LdapEventType)nType));
					result = true;
				}
				break;
			case 4:
			{
				if (search_result_event == null)
				{
					break;
				}
				LdapEventType aType = LdapEventType.TYPE_UNKNOWN;
				LdapControl[] controls = sourceMessage.Controls;
				foreach (LdapControl ldapControl in controls)
				{
					if (ldapControl is LdapEntryChangeControl)
					{
						aType = (LdapEventType)((LdapEntryChangeControl)ldapControl).ChangeType;
					}
				}
				search_result_event(this, new SearchResultEventArgs(sourceMessage, aClassification, aType));
				result = true;
				break;
			}
			case 5:
				NotifyDirectoryListeners(new LdapEventArgs(sourceMessage, EventClassifiers.CLASSIFICATION_LDAP_PSEARCH, LdapEventType.LDAP_PSEARCH_ANY));
				result = true;
				break;
			}
			return result;
		}
	}
}

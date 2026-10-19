using System;
using System.Collections;
using Novell.Directory.Ldap.Utilclass;

namespace Novell.Directory.Ldap
{
	public class LdapSearchResults
	{
		private ArrayList entries;

		private int entryCount;

		private int entryIndex;

		private ArrayList references;

		private int referenceCount;

		private int referenceIndex;

		private int batchSize;

		private bool completed;

		private LdapControl[] controls;

		private LdapSearchQueue queue;

		private static object nameLock;

		private static int resultsNum;

		private string name;

		private LdapConnection conn;

		private LdapSearchConstraints cons;

		private ArrayList referralConn;

		public virtual int Count
		{
			get
			{
				int count = queue.MessageAgent.Count;
				return entryCount - entryIndex + referenceCount - referenceIndex + count;
			}
		}

		public virtual LdapControl[] ResponseControls => controls;

		private bool BatchOfResults
		{
			get
			{
				int num = 0;
				while (num < batchSize)
				{
					try
					{
						LdapMessage response;
						if ((response = queue.getResponse()) != null)
						{
							LdapControl[] array = response.Controls;
							if (array != null)
							{
								controls = array;
							}
							if (response is LdapSearchResult)
							{
								object entry = ((LdapSearchResult)response).Entry;
								entries.Add(entry);
								num++;
								entryCount++;
								continue;
							}
							if (response is LdapSearchResultReference)
							{
								string[] referrals = ((LdapSearchResultReference)response).Referrals;
								if (!cons.ReferralFollowing)
								{
									references.Add(referrals);
									referenceCount++;
								}
								continue;
							}
							LdapResponse ldapResponse = (LdapResponse)response;
							int num2 = ldapResponse.ResultCode;
							if (ldapResponse.hasException())
							{
								num2 = 91;
							}
							if ((num2 != 10 || !cons.ReferralFollowing) && num2 != 0)
							{
								entries.Add(ldapResponse);
								entryCount++;
							}
							if (queue.MessageIDs.Length != 0)
							{
								continue;
							}
							return true;
						}
						LdapException value = new LdapException(null, 85, null);
						entries.Add(value);
						break;
					}
					catch (LdapException value2)
					{
						entries.Add(value2);
					}
				}
				return false;
			}
		}

		internal LdapSearchResults(LdapConnection conn, LdapSearchQueue queue, LdapSearchConstraints cons)
		{
			this.conn = conn;
			this.cons = cons;
			int num = cons.BatchSize;
			entries = new ArrayList((num == 0) ? 64 : num);
			entryCount = 0;
			entryIndex = 0;
			references = new ArrayList(5);
			referenceCount = 0;
			referenceIndex = 0;
			this.queue = queue;
			batchSize = ((num == 0) ? int.MaxValue : num);
		}

		public virtual bool hasMore()
		{
			bool result = false;
			if (entryIndex < entryCount || referenceIndex < referenceCount)
			{
				result = true;
			}
			else if (!completed)
			{
				resetVectors();
				result = entryIndex < entryCount || referenceIndex < referenceCount;
			}
			return result;
		}

		private void resetVectors()
		{
			if (!completed)
			{
				if (referenceIndex != 0 && referenceIndex >= referenceCount)
				{
					SupportClass.SetSize(references, 0);
					referenceCount = 0;
					referenceIndex = 0;
				}
				if (entryIndex != 0 && entryIndex >= entryCount)
				{
					SupportClass.SetSize(entries, 0);
					entryCount = 0;
					entryIndex = 0;
				}
				if (referenceIndex == 0 && referenceCount == 0 && entryIndex == 0 && entryCount == 0)
				{
					completed = BatchOfResults;
				}
			}
		}

		public virtual LdapEntry next()
		{
			if (completed && entryIndex >= entryCount && referenceIndex >= referenceCount)
			{
				throw new ArgumentOutOfRangeException("LdapSearchResults.next() no more results");
			}
			resetVectors();
			object obj = null;
			if (referenceIndex < referenceCount)
			{
				string[] referrals = (string[])references[referenceIndex++];
				LdapReferralException ex = new LdapReferralException("REFERENCE_NOFOLLOW");
				ex.setReferrals(referrals);
				throw ex;
			}
			if (entryIndex < entryCount)
			{
				obj = entries[entryIndex++];
				if (obj is LdapResponse)
				{
					if (((LdapResponse)obj).hasException())
					{
						LdapResponse ldapResponse = (LdapResponse)obj;
						ReferralInfo activeReferral = ldapResponse.ActiveReferral;
						if (activeReferral != null)
						{
							LdapReferralException ex2 = new LdapReferralException("REFERENCE_ERROR", ldapResponse.Exception);
							ex2.setReferrals(activeReferral.ReferralList);
							ex2.FailedReferral = activeReferral.ReferralUrl.ToString();
							throw ex2;
						}
					}
					((LdapResponse)obj).chkResultCode();
				}
				else if (obj is LdapException)
				{
					throw (LdapException)obj;
				}
				return (LdapEntry)obj;
			}
			throw new LdapException("REFERRAL_LOCAL", new object[1] { "next" }, 82, null);
		}

		internal virtual void Abandon()
		{
			queue.MessageAgent.AbandonAll();
			resetVectors();
			completed = true;
		}

		static LdapSearchResults()
		{
			nameLock = new object();
		}
	}
}

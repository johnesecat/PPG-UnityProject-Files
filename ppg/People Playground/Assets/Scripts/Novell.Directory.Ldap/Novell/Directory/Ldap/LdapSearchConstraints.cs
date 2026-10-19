using System.Collections;

namespace Novell.Directory.Ldap
{
	public class LdapSearchConstraints : LdapConstraints
	{
		private int dereference;

		private int serverTimeLimit;

		private int maxResults = 1000;

		private int batchSize = 1;

		private static object nameLock;

		private static int lSConsNum;

		private string name;

		public const int DEREF_NEVER = 0;

		public const int DEREF_SEARCHING = 1;

		public const int DEREF_FINDING = 2;

		public const int DEREF_ALWAYS = 3;

		public virtual int BatchSize
		{
			get
			{
				return batchSize;
			}
			set
			{
				batchSize = value;
			}
		}

		public virtual int Dereference
		{
			get
			{
				return dereference;
			}
			set
			{
				dereference = value;
			}
		}

		public virtual int MaxResults
		{
			get
			{
				return maxResults;
			}
			set
			{
				maxResults = value;
			}
		}

		public virtual int ServerTimeLimit
		{
			get
			{
				return serverTimeLimit;
			}
			set
			{
				serverTimeLimit = value;
			}
		}

		private void InitBlock()
		{
			dereference = 0;
		}

		public LdapSearchConstraints()
		{
			InitBlock();
		}

		public LdapSearchConstraints(LdapConstraints cons)
			: base(cons.TimeLimit, cons.ReferralFollowing, cons.getReferralHandler(), cons.HopLimit)
		{
			InitBlock();
			LdapControl[] array = cons.getControls();
			if (array != null)
			{
				LdapControl[] array2 = new LdapControl[array.Length];
				array.CopyTo(array2, 0);
				base.setControls(array2);
			}
			Hashtable hashtable = cons.Properties;
			if (hashtable != null)
			{
				base.Properties = (Hashtable)hashtable.Clone();
			}
			if (cons is LdapSearchConstraints)
			{
				LdapSearchConstraints ldapSearchConstraints = (LdapSearchConstraints)cons;
				serverTimeLimit = ldapSearchConstraints.ServerTimeLimit;
				dereference = ldapSearchConstraints.Dereference;
				maxResults = ldapSearchConstraints.MaxResults;
				batchSize = ldapSearchConstraints.BatchSize;
			}
		}

		public LdapSearchConstraints(int msLimit, int serverTimeLimit, int dereference, int maxResults, bool doReferrals, int batchSize, LdapReferralHandler handler, int hop_limit)
			: base(msLimit, doReferrals, handler, hop_limit)
		{
			InitBlock();
			this.serverTimeLimit = serverTimeLimit;
			this.dereference = dereference;
			this.maxResults = maxResults;
			this.batchSize = batchSize;
		}

		static LdapSearchConstraints()
		{
			nameLock = new object();
		}
	}
}

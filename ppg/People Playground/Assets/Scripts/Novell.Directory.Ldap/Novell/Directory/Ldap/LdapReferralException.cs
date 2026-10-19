using System;
using Novell.Directory.Ldap.Utilclass;

namespace Novell.Directory.Ldap
{
	public class LdapReferralException : LdapException
	{
		private string failedReferral;

		private string[] referrals;

		public virtual string FailedReferral
		{
			get
			{
				return failedReferral;
			}
			set
			{
				failedReferral = value;
			}
		}

		public LdapReferralException()
		{
		}

		public LdapReferralException(string message)
			: base(message, 10, null)
		{
		}

		public LdapReferralException(string message, object[] arguments)
			: base(message, arguments, 10, null)
		{
		}

		public LdapReferralException(string message, Exception rootException)
			: base(message, 10, null, rootException)
		{
		}

		public LdapReferralException(string message, object[] arguments, Exception rootException)
			: base(message, arguments, 10, null, rootException)
		{
		}

		public LdapReferralException(string message, int resultCode, string serverMessage)
			: base(message, resultCode, serverMessage)
		{
		}

		public LdapReferralException(string message, object[] arguments, int resultCode, string serverMessage)
			: base(message, arguments, resultCode, serverMessage)
		{
		}

		public LdapReferralException(string message, int resultCode, string serverMessage, Exception rootException)
			: base(message, resultCode, serverMessage, rootException)
		{
		}

		public LdapReferralException(string message, object[] arguments, int resultCode, string serverMessage, Exception rootException)
			: base(message, arguments, resultCode, serverMessage, rootException)
		{
		}

		public virtual string[] getReferrals()
		{
			return referrals;
		}

		internal virtual void setReferrals(string[] urls)
		{
			referrals = urls;
		}

		public override string ToString()
		{
			string text = getExceptionString("LdapReferralException");
			if (failedReferral != null)
			{
				string text2 = ResourcesHandler.getMessage("FAILED_REFERRAL", new object[2] { "LdapReferralException", failedReferral });
				if (text2.ToUpper().Equals("SERVER_MSG".ToUpper()))
				{
					text2 = "LdapReferralException: Failed Referral: " + failedReferral;
				}
				text = text + "\n" + text2;
			}
			if (referrals != null)
			{
				for (int i = 0; i < referrals.Length; i++)
				{
					string text2 = ResourcesHandler.getMessage("REFERRAL_ITEM", new object[2]
					{
						"LdapReferralException",
						referrals[i]
					});
					if (text2.ToUpper().Equals("SERVER_MSG".ToUpper()))
					{
						text2 = "LdapReferralException: Referral: " + referrals[i];
					}
					text = text + "\n" + text2;
				}
			}
			return text;
		}
	}
}

namespace Novell.Directory.Ldap.Utilclass
{
	public class ReferralInfo
	{
		private LdapConnection conn;

		private LdapUrl referralUrl;

		private string[] referralList;

		public virtual LdapUrl ReferralUrl => referralUrl;

		public virtual LdapConnection ReferralConnection => conn;

		public virtual string[] ReferralList => referralList;

		public ReferralInfo(LdapConnection lc, string[] refList, LdapUrl refUrl)
		{
			conn = lc;
			referralUrl = refUrl;
			referralList = refList;
		}
	}
}

namespace Novell.Directory.Ldap.Controls
{
	public class LdapSortKey
	{
		private string key;

		private bool reverse;

		private string matchRule;

		public virtual string Key => key;

		public virtual bool Reverse => reverse;

		public virtual string MatchRule => matchRule;

		public LdapSortKey(string keyDescription)
		{
			matchRule = null;
			reverse = false;
			string text = keyDescription;
			if (text[0] == '-')
			{
				text = text.Substring(1);
				reverse = true;
			}
			int num = text.IndexOf(":");
			if (num != -1)
			{
				key = text.Substring(0, num);
				matchRule = text.Substring(num + 1);
			}
			else
			{
				key = text;
			}
		}

		public LdapSortKey(string key, bool reverse)
			: this(key, reverse, null)
		{
		}

		public LdapSortKey(string key, bool reverse, string matchRule)
		{
			this.key = key;
			this.reverse = reverse;
			this.matchRule = matchRule;
		}
	}
}

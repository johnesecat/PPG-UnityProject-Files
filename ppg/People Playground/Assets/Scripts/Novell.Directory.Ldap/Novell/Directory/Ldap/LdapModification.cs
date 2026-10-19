namespace Novell.Directory.Ldap
{
	public class LdapModification
	{
		private int op;

		private LdapAttribute attr;

		public const int ADD = 0;

		public const int DELETE = 1;

		public const int REPLACE = 2;

		public virtual LdapAttribute Attribute => attr;

		public virtual int Op => op;

		public LdapModification(int op, LdapAttribute attr)
		{
			this.op = op;
			this.attr = attr;
		}
	}
}

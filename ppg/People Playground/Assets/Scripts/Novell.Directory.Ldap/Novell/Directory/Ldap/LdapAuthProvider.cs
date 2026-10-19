using System;

namespace Novell.Directory.Ldap
{
	public class LdapAuthProvider
	{
		private string dn;

		private sbyte[] password;

		public virtual string DN => dn;

		[CLSCompliant(false)]
		public virtual sbyte[] Password => password;

		[CLSCompliant(false)]
		public LdapAuthProvider(string dn, sbyte[] password)
		{
			this.dn = dn;
			this.password = password;
		}
	}
}

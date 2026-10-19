using Novell.Directory.Ldap.Asn1;
using Novell.Directory.Ldap.Rfc2251;

namespace Novell.Directory.Ldap
{
	public class LdapSearchResultReference : LdapMessage
	{
		private string[] srefs;

		private static object nameLock;

		private static int refNum;

		private string name;

		public virtual string[] Referrals
		{
			get
			{
				Asn1Object[] array = ((RfcSearchResultReference)message.Response).toArray();
				srefs = new string[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					srefs[i] = ((Asn1OctetString)array[i]).stringValue();
				}
				return srefs;
			}
		}

		internal LdapSearchResultReference(RfcLdapMessage message)
			: base(message)
		{
		}

		static LdapSearchResultReference()
		{
			nameLock = new object();
		}
	}
}

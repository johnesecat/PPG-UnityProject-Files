using Novell.Directory.Ldap.Rfc2251;

namespace Novell.Directory.Ldap
{
	public class LdapDeleteRequest : LdapMessage
	{
		public virtual string DN => Asn1Object.RequestDN;

		public LdapDeleteRequest(string dn, LdapControl[] cont)
			: base(10, new RfcDelRequest(dn), cont)
		{
		}

		public override string ToString()
		{
			return Asn1Object.ToString();
		}
	}
}

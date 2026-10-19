using Novell.Directory.Ldap.Asn1;
using Novell.Directory.Ldap.Rfc2251;

namespace Novell.Directory.Ldap
{
	public class LdapModifyDNRequest : LdapMessage
	{
		public virtual string DN => Asn1Object.RequestDN;

		public virtual string NewRDN => ((RfcRelativeLdapDN)((RfcModifyDNRequest)Asn1Object.getRequest()).toArray()[1]).stringValue();

		public virtual bool DeleteOldRDN => ((Asn1Boolean)((RfcModifyDNRequest)Asn1Object.getRequest()).toArray()[2]).booleanValue();

		public virtual string ParentDN
		{
			get
			{
				RfcModifyDNRequest rfcModifyDNRequest = (RfcModifyDNRequest)Asn1Object.getRequest();
				Asn1Object[] array = rfcModifyDNRequest.toArray();
				if (array.Length < 4 || array[3] == null)
				{
					return null;
				}
				return ((RfcLdapDN)rfcModifyDNRequest.toArray()[3]).stringValue();
			}
		}

		public LdapModifyDNRequest(string dn, string newRdn, string newParentdn, bool deleteOldRdn, LdapControl[] cont)
			: base(12, new RfcModifyDNRequest(new RfcLdapDN(dn), new RfcRelativeLdapDN(newRdn), new Asn1Boolean(deleteOldRdn), (newParentdn != null) ? new RfcLdapSuperDN(newParentdn) : null), cont)
		{
		}

		public override string ToString()
		{
			return Asn1Object.ToString();
		}
	}
}

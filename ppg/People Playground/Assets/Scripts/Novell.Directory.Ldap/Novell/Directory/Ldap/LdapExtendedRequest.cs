using Novell.Directory.Ldap.Asn1;
using Novell.Directory.Ldap.Rfc2251;

namespace Novell.Directory.Ldap
{
	public class LdapExtendedRequest : LdapMessage
	{
		public virtual LdapExtendedOperation ExtendedOperation
		{
			get
			{
				RfcExtendedRequest rfcExtendedRequest = (RfcExtendedRequest)Asn1Object.get_Renamed(1);
				string oid = ((RfcLdapOID)((Asn1Tagged)rfcExtendedRequest.get_Renamed(0)).taggedValue()).stringValue();
				sbyte[] vals = null;
				if (rfcExtendedRequest.size() >= 2)
				{
					vals = ((Asn1OctetString)((Asn1Tagged)rfcExtendedRequest.get_Renamed(1)).taggedValue()).byteValue();
				}
				return new LdapExtendedOperation(oid, vals);
			}
		}

		public LdapExtendedRequest(LdapExtendedOperation op, LdapControl[] cont)
			: base(23, new RfcExtendedRequest(new RfcLdapOID(op.getID()), (op.getValue() != null) ? new Asn1OctetString(op.getValue()) : null), cont)
		{
		}
	}
}

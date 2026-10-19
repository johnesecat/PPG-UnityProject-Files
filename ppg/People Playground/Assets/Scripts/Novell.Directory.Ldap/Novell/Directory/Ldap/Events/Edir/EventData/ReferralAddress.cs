using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Events.Edir.EventData
{
	public class ReferralAddress
	{
		protected int address_type;

		protected string strAddress;

		public int AddressType => address_type;

		public string Address => strAddress;

		public ReferralAddress(Asn1Sequence dseObject)
		{
			address_type = ((Asn1Integer)dseObject.get_Renamed(0)).intValue();
			strAddress = ((Asn1OctetString)dseObject.get_Renamed(1)).stringValue();
		}
	}
}

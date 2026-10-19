using System.Collections;
using System.Text;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Events.Edir.EventData
{
	public class SecurityEquivalenceEventData : BaseEdirEventData
	{
		protected string strEntryDN;

		protected int retry_count;

		protected string strValueDN;

		protected int referral_count;

		protected ArrayList referral_list;

		public string EntryDN => strEntryDN;

		public int RetryCount => retry_count;

		public string ValueDN => strValueDN;

		public int ReferralCount => referral_count;

		public ArrayList ReferralList => referral_list;

		public SecurityEquivalenceEventData(EdirEventDataType eventDataType, Asn1Object message)
			: base(eventDataType, message)
		{
			int[] len = new int[1];
			strEntryDN = ((Asn1OctetString)decoder.decode(decodedData, len)).stringValue();
			retry_count = ((Asn1Integer)decoder.decode(decodedData, len)).intValue();
			strValueDN = ((Asn1OctetString)decoder.decode(decodedData, len)).stringValue();
			Asn1Sequence asn1Sequence = (Asn1Sequence)decoder.decode(decodedData, len);
			referral_count = ((Asn1Integer)asn1Sequence.get_Renamed(0)).intValue();
			referral_list = new ArrayList();
			if (referral_count > 0)
			{
				Asn1Sequence asn1Sequence2 = (Asn1Sequence)asn1Sequence.get_Renamed(1);
				for (int i = 0; i < referral_count; i++)
				{
					referral_list.Add(new ReferralAddress((Asn1Sequence)asn1Sequence2.get_Renamed(i)));
				}
			}
			DataInitDone();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[SecurityEquivalenceEventData");
			stringBuilder.AppendFormat("(EntryDN={0})", strEntryDN);
			stringBuilder.AppendFormat("(RetryCount={0})", retry_count);
			stringBuilder.AppendFormat("(valueDN={0})", strValueDN);
			stringBuilder.AppendFormat("(referralCount={0})", referral_count);
			stringBuilder.AppendFormat("(Referral Lists={0})", referral_list);
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}
	}
}

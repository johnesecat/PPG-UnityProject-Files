using System.Text;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Events.Edir.EventData
{
	public class EntryEventData : BaseEdirEventData
	{
		protected string strPerpetratorDN;

		protected string strEntry;

		protected string strNewDN;

		protected string strClassId;

		protected int nVerb;

		protected int nFlags;

		protected DSETimeStamp timeStampObj;

		public string PerpetratorDN => strPerpetratorDN;

		public string Entry => strEntry;

		public string NewDN => strNewDN;

		public string ClassId => strClassId;

		public int Verb => nVerb;

		public int Flags => nFlags;

		public DSETimeStamp TimeStamp => timeStampObj;

		public EntryEventData(EdirEventDataType eventDataType, Asn1Object message)
			: base(eventDataType, message)
		{
			int[] len = new int[1];
			strPerpetratorDN = ((Asn1OctetString)decoder.decode(decodedData, len)).stringValue();
			strEntry = ((Asn1OctetString)decoder.decode(decodedData, len)).stringValue();
			strClassId = ((Asn1OctetString)decoder.decode(decodedData, len)).stringValue();
			timeStampObj = new DSETimeStamp((Asn1Sequence)decoder.decode(decodedData, len));
			nVerb = ((Asn1Integer)decoder.decode(decodedData, len)).intValue();
			nFlags = ((Asn1Integer)decoder.decode(decodedData, len)).intValue();
			strNewDN = ((Asn1OctetString)decoder.decode(decodedData, len)).stringValue();
			DataInitDone();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("EntryEventData[");
			stringBuilder.AppendFormat("(Entry={0})", strEntry);
			stringBuilder.AppendFormat("(Prepetrator={0})", strPerpetratorDN);
			stringBuilder.AppendFormat("(ClassId={0})", strClassId);
			stringBuilder.AppendFormat("(Verb={0})", nVerb);
			stringBuilder.AppendFormat("(Flags={0})", nFlags);
			stringBuilder.AppendFormat("(NewDN={0})", strNewDN);
			stringBuilder.AppendFormat("(TimeStamp={0})", timeStampObj);
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}
	}
}

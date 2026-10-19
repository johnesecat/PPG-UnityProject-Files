using System.Text;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Events.Edir.EventData
{
	public class NetworkAddressEventData : BaseEdirEventData
	{
		protected int nType;

		protected string strData;

		public int ValueType => nType;

		public string Data => strData;

		public NetworkAddressEventData(EdirEventDataType eventDataType, Asn1Object message)
			: base(eventDataType, message)
		{
			int[] len = new int[1];
			nType = ((Asn1Integer)decoder.decode(decodedData, len)).intValue();
			strData = ((Asn1OctetString)decoder.decode(decodedData, len)).stringValue();
			DataInitDone();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[NetworkAddress");
			stringBuilder.AppendFormat("(type={0})", nType);
			stringBuilder.AppendFormat("(Data={0})", strData);
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}
	}
}

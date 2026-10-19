using System.Text;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Events.Edir.EventData
{
	public class ModuleStateEventData : BaseEdirEventData
	{
		protected string strConnectionDN;

		protected int nFlags;

		protected string strName;

		protected string strDescription;

		protected string strSource;

		public string ConnectionDN => strConnectionDN;

		public int Flags => nFlags;

		public string Name => strName;

		public string Description => strDescription;

		public string Source => strSource;

		public ModuleStateEventData(EdirEventDataType eventDataType, Asn1Object message)
			: base(eventDataType, message)
		{
			int[] len = new int[1];
			strConnectionDN = ((Asn1OctetString)decoder.decode(decodedData, len)).stringValue();
			nFlags = ((Asn1Integer)decoder.decode(decodedData, len)).intValue();
			strName = ((Asn1OctetString)decoder.decode(decodedData, len)).stringValue();
			strDescription = ((Asn1OctetString)decoder.decode(decodedData, len)).stringValue();
			strSource = ((Asn1OctetString)decoder.decode(decodedData, len)).stringValue();
			DataInitDone();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[ModuleStateEvent");
			stringBuilder.AppendFormat("(connectionDN={0})", strConnectionDN);
			stringBuilder.AppendFormat("(flags={0})", nFlags);
			stringBuilder.AppendFormat("(Name={0})", strName);
			stringBuilder.AppendFormat("(Description={0})", strDescription);
			stringBuilder.AppendFormat("(Source={0})", strSource);
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}
	}
}

using System.Text;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Events.Edir.EventData
{
	public class ConnectionStateEventData : BaseEdirEventData
	{
		protected string strConnectionDN;

		protected int old_flags;

		protected int new_flags;

		protected string source_module;

		public string ConnectionDN => strConnectionDN;

		public int OldFlags => old_flags;

		public int NewFlags => new_flags;

		public string SourceModule => source_module;

		public ConnectionStateEventData(EdirEventDataType eventDataType, Asn1Object message)
			: base(eventDataType, message)
		{
			int[] len = new int[1];
			strConnectionDN = ((Asn1OctetString)decoder.decode(decodedData, len)).stringValue();
			old_flags = ((Asn1Integer)decoder.decode(decodedData, len)).intValue();
			new_flags = ((Asn1Integer)decoder.decode(decodedData, len)).intValue();
			source_module = ((Asn1OctetString)decoder.decode(decodedData, len)).stringValue();
			DataInitDone();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[ConnectionStateEvent");
			stringBuilder.AppendFormat("(ConnectionDN={0})", strConnectionDN);
			stringBuilder.AppendFormat("(oldFlags={0})", old_flags);
			stringBuilder.AppendFormat("(newFlags={0})", new_flags);
			stringBuilder.AppendFormat("(SourceModule={0})", source_module);
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}
	}
}

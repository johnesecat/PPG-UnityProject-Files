using System.IO;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Events.Edir.EventData
{
	public class BaseEdirEventData
	{
		protected MemoryStream decodedData;

		protected LBERDecoder decoder;

		protected EdirEventDataType event_data_type;

		public EdirEventDataType EventDataType => event_data_type;

		public BaseEdirEventData(EdirEventDataType eventDataType, Asn1Object message)
		{
			event_data_type = eventDataType;
			byte[] buffer = SupportClass.ToByteArray(((Asn1OctetString)message).byteValue());
			decodedData = new MemoryStream(buffer);
			decoder = new LBERDecoder();
		}

		protected void DataInitDone()
		{
			decodedData = null;
			decoder = null;
		}
	}
}

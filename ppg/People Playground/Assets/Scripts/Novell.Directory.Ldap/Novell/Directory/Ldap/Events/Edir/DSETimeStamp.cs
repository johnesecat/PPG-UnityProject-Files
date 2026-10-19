using System.Text;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Events.Edir
{
	public class DSETimeStamp
	{
		protected int nSeconds;

		protected int replica_number;

		protected int nEvent;

		public int Seconds => nSeconds;

		public int ReplicaNumber => replica_number;

		public int Event => nEvent;

		public DSETimeStamp(Asn1Sequence dseObject)
		{
			nSeconds = ((Asn1Integer)dseObject.get_Renamed(0)).intValue();
			replica_number = ((Asn1Integer)dseObject.get_Renamed(1)).intValue();
			nEvent = ((Asn1Integer)dseObject.get_Renamed(2)).intValue();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("[TimeStamp (seconds={0})", nSeconds);
			stringBuilder.AppendFormat("(replicaNumber={0})", replica_number);
			stringBuilder.AppendFormat("(event={0})", nEvent);
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}
	}
}

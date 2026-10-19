using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	internal class RfcMessageID : Asn1Integer
	{
		private static int messageID;

		private static object lock_Renamed;

		private static int MessageID
		{
			get
			{
				lock (lock_Renamed)
				{
					return (messageID >= int.MaxValue) ? (messageID = 1) : (++messageID);
				}
			}
		}

		protected internal RfcMessageID()
			: base(MessageID)
		{
		}

		protected internal RfcMessageID(int i)
			: base(i)
		{
		}

		static RfcMessageID()
		{
			lock_Renamed = new object();
		}
	}
}

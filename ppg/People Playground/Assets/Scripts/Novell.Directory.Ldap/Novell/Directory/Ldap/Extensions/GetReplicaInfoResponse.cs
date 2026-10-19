using System.IO;
using Novell.Directory.Ldap.Asn1;
using Novell.Directory.Ldap.Rfc2251;

namespace Novell.Directory.Ldap.Extensions
{
	public class GetReplicaInfoResponse : LdapExtendedResponse
	{
		private int partitionID;

		private int replicaState;

		private int modificationTime;

		private int purgeTime;

		private int localPartitionID;

		private string partitionDN;

		private int replicaType;

		private int flags;

		public GetReplicaInfoResponse(RfcLdapMessage rfcMessage)
			: base(rfcMessage)
		{
			if (ResultCode == 0)
			{
				sbyte[] value = Value;
				if (value == null)
				{
					throw new IOException("No returned value");
				}
				LBERDecoder obj = new LBERDecoder() ?? throw new IOException("Decoding error");
				MemoryStream in_Renamed = new MemoryStream(SupportClass.ToByteArray(value));
				Asn1Integer asn1Integer = (Asn1Integer)obj.decode(in_Renamed);
				if (asn1Integer == null)
				{
					throw new IOException("Decoding error");
				}
				partitionID = asn1Integer.intValue();
				Asn1Integer asn1Integer2 = (Asn1Integer)obj.decode(in_Renamed);
				if (asn1Integer2 == null)
				{
					throw new IOException("Decoding error");
				}
				replicaState = asn1Integer2.intValue();
				Asn1Integer asn1Integer3 = (Asn1Integer)obj.decode(in_Renamed);
				if (asn1Integer3 == null)
				{
					throw new IOException("Decoding error");
				}
				modificationTime = asn1Integer3.intValue();
				Asn1Integer asn1Integer4 = (Asn1Integer)obj.decode(in_Renamed);
				if (asn1Integer4 == null)
				{
					throw new IOException("Decoding error");
				}
				purgeTime = asn1Integer4.intValue();
				Asn1Integer asn1Integer5 = (Asn1Integer)obj.decode(in_Renamed);
				if (asn1Integer5 == null)
				{
					throw new IOException("Decoding error");
				}
				localPartitionID = asn1Integer5.intValue();
				Asn1OctetString asn1OctetString = (Asn1OctetString)obj.decode(in_Renamed);
				if (asn1OctetString == null)
				{
					throw new IOException("Decoding error");
				}
				partitionDN = asn1OctetString.stringValue();
				if (partitionDN == null)
				{
					throw new IOException("Decoding error");
				}
				Asn1Integer asn1Integer6 = (Asn1Integer)obj.decode(in_Renamed);
				if (asn1Integer6 == null)
				{
					throw new IOException("Decoding error");
				}
				replicaType = asn1Integer6.intValue();
				Asn1Integer asn1Integer7 = (Asn1Integer)obj.decode(in_Renamed);
				if (asn1Integer7 == null)
				{
					throw new IOException("Decoding error");
				}
				flags = asn1Integer7.intValue();
			}
			else
			{
				partitionID = 0;
				replicaState = 0;
				modificationTime = 0;
				purgeTime = 0;
				localPartitionID = 0;
				partitionDN = "";
				replicaType = 0;
				flags = 0;
			}
		}

		public virtual int getpartitionID()
		{
			return partitionID;
		}

		public virtual int getreplicaState()
		{
			return replicaState;
		}

		public virtual int getmodificationTime()
		{
			return modificationTime;
		}

		public virtual int getpurgeTime()
		{
			return purgeTime;
		}

		public virtual int getlocalPartitionID()
		{
			return localPartitionID;
		}

		public virtual string getpartitionDN()
		{
			return partitionDN;
		}

		public virtual int getreplicaType()
		{
			return replicaType;
		}

		public virtual int getflags()
		{
			return flags;
		}
	}
}

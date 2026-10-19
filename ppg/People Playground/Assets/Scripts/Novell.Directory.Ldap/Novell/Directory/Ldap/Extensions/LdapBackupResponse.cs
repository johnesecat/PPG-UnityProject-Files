using System;
using System.IO;
using System.Text;
using Novell.Directory.Ldap.Asn1;
using Novell.Directory.Ldap.Rfc2251;

namespace Novell.Directory.Ldap.Extensions
{
	public class LdapBackupResponse : LdapExtendedResponse
	{
		private int bufferLength;

		private string stateInfo;

		private string chunkSizesString;

		private byte[] returnedBuffer;

		public LdapBackupResponse(RfcLdapMessage rfcMessage)
			: base(rfcMessage)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int[] array = null;
			if (ID == null || !ID.Equals("2.16.840.1.113719.1.27.100.97"))
			{
				throw new IOException("LDAP Extended Operation not supported");
			}
			if (ResultCode == 0)
			{
				byte[] array2 = SupportClass.ToByteArray(Value);
				if (array2 == null)
				{
					throw new Exception("LDAP Operations error. No returned value.");
				}
				LBERDecoder obj = new LBERDecoder() ?? throw new Exception("Decoding error");
				MemoryStream in_Renamed = new MemoryStream(array2);
				Asn1Integer asn1Integer = (Asn1Integer)obj.decode(in_Renamed);
				if (asn1Integer == null)
				{
					throw new IOException("Decoding error");
				}
				bufferLength = asn1Integer.intValue();
				num = (((Asn1Integer)obj.decode(in_Renamed)) ?? throw new IOException("Decoding error")).intValue();
				stateInfo = string.Concat(arg2: (((Asn1Integer)obj.decode(in_Renamed)) ?? throw new IOException("Decoding error")).intValue(), arg0: num, arg1: "+");
				Asn1OctetString asn1OctetString = (Asn1OctetString)obj.decode(in_Renamed);
				if (asn1OctetString == null)
				{
					throw new IOException("Decoding error");
				}
				returnedBuffer = SupportClass.ToByteArray(asn1OctetString.byteValue());
				Asn1Sequence obj3 = ((Asn1Sequence)obj.decode(in_Renamed)) ?? throw new IOException("Decoding error");
				num3 = ((Asn1Integer)obj3.get_Renamed(0)).intValue();
				array = new int[num3];
				Asn1Set asn1Set = (Asn1Set)obj3.get_Renamed(1);
				for (int i = 0; i < num3; i++)
				{
					Asn1Sequence asn1Sequence = (Asn1Sequence)asn1Set.get_Renamed(i);
					array[i] = ((Asn1Integer)asn1Sequence.get_Renamed(0)).intValue();
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(num3);
				stringBuilder.Append(";");
				int j;
				for (j = 0; j < num3 - 1; j++)
				{
					stringBuilder.Append(array[j]);
					stringBuilder.Append(";");
				}
				stringBuilder.Append(array[j]);
				chunkSizesString = stringBuilder.ToString();
			}
			else
			{
				bufferLength = 0;
				stateInfo = null;
				chunkSizesString = null;
				returnedBuffer = null;
			}
		}

		public int getBufferLength()
		{
			return bufferLength;
		}

		public string getStatusInfo()
		{
			return stateInfo;
		}

		public string getChunkSizesString()
		{
			return chunkSizesString;
		}

		public byte[] getReturnedBuffer()
		{
			return returnedBuffer;
		}
	}
}

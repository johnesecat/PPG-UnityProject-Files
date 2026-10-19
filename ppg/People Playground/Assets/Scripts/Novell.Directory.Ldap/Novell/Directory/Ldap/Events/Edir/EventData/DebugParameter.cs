using System;
using System.Collections;
using System.IO;
using System.Text;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Events.Edir.EventData
{
	public class DebugParameter
	{
		protected DebugParameterType debug_type;

		protected object objData;

		public DebugParameterType DebugType => debug_type;

		public object Data => objData;

		public DebugParameter(Asn1Tagged dseObject)
		{
			switch (dseObject.getIdentifier().Tag)
			{
			case 1:
			case 4:
				objData = getTaggedIntValue(dseObject);
				break;
			case 3:
				objData = ((Asn1OctetString)dseObject.taggedValue()).byteValue();
				break;
			case 2:
				objData = ((Asn1OctetString)dseObject.taggedValue()).stringValue();
				break;
			case 6:
				objData = new DSETimeStamp(getTaggedSequence(dseObject));
				break;
			case 7:
			{
				ArrayList arrayList = new ArrayList();
				Asn1Sequence taggedSequence = getTaggedSequence(dseObject);
				int num = ((Asn1Integer)taggedSequence.get_Renamed(0)).intValue();
				if (num > 0)
				{
					Asn1Sequence asn1Sequence = (Asn1Sequence)taggedSequence.get_Renamed(1);
					for (int i = 0; i < num; i++)
					{
						arrayList.Add(new DSETimeStamp((Asn1Sequence)asn1Sequence.get_Renamed(i)));
					}
				}
				objData = arrayList;
				break;
			}
			case 5:
				objData = new ReferralAddress(getTaggedSequence(dseObject));
				break;
			default:
				throw new IOException("Unknown Tag in DebugParameter..");
			}
			debug_type = (DebugParameterType)dseObject.getIdentifier().Tag;
		}

		protected int getTaggedIntValue(Asn1Tagged tagVal)
		{
			byte[] array = SupportClass.ToByteArray(((Asn1OctetString)tagVal.taggedValue()).byteValue());
			MemoryStream in_Renamed = new MemoryStream(array);
			return (int)new LBERDecoder().decodeNumeric(in_Renamed, array.Length);
		}

		protected Asn1Sequence getTaggedSequence(Asn1Tagged tagVal)
		{
			byte[] array = SupportClass.ToByteArray(((Asn1OctetString)tagVal.taggedValue()).byteValue());
			MemoryStream in_Renamed = new MemoryStream(array);
			return new Asn1Sequence(new LBERDecoder(), in_Renamed, array.Length);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[DebugParameter");
			if (Enum.IsDefined(debug_type.GetType(), debug_type))
			{
				stringBuilder.AppendFormat("(type={0},", debug_type);
				stringBuilder.AppendFormat("value={0})", objData);
			}
			else
			{
				stringBuilder.Append("(type=Unknown)");
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}
	}
}

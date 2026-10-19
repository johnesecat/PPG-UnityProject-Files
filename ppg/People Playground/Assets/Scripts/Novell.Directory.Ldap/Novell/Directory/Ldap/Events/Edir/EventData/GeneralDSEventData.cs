using System.IO;
using System.Text;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Events.Edir.EventData
{
	public class GeneralDSEventData : BaseEdirEventData
	{
		protected int ds_time;

		protected int milli_seconds;

		protected int nVerb;

		protected int current_process;

		protected string strPerpetratorDN;

		protected int[] integer_values;

		protected string[] string_values;

		public int DSTime => ds_time;

		public int MilliSeconds => milli_seconds;

		public int Verb => nVerb;

		public int CurrentProcess => current_process;

		public string PerpetratorDN => strPerpetratorDN;

		public int[] IntegerValues => integer_values;

		public string[] StringValues => string_values;

		public GeneralDSEventData(EdirEventDataType eventDataType, Asn1Object message)
			: base(eventDataType, message)
		{
			int[] len = new int[1];
			ds_time = getTaggedIntValue((Asn1Tagged)decoder.decode(decodedData, len), GeneralEventField.EVT_TAG_GEN_DSTIME);
			milli_seconds = getTaggedIntValue((Asn1Tagged)decoder.decode(decodedData, len), GeneralEventField.EVT_TAG_GEN_MILLISEC);
			nVerb = getTaggedIntValue((Asn1Tagged)decoder.decode(decodedData, len), GeneralEventField.EVT_TAG_GEN_VERB);
			current_process = getTaggedIntValue((Asn1Tagged)decoder.decode(decodedData, len), GeneralEventField.EVT_TAG_GEN_CURRPROC);
			strPerpetratorDN = getTaggedStringValue((Asn1Tagged)decoder.decode(decodedData, len), GeneralEventField.EVT_TAG_GEN_PERP);
			Asn1Tagged asn1Tagged = (Asn1Tagged)decoder.decode(decodedData, len);
			if (asn1Tagged.getIdentifier().Tag == 6)
			{
				Asn1Object[] array = getTaggedSequence(asn1Tagged, GeneralEventField.EVT_TAG_GEN_INTEGERS).toArray();
				integer_values = new int[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					integer_values[i] = ((Asn1Integer)array[i]).intValue();
				}
				asn1Tagged = (Asn1Tagged)decoder.decode(decodedData, len);
			}
			else
			{
				integer_values = null;
			}
			if (asn1Tagged.getIdentifier().Tag == 7 && asn1Tagged.getIdentifier().Constructed)
			{
				Asn1Object[] array2 = getTaggedSequence(asn1Tagged, GeneralEventField.EVT_TAG_GEN_STRINGS).toArray();
				string_values = new string[array2.Length];
				for (int j = 0; j < array2.Length; j++)
				{
					string_values[j] = ((Asn1OctetString)array2[j]).stringValue();
				}
			}
			else
			{
				string_values = null;
			}
			DataInitDone();
		}

		protected int getTaggedIntValue(Asn1Tagged tagvalue, GeneralEventField tagid)
		{
			Asn1Object asn1Object = tagvalue.taggedValue();
			if (tagid != (GeneralEventField)tagvalue.getIdentifier().Tag)
			{
				throw new IOException("Unknown Tagged Data");
			}
			byte[] array = SupportClass.ToByteArray(((Asn1OctetString)asn1Object).byteValue());
			MemoryStream in_Renamed = new MemoryStream(array);
			LBERDecoder lBERDecoder = new LBERDecoder();
			int len = array.Length;
			return (int)lBERDecoder.decodeNumeric(in_Renamed, len);
		}

		protected string getTaggedStringValue(Asn1Tagged tagvalue, GeneralEventField tagid)
		{
			Asn1Object asn1Object = tagvalue.taggedValue();
			if (tagid != (GeneralEventField)tagvalue.getIdentifier().Tag)
			{
				throw new IOException("Unknown Tagged Data");
			}
			byte[] array = SupportClass.ToByteArray(((Asn1OctetString)asn1Object).byteValue());
			MemoryStream in_Renamed = new MemoryStream(array);
			LBERDecoder lBERDecoder = new LBERDecoder();
			int len = array.Length;
			return (string)lBERDecoder.decodeCharacterString(in_Renamed, len);
		}

		protected Asn1Sequence getTaggedSequence(Asn1Tagged tagvalue, GeneralEventField tagid)
		{
			Asn1Object asn1Object = tagvalue.taggedValue();
			if (tagid != (GeneralEventField)tagvalue.getIdentifier().Tag)
			{
				throw new IOException("Unknown Tagged Data");
			}
			byte[] array = SupportClass.ToByteArray(((Asn1OctetString)asn1Object).byteValue());
			MemoryStream in_Renamed = new MemoryStream(array);
			LBERDecoder dec = new LBERDecoder();
			int len = array.Length;
			return new Asn1Sequence(dec, in_Renamed, len);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[GeneralDSEventData");
			stringBuilder.AppendFormat("(DSTime={0})", ds_time);
			stringBuilder.AppendFormat("(MilliSeconds={0})", milli_seconds);
			stringBuilder.AppendFormat("(verb={0})", nVerb);
			stringBuilder.AppendFormat("(currentProcess={0})", current_process);
			stringBuilder.AppendFormat("(PerpetartorDN={0})", strPerpetratorDN);
			stringBuilder.AppendFormat("(Integer Values={0})", integer_values);
			stringBuilder.AppendFormat("(String Values={0})", string_values);
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}
	}
}

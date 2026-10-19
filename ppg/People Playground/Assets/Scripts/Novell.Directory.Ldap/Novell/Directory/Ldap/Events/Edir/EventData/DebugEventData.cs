using System.Collections;
using System.Text;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Events.Edir.EventData
{
	public class DebugEventData : BaseEdirEventData
	{
		protected int ds_time;

		protected int milli_seconds;

		protected string strPerpetratorDN;

		protected string strFormatString;

		protected int nVerb;

		protected int parameter_count;

		protected ArrayList parameter_collection;

		public int DSTime => ds_time;

		public int MilliSeconds => milli_seconds;

		public string PerpetratorDN => strPerpetratorDN;

		public string FormatString => strFormatString;

		public int Verb => nVerb;

		public int ParameterCount => parameter_count;

		public ArrayList Parameters => parameter_collection;

		public DebugEventData(EdirEventDataType eventDataType, Asn1Object message)
			: base(eventDataType, message)
		{
			int[] len = new int[1];
			ds_time = ((Asn1Integer)decoder.decode(decodedData, len)).intValue();
			milli_seconds = ((Asn1Integer)decoder.decode(decodedData, len)).intValue();
			strPerpetratorDN = ((Asn1OctetString)decoder.decode(decodedData, len)).stringValue();
			strFormatString = ((Asn1OctetString)decoder.decode(decodedData, len)).stringValue();
			nVerb = ((Asn1Integer)decoder.decode(decodedData, len)).intValue();
			parameter_count = ((Asn1Integer)decoder.decode(decodedData, len)).intValue();
			parameter_collection = new ArrayList();
			if (parameter_count > 0)
			{
				Asn1Sequence asn1Sequence = (Asn1Sequence)decoder.decode(decodedData, len);
				for (int i = 0; i < parameter_count; i++)
				{
					parameter_collection.Add(new DebugParameter((Asn1Tagged)asn1Sequence.get_Renamed(i)));
				}
			}
			DataInitDone();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[DebugEventData");
			stringBuilder.AppendFormat("(Millseconds={0})", milli_seconds);
			stringBuilder.AppendFormat("(DSTime={0})", ds_time);
			stringBuilder.AppendFormat("(PerpetratorDN={0})", strPerpetratorDN);
			stringBuilder.AppendFormat("(Verb={0})", nVerb);
			stringBuilder.AppendFormat("(ParameterCount={0})", parameter_count);
			for (int i = 0; i < parameter_count; i++)
			{
				stringBuilder.AppendFormat("(Parameter[{0}]={1})", i, parameter_collection[i]);
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}
	}
}

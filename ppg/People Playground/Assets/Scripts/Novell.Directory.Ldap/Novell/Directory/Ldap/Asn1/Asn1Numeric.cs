namespace Novell.Directory.Ldap.Asn1
{
	public abstract class Asn1Numeric : Asn1Object
	{
		private long content;

		internal Asn1Numeric(Asn1Identifier id, int value_Renamed)
			: base(id)
		{
			content = value_Renamed;
		}

		internal Asn1Numeric(Asn1Identifier id, long value_Renamed)
			: base(id)
		{
			content = value_Renamed;
		}

		public int intValue()
		{
			return (int)content;
		}

		public long longValue()
		{
			return content;
		}
	}
}

using System;
using System.IO;

namespace Novell.Directory.Ldap.Asn1
{
	public class Asn1Integer : Asn1Numeric
	{
		public const int TAG = 2;

		public static readonly Asn1Identifier ID = new Asn1Identifier(0, constructed: false, 2);

		public Asn1Integer(int content)
			: base(ID, content)
		{
		}

		public Asn1Integer(long content)
			: base(ID, content)
		{
		}

		[CLSCompliant(false)]
		public Asn1Integer(Asn1Decoder dec, Stream in_Renamed, int len)
			: base(ID, (long)dec.decodeNumeric(in_Renamed, len))
		{
		}

		public override void encode(Asn1Encoder enc, Stream out_Renamed)
		{
			enc.encode(this, out_Renamed);
		}

		public override string ToString()
		{
			return base.ToString() + "INTEGER: " + longValue();
		}
	}
}

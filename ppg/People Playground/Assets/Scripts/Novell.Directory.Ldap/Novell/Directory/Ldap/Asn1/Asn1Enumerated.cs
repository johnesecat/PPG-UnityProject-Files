using System;
using System.IO;

namespace Novell.Directory.Ldap.Asn1
{
	public class Asn1Enumerated : Asn1Numeric
	{
		public const int TAG = 10;

		public static readonly Asn1Identifier ID = new Asn1Identifier(0, constructed: false, 10);

		public Asn1Enumerated(int content)
			: base(ID, content)
		{
		}

		public Asn1Enumerated(long content)
			: base(ID, content)
		{
		}

		[CLSCompliant(false)]
		public Asn1Enumerated(Asn1Decoder dec, Stream in_Renamed, int len)
			: base(ID, (long)dec.decodeNumeric(in_Renamed, len))
		{
		}

		public override void encode(Asn1Encoder enc, Stream out_Renamed)
		{
			enc.encode(this, out_Renamed);
		}

		public override string ToString()
		{
			return base.ToString() + "ENUMERATED: " + longValue();
		}
	}
}

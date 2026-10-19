using System;
using System.IO;

namespace Novell.Directory.Ldap.Asn1
{
	public class Asn1Set : Asn1Structured
	{
		public const int TAG = 17;

		public static readonly Asn1Identifier ID = new Asn1Identifier(0, constructed: true, 17);

		public Asn1Set()
			: base(ID)
		{
		}

		public Asn1Set(int size)
			: base(ID, size)
		{
		}

		[CLSCompliant(false)]
		public Asn1Set(Asn1Decoder dec, Stream in_Renamed, int len)
			: base(ID)
		{
			decodeStructured(dec, in_Renamed, len);
		}

		[CLSCompliant(false)]
		public override string ToString()
		{
			return base.toString("SET: { ");
		}
	}
}

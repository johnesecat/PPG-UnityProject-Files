using System;
using System.IO;

namespace Novell.Directory.Ldap.Asn1
{
	public class Asn1Sequence : Asn1Structured
	{
		public const int TAG = 16;

		private static readonly Asn1Identifier ID = new Asn1Identifier(0, constructed: true, 16);

		public Asn1Sequence()
			: base(ID, 10)
		{
		}

		public Asn1Sequence(int size)
			: base(ID, size)
		{
		}

		public Asn1Sequence(Asn1Object[] newContent, int size)
			: base(ID, newContent, size)
		{
		}

		[CLSCompliant(false)]
		public Asn1Sequence(Asn1Decoder dec, Stream in_Renamed, int len)
			: base(ID)
		{
			decodeStructured(dec, in_Renamed, len);
		}

		[CLSCompliant(false)]
		public override string ToString()
		{
			return base.toString("SEQUENCE: { ");
		}
	}
}

using System;
using System.IO;

namespace Novell.Directory.Ldap.Asn1
{
	public class Asn1SequenceOf : Asn1Structured
	{
		public const int TAG = 16;

		public static readonly Asn1Identifier ID = new Asn1Identifier(0, constructed: true, 16);

		public Asn1SequenceOf()
			: base(ID)
		{
		}

		public Asn1SequenceOf(int size)
			: base(ID, size)
		{
		}

		public Asn1SequenceOf(Asn1Sequence sequence)
			: base(ID, sequence.toArray(), sequence.size())
		{
		}

		[CLSCompliant(false)]
		public Asn1SequenceOf(Asn1Decoder dec, Stream in_Renamed, int len)
			: base(ID)
		{
			decodeStructured(dec, in_Renamed, len);
		}

		[CLSCompliant(false)]
		public override string ToString()
		{
			return base.toString("SEQUENCE OF: { ");
		}
	}
}

using System;

namespace Novell.Directory.Ldap.Asn1
{
	public class Asn1SetOf : Asn1Structured
	{
		public const int TAG = 17;

		public static readonly Asn1Identifier ID = new Asn1Identifier(0, constructed: true, 17);

		public Asn1SetOf()
			: base(ID)
		{
		}

		public Asn1SetOf(int size)
			: base(ID, size)
		{
		}

		public Asn1SetOf(Asn1Set set_Renamed)
			: base(ID, set_Renamed.toArray(), set_Renamed.size())
		{
		}

		[CLSCompliant(false)]
		public override string ToString()
		{
			return base.toString("SET OF: { ");
		}
	}
}

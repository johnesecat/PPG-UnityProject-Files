using System.IO;

namespace Novell.Directory.Ldap.Asn1
{
	public class Asn1Null : Asn1Object
	{
		public const int TAG = 5;

		public static readonly Asn1Identifier ID = new Asn1Identifier(0, constructed: false, 5);

		public Asn1Null()
			: base(ID)
		{
		}

		public override void encode(Asn1Encoder enc, Stream out_Renamed)
		{
			enc.encode(this, out_Renamed);
		}

		public override string ToString()
		{
			return base.ToString() + "NULL: \"\"";
		}
	}
}

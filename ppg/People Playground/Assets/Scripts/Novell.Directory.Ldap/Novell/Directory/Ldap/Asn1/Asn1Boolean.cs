using System;
using System.IO;

namespace Novell.Directory.Ldap.Asn1
{
	public class Asn1Boolean : Asn1Object
	{
		private bool content;

		public const int TAG = 1;

		public static readonly Asn1Identifier ID = new Asn1Identifier(0, constructed: false, 1);

		public Asn1Boolean(bool content)
			: base(ID)
		{
			this.content = content;
		}

		[CLSCompliant(false)]
		public Asn1Boolean(Asn1Decoder dec, Stream in_Renamed, int len)
			: base(ID)
		{
			content = (bool)dec.decodeBoolean(in_Renamed, len);
		}

		public override void encode(Asn1Encoder enc, Stream out_Renamed)
		{
			enc.encode(this, out_Renamed);
		}

		public bool booleanValue()
		{
			return content;
		}

		public override string ToString()
		{
			return base.ToString() + "BOOLEAN: " + content;
		}
	}
}

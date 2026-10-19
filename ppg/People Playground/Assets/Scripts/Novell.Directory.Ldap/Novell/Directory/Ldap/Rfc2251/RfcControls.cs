using System;
using System.IO;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcControls : Asn1SequenceOf
	{
		public const int CONTROLS = 0;

		public RfcControls()
			: base(5)
		{
		}

		[CLSCompliant(false)]
		public RfcControls(Asn1Decoder dec, Stream in_Renamed, int len)
			: base(dec, in_Renamed, len)
		{
			for (int i = 0; i < size(); i++)
			{
				RfcControl control = new RfcControl((Asn1Sequence)get_Renamed(i));
				set_Renamed(i, control);
			}
		}

		public void add(RfcControl control)
		{
			add((Asn1Object)control);
		}

		public void set_Renamed(int index, RfcControl control)
		{
			set_Renamed(index, (Asn1Object)control);
		}

		public override Asn1Identifier getIdentifier()
		{
			return new Asn1Identifier(2, constructed: true, 0);
		}
	}
}

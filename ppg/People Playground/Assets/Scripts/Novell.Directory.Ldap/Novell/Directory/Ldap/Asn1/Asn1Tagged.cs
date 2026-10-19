using System;
using System.IO;

namespace Novell.Directory.Ldap.Asn1
{
	public class Asn1Tagged : Asn1Object
	{
		private bool explicit_Renamed;

		private Asn1Object content;

		[CLSCompliant(false)]
		public virtual Asn1Object TaggedValue
		{
			set
			{
				content = value;
				if (!explicit_Renamed)
				{
					value?.setIdentifier(getIdentifier());
				}
			}
		}

		public virtual bool Explicit => explicit_Renamed;

		public Asn1Tagged(Asn1Identifier identifier, Asn1Object object_Renamed)
			: this(identifier, object_Renamed, explicit_Renamed: true)
		{
		}

		public Asn1Tagged(Asn1Identifier identifier, Asn1Object object_Renamed, bool explicit_Renamed)
			: base(identifier)
		{
			content = object_Renamed;
			this.explicit_Renamed = explicit_Renamed;
			if (!explicit_Renamed && content != null)
			{
				content.setIdentifier(identifier);
			}
		}

		[CLSCompliant(false)]
		public Asn1Tagged(Asn1Decoder dec, Stream in_Renamed, int len, Asn1Identifier identifier)
			: base(identifier)
		{
			content = new Asn1OctetString(dec, in_Renamed, len);
		}

		public override void encode(Asn1Encoder enc, Stream out_Renamed)
		{
			enc.encode(this, out_Renamed);
		}

		public Asn1Object taggedValue()
		{
			return content;
		}

		public override string ToString()
		{
			if (explicit_Renamed)
			{
				return base.ToString() + content.ToString();
			}
			return content.ToString();
		}
	}
}

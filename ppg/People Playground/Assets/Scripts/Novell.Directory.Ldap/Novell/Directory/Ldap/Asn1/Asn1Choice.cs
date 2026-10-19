using System;
using System.IO;

namespace Novell.Directory.Ldap.Asn1
{
	public class Asn1Choice : Asn1Object
	{
		private Asn1Object content;

		[CLSCompliant(false)]
		protected internal virtual Asn1Object ChoiceValue
		{
			set
			{
				content = value;
			}
		}

		public Asn1Choice(Asn1Object content)
			: base(null)
		{
			this.content = content;
		}

		protected internal Asn1Choice()
			: base(null)
		{
			content = null;
		}

		public override void encode(Asn1Encoder enc, Stream out_Renamed)
		{
			content.encode(enc, out_Renamed);
		}

		public Asn1Object choiceValue()
		{
			return content;
		}

		public override Asn1Identifier getIdentifier()
		{
			return content.getIdentifier();
		}

		public override void setIdentifier(Asn1Identifier id)
		{
			content.setIdentifier(id);
		}

		public override string ToString()
		{
			return content.ToString();
		}
	}
}

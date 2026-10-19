using System;
using System.IO;
using System.Text;

namespace Novell.Directory.Ldap.Asn1
{
	public class Asn1OctetString : Asn1Object
	{
		private sbyte[] content;

		public const int TAG = 4;

		protected internal static readonly Asn1Identifier ID = new Asn1Identifier(0, constructed: false, 4);

		[CLSCompliant(false)]
		public Asn1OctetString(sbyte[] content)
			: base(ID)
		{
			this.content = content;
		}

		public Asn1OctetString(string content)
			: base(ID)
		{
			try
			{
				sbyte[] array = (this.content = SupportClass.ToSByteArray(Encoding.GetEncoding("utf-8").GetBytes(content)));
			}
			catch (IOException ex)
			{
				throw new SystemException(ex.ToString());
			}
		}

		[CLSCompliant(false)]
		public Asn1OctetString(Asn1Decoder dec, Stream in_Renamed, int len)
			: base(ID)
		{
			content = ((len > 0) ? ((sbyte[])dec.decodeOctetString(in_Renamed, len)) : new sbyte[0]);
		}

		public override void encode(Asn1Encoder enc, Stream out_Renamed)
		{
			enc.encode(this, out_Renamed);
		}

		[CLSCompliant(false)]
		public sbyte[] byteValue()
		{
			return content;
		}

		public string stringValue()
		{
			string text = null;
			try
			{
				return new string(Encoding.GetEncoding("utf-8").GetChars(SupportClass.ToByteArray(content)));
			}
			catch (IOException ex)
			{
				throw new SystemException(ex.ToString());
			}
		}

		public override string ToString()
		{
			return base.ToString() + "OCTET STRING: " + stringValue();
		}
	}
}

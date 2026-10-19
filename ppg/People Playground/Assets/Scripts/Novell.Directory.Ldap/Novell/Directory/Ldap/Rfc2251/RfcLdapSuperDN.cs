using System;
using System.IO;
using System.Text;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcLdapSuperDN : Asn1Tagged
	{
		private sbyte[] content;

		public static readonly int TAG = 0;

		protected static readonly Asn1Identifier ID = new Asn1Identifier(2, constructed: false, TAG);

		public RfcLdapSuperDN(string s)
			: base(ID, new Asn1OctetString(s), explicit_Renamed: false)
		{
			try
			{
				sbyte[] array = (content = SupportClass.ToSByteArray(Encoding.GetEncoding("utf-8").GetBytes(s)));
			}
			catch (IOException ex)
			{
				throw new SystemException(ex.ToString());
			}
		}

		[CLSCompliant(false)]
		public RfcLdapSuperDN(sbyte[] ba)
			: base(ID, new Asn1OctetString(ba), explicit_Renamed: false)
		{
			content = ba;
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
			return base.ToString() + " " + stringValue();
		}
	}
}

using System;
using System.IO;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcBindResponse : Asn1Sequence, RfcResponse
	{
		public virtual Asn1OctetString ServerSaslCreds
		{
			get
			{
				if (size() == 5)
				{
					return (Asn1OctetString)((Asn1Tagged)get_Renamed(4)).taggedValue();
				}
				if (size() == 4)
				{
					Asn1Object asn1Object = get_Renamed(3);
					if (asn1Object is Asn1Tagged)
					{
						return (Asn1OctetString)((Asn1Tagged)asn1Object).taggedValue();
					}
				}
				return null;
			}
		}

		[CLSCompliant(false)]
		public RfcBindResponse(Asn1Decoder dec, Stream in_Renamed, int len)
			: base(dec, in_Renamed, len)
		{
			if (size() > 3)
			{
				Asn1Tagged asn1Tagged = (Asn1Tagged)get_Renamed(3);
				if (asn1Tagged.getIdentifier().Tag == 3)
				{
					sbyte[] array = ((Asn1OctetString)asn1Tagged.taggedValue()).byteValue();
					MemoryStream in_Renamed2 = new MemoryStream(SupportClass.ToByteArray(array));
					set_Renamed(3, new RfcReferral(dec, in_Renamed2, array.Length));
				}
			}
		}

		public Asn1Enumerated getResultCode()
		{
			return (Asn1Enumerated)get_Renamed(0);
		}

		public RfcLdapDN getMatchedDN()
		{
			return new RfcLdapDN(((Asn1OctetString)get_Renamed(1)).byteValue());
		}

		public RfcLdapString getErrorMessage()
		{
			return new RfcLdapString(((Asn1OctetString)get_Renamed(2)).byteValue());
		}

		public RfcReferral getReferral()
		{
			if (size() > 3)
			{
				Asn1Object asn1Object = get_Renamed(3);
				if (asn1Object is RfcReferral)
				{
					return (RfcReferral)asn1Object;
				}
			}
			return null;
		}

		public override Asn1Identifier getIdentifier()
		{
			return new Asn1Identifier(1, constructed: true, 1);
		}
	}
}

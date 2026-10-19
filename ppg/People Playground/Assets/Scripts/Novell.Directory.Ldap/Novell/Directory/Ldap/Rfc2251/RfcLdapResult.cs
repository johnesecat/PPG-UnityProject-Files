using System;
using System.IO;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcLdapResult : Asn1Sequence, RfcResponse
	{
		public const int REFERRAL = 3;

		public RfcLdapResult(Asn1Enumerated resultCode, RfcLdapDN matchedDN, RfcLdapString errorMessage)
			: this(resultCode, matchedDN, errorMessage, null)
		{
		}

		public RfcLdapResult(Asn1Enumerated resultCode, RfcLdapDN matchedDN, RfcLdapString errorMessage, RfcReferral referral)
			: base(4)
		{
			add(resultCode);
			add(matchedDN);
			add(errorMessage);
			if (referral != null)
			{
				add(referral);
			}
		}

		[CLSCompliant(false)]
		public RfcLdapResult(Asn1Decoder dec, Stream in_Renamed, int len)
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
			if (size() <= 3)
			{
				return null;
			}
			return (RfcReferral)get_Renamed(3);
		}
	}
}

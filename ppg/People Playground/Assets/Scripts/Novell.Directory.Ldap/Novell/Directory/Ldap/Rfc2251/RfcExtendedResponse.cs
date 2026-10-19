using System;
using System.IO;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcExtendedResponse : Asn1Sequence, RfcResponse
	{
		public const int RESPONSE_NAME = 10;

		public const int RESPONSE = 11;

		private int referralIndex;

		private int responseNameIndex;

		private int responseIndex;

		public virtual RfcLdapOID ResponseName
		{
			get
			{
				if (responseNameIndex == 0)
				{
					return null;
				}
				return (RfcLdapOID)get_Renamed(responseNameIndex);
			}
		}

		[CLSCompliant(false)]
		public virtual Asn1OctetString Response
		{
			get
			{
				if (responseIndex == 0)
				{
					return null;
				}
				return (Asn1OctetString)get_Renamed(responseIndex);
			}
		}

		[CLSCompliant(false)]
		public RfcExtendedResponse(Asn1Decoder dec, Stream in_Renamed, int len)
			: base(dec, in_Renamed, len)
		{
			if (size() <= 3)
			{
				return;
			}
			for (int i = 3; i < size(); i++)
			{
				Asn1Tagged asn1Tagged = (Asn1Tagged)get_Renamed(i);
				switch (asn1Tagged.getIdentifier().Tag)
				{
				case 3:
				{
					sbyte[] array = ((Asn1OctetString)asn1Tagged.taggedValue()).byteValue();
					MemoryStream in_Renamed2 = new MemoryStream(SupportClass.ToByteArray(array));
					set_Renamed(i, new RfcReferral(dec, in_Renamed2, array.Length));
					referralIndex = i;
					break;
				}
				case 10:
					set_Renamed(i, new RfcLdapOID(((Asn1OctetString)asn1Tagged.taggedValue()).byteValue()));
					responseNameIndex = i;
					break;
				case 11:
					set_Renamed(i, asn1Tagged.taggedValue());
					responseIndex = i;
					break;
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
			if (referralIndex == 0)
			{
				return null;
			}
			return (RfcReferral)get_Renamed(referralIndex);
		}

		public override Asn1Identifier getIdentifier()
		{
			return new Asn1Identifier(1, constructed: true, 24);
		}
	}
}

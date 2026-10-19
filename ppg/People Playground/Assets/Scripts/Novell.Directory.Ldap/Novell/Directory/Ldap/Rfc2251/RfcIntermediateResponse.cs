using System;
using System.IO;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcIntermediateResponse : Asn1Sequence, RfcResponse
	{
		public const int TAG_RESPONSE_NAME = 0;

		public const int TAG_RESPONSE = 1;

		private int m_referralIndex;

		private int m_responseNameIndex;

		private int m_responseValueIndex;

		[CLSCompliant(false)]
		public RfcIntermediateResponse(Asn1Decoder dec, Stream in_Renamed, int len)
			: base(dec, in_Renamed, len)
		{
			int num = 0;
			m_responseNameIndex = (m_responseValueIndex = 0);
			for (num = ((size() >= 3) ? 3 : 0); num < size(); num++)
			{
				Asn1Tagged asn1Tagged = (Asn1Tagged)get_Renamed(num);
				switch (asn1Tagged.getIdentifier().Tag)
				{
				case 0:
					set_Renamed(num, new RfcLdapOID(((Asn1OctetString)asn1Tagged.taggedValue()).byteValue()));
					m_responseNameIndex = num;
					break;
				case 1:
					set_Renamed(num, asn1Tagged.taggedValue());
					m_responseValueIndex = num;
					break;
				}
			}
		}

		public Asn1Enumerated getResultCode()
		{
			if (size() > 3)
			{
				return (Asn1Enumerated)get_Renamed(0);
			}
			return null;
		}

		public RfcLdapDN getMatchedDN()
		{
			if (size() > 3)
			{
				return new RfcLdapDN(((Asn1OctetString)get_Renamed(1)).byteValue());
			}
			return null;
		}

		public RfcLdapString getErrorMessage()
		{
			if (size() > 3)
			{
				return new RfcLdapString(((Asn1OctetString)get_Renamed(2)).byteValue());
			}
			return null;
		}

		public RfcReferral getReferral()
		{
			if (size() <= 3)
			{
				return null;
			}
			return (RfcReferral)get_Renamed(3);
		}

		public RfcLdapOID getResponseName()
		{
			if (m_responseNameIndex < 0)
			{
				return null;
			}
			return (RfcLdapOID)get_Renamed(m_responseNameIndex);
		}

		public Asn1OctetString getResponse()
		{
			if (m_responseValueIndex == 0)
			{
				return null;
			}
			return (Asn1OctetString)get_Renamed(m_responseValueIndex);
		}

		public override Asn1Identifier getIdentifier()
		{
			return new Asn1Identifier(1, constructed: true, 25);
		}
	}
}

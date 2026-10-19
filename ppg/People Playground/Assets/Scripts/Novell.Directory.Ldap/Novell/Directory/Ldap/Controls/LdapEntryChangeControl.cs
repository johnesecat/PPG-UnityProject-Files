using System;
using System.IO;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Controls
{
	public class LdapEntryChangeControl : LdapControl
	{
		private int m_changeType;

		private string m_previousDN;

		private bool m_hasChangeNumber;

		private int m_changeNumber;

		public virtual bool HasChangeNumber => m_hasChangeNumber;

		public virtual int ChangeNumber => m_changeNumber;

		public virtual int ChangeType => m_changeType;

		public virtual string PreviousDN => m_previousDN;

		[CLSCompliant(false)]
		public LdapEntryChangeControl(string oid, bool critical, sbyte[] value_Renamed)
			: base(oid, critical, value_Renamed)
		{
			Asn1Object asn1Object = (new LBERDecoder() ?? throw new IOException("Decoding error.")).decode(value_Renamed);
			if (asn1Object == null || !(asn1Object is Asn1Sequence))
			{
				throw new IOException("Decoding error.");
			}
			Asn1Sequence asn1Sequence = (Asn1Sequence)asn1Object;
			Asn1Object asn1Object2 = asn1Sequence.get_Renamed(0);
			if (asn1Object2 == null || !(asn1Object2 is Asn1Enumerated))
			{
				throw new IOException("Decoding error.");
			}
			m_changeType = ((Asn1Enumerated)asn1Object2).intValue();
			if (asn1Sequence.size() > 1 && m_changeType == 8)
			{
				asn1Object2 = asn1Sequence.get_Renamed(1);
				if (asn1Object2 == null || !(asn1Object2 is Asn1OctetString))
				{
					throw new IOException("Decoding error get previous DN");
				}
				m_previousDN = ((Asn1OctetString)asn1Object2).stringValue();
			}
			else
			{
				m_previousDN = "";
			}
			if (asn1Sequence.size() == 3)
			{
				asn1Object2 = asn1Sequence.get_Renamed(2);
				if (asn1Object2 == null || !(asn1Object2 is Asn1Integer))
				{
					throw new IOException("Decoding error getting change number");
				}
				m_changeNumber = ((Asn1Integer)asn1Object2).intValue();
				m_hasChangeNumber = true;
			}
			else
			{
				m_hasChangeNumber = false;
			}
		}

		public override string ToString()
		{
			return base.ToString();
		}
	}
}

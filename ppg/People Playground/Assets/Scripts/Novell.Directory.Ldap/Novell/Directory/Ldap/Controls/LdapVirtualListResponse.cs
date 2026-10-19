using System;
using System.IO;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Controls
{
	public class LdapVirtualListResponse : LdapControl
	{
		private int m_firstPosition;

		private int m_ContentCount;

		private int m_resultCode;

		private string m_context;

		public virtual int ContentCount => m_ContentCount;

		public virtual int FirstPosition => m_firstPosition;

		public virtual int ResultCode => m_resultCode;

		public virtual string Context => m_context;

		[CLSCompliant(false)]
		public LdapVirtualListResponse(string oid, bool critical, sbyte[] values)
			: base(oid, critical, values)
		{
			Asn1Object asn1Object = (new LBERDecoder() ?? throw new IOException("Decoding error")).decode(values);
			if (asn1Object == null || !(asn1Object is Asn1Sequence))
			{
				throw new IOException("Decoding error");
			}
			Asn1Object asn1Object2 = ((Asn1Sequence)asn1Object).get_Renamed(0);
			if (asn1Object2 != null && asn1Object2 is Asn1Integer)
			{
				m_firstPosition = ((Asn1Integer)asn1Object2).intValue();
				Asn1Object asn1Object3 = ((Asn1Sequence)asn1Object).get_Renamed(1);
				if (asn1Object3 != null && asn1Object3 is Asn1Integer)
				{
					m_ContentCount = ((Asn1Integer)asn1Object3).intValue();
					Asn1Object asn1Object4 = ((Asn1Sequence)asn1Object).get_Renamed(2);
					if (asn1Object4 != null && asn1Object4 is Asn1Enumerated)
					{
						m_resultCode = ((Asn1Enumerated)asn1Object4).intValue();
						if (((Asn1Sequence)asn1Object).size() > 3)
						{
							Asn1Object asn1Object5 = ((Asn1Sequence)asn1Object).get_Renamed(3);
							if (asn1Object5 != null && asn1Object5 is Asn1OctetString)
							{
								m_context = ((Asn1OctetString)asn1Object5).stringValue();
							}
						}
						return;
					}
					throw new IOException("Decoding error");
				}
				throw new IOException("Decoding error");
			}
			throw new IOException("Decoding error");
		}
	}
}

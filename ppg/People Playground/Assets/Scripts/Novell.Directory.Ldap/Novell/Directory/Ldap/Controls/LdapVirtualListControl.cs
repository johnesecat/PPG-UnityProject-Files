using System;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Controls
{
	public class LdapVirtualListControl : LdapControl
	{
		private static int BYOFFSET;

		private static int GREATERTHANOREQUAL;

		private static string requestOID;

		private static string responseOID;

		private Asn1Sequence m_vlvRequest;

		private int m_beforeCount;

		private int m_afterCount;

		private string m_jumpTo;

		private string m_context;

		private int m_startIndex;

		private int m_contentCount = -1;

		public virtual int AfterCount => m_afterCount;

		public virtual int BeforeCount => m_beforeCount;

		public virtual int ListSize
		{
			get
			{
				return m_contentCount;
			}
			set
			{
				m_contentCount = value;
				BuildIndexedVLVRequest();
				setValue(m_vlvRequest.getEncoding(new LBEREncoder()));
			}
		}

		public virtual string Context
		{
			get
			{
				return m_context;
			}
			set
			{
				int index = 3;
				m_context = value;
				if (m_vlvRequest.size() == 4)
				{
					m_vlvRequest.set_Renamed(index, new Asn1OctetString(m_context));
				}
				else if (m_vlvRequest.size() == 3)
				{
					m_vlvRequest.add(new Asn1OctetString(m_context));
				}
				setValue(m_vlvRequest.getEncoding(new LBEREncoder()));
			}
		}

		public LdapVirtualListControl(string jumpTo, int beforeCount, int afterCount)
			: this(jumpTo, beforeCount, afterCount, null)
		{
		}

		public LdapVirtualListControl(string jumpTo, int beforeCount, int afterCount, string context)
			: base(requestOID, critical: true, null)
		{
			m_beforeCount = beforeCount;
			m_afterCount = afterCount;
			m_jumpTo = jumpTo;
			m_context = context;
			BuildTypedVLVRequest();
			setValue(m_vlvRequest.getEncoding(new LBEREncoder()));
		}

		private void BuildTypedVLVRequest()
		{
			m_vlvRequest = new Asn1Sequence(4);
			m_vlvRequest.add(new Asn1Integer(m_beforeCount));
			m_vlvRequest.add(new Asn1Integer(m_afterCount));
			m_vlvRequest.add(new Asn1Tagged(new Asn1Identifier(2, constructed: false, GREATERTHANOREQUAL), new Asn1OctetString(m_jumpTo), explicit_Renamed: false));
			if (m_context != null)
			{
				m_vlvRequest.add(new Asn1OctetString(m_context));
			}
		}

		public LdapVirtualListControl(int startIndex, int beforeCount, int afterCount, int contentCount)
			: this(startIndex, beforeCount, afterCount, contentCount, null)
		{
		}

		public LdapVirtualListControl(int startIndex, int beforeCount, int afterCount, int contentCount, string context)
			: base(requestOID, critical: true, null)
		{
			m_beforeCount = beforeCount;
			m_afterCount = afterCount;
			m_startIndex = startIndex;
			m_contentCount = contentCount;
			m_context = context;
			BuildIndexedVLVRequest();
			setValue(m_vlvRequest.getEncoding(new LBEREncoder()));
		}

		private void BuildIndexedVLVRequest()
		{
			m_vlvRequest = new Asn1Sequence(4);
			m_vlvRequest.add(new Asn1Integer(m_beforeCount));
			m_vlvRequest.add(new Asn1Integer(m_afterCount));
			Asn1Sequence asn1Sequence = new Asn1Sequence(2);
			asn1Sequence.add(new Asn1Integer(m_startIndex));
			asn1Sequence.add(new Asn1Integer(m_contentCount));
			m_vlvRequest.add(new Asn1Tagged(new Asn1Identifier(2, constructed: true, BYOFFSET), asn1Sequence, explicit_Renamed: false));
			if (m_context != null)
			{
				m_vlvRequest.add(new Asn1OctetString(m_context));
			}
		}

		public virtual void setRange(int listIndex, int beforeCount, int afterCount)
		{
			m_beforeCount = beforeCount;
			m_afterCount = afterCount;
			m_startIndex = listIndex;
			BuildIndexedVLVRequest();
			setValue(m_vlvRequest.getEncoding(new LBEREncoder()));
		}

		public virtual void setRange(string jumpTo, int beforeCount, int afterCount)
		{
			m_beforeCount = beforeCount;
			m_afterCount = afterCount;
			m_jumpTo = jumpTo;
			BuildTypedVLVRequest();
			setValue(m_vlvRequest.getEncoding(new LBEREncoder()));
		}

		static LdapVirtualListControl()
		{
			BYOFFSET = 0;
			GREATERTHANOREQUAL = 1;
			requestOID = "2.16.840.1.113730.3.4.9";
			responseOID = "2.16.840.1.113730.3.4.10";
			try
			{
				LdapControl.register(responseOID, Type.GetType("Novell.Directory.Ldap.Controls.LdapVirtualListResponse"));
			}
			catch (Exception)
			{
			}
		}
	}
}

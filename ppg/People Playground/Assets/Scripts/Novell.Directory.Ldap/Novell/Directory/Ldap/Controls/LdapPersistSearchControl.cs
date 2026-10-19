using System;
using System.Text;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Controls
{
	public class LdapPersistSearchControl : LdapControl
	{
		private static int SEQUENCE_SIZE;

		private static int CHANGETYPES_INDEX;

		private static int CHANGESONLY_INDEX;

		private static int RETURNCONTROLS_INDEX;

		private static LBEREncoder s_encoder;

		private int m_changeTypes;

		private bool m_changesOnly;

		private bool m_returnControls;

		private Asn1Sequence m_sequence;

		private static string requestOID;

		private static string responseOID;

		public const int ADD = 1;

		public const int DELETE = 2;

		public const int MODIFY = 4;

		public const int MODDN = 8;

		public static readonly int ANY;

		public virtual int ChangeTypes
		{
			get
			{
				return m_changeTypes;
			}
			set
			{
				m_changeTypes = value;
				m_sequence.set_Renamed(CHANGETYPES_INDEX, new Asn1Integer(m_changeTypes));
				setValue();
			}
		}

		public virtual bool ReturnControls
		{
			get
			{
				return m_returnControls;
			}
			set
			{
				m_returnControls = value;
				m_sequence.set_Renamed(RETURNCONTROLS_INDEX, new Asn1Boolean(m_returnControls));
				setValue();
			}
		}

		public virtual bool ChangesOnly
		{
			get
			{
				return m_changesOnly;
			}
			set
			{
				m_changesOnly = value;
				m_sequence.set_Renamed(CHANGESONLY_INDEX, new Asn1Boolean(m_changesOnly));
				setValue();
			}
		}

		public LdapPersistSearchControl()
			: this(ANY, changesOnly: true, returnControls: true, isCritical: true)
		{
		}

		public LdapPersistSearchControl(int changeTypes, bool changesOnly, bool returnControls, bool isCritical)
			: base(requestOID, isCritical, null)
		{
			m_changeTypes = changeTypes;
			m_changesOnly = changesOnly;
			m_returnControls = returnControls;
			m_sequence = new Asn1Sequence(SEQUENCE_SIZE);
			m_sequence.add(new Asn1Integer(m_changeTypes));
			m_sequence.add(new Asn1Boolean(m_changesOnly));
			m_sequence.add(new Asn1Boolean(m_returnControls));
			setValue();
		}

		public override string ToString()
		{
			sbyte[] encoding = m_sequence.getEncoding(s_encoder);
			StringBuilder stringBuilder = new StringBuilder(encoding.Length);
			for (int i = 0; i < encoding.Length; i++)
			{
				stringBuilder.Append(encoding[i].ToString());
				if (i < encoding.Length - 1)
				{
					stringBuilder.Append(",");
				}
			}
			return stringBuilder.ToString();
		}

		private void setValue()
		{
			base.setValue(m_sequence.getEncoding(s_encoder));
		}

		static LdapPersistSearchControl()
		{
			SEQUENCE_SIZE = 3;
			CHANGETYPES_INDEX = 0;
			CHANGESONLY_INDEX = 1;
			RETURNCONTROLS_INDEX = 2;
			requestOID = "2.16.840.1.113730.3.4.3";
			responseOID = "2.16.840.1.113730.3.4.7";
			ANY = 15;
			s_encoder = new LBEREncoder();
			try
			{
				LdapControl.register(responseOID, Type.GetType("Novell.Directory.Ldap.Controls.LdapEntryChangeControl"));
			}
			catch (Exception)
			{
			}
		}
	}
}

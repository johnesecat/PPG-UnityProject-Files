using System;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Controls
{
	public class LdapSortControl : LdapControl
	{
		private static int ORDERING_RULE;

		private static int REVERSE_ORDER;

		private static string requestOID;

		private static string responseOID;

		public LdapSortControl(LdapSortKey key, bool critical)
			: this(new LdapSortKey[1] { key }, critical)
		{
		}

		public LdapSortControl(LdapSortKey[] keys, bool critical)
			: base(requestOID, critical, null)
		{
			Asn1SequenceOf asn1SequenceOf = new Asn1SequenceOf();
			for (int i = 0; i < keys.Length; i++)
			{
				Asn1Sequence asn1Sequence = new Asn1Sequence();
				asn1Sequence.add(new Asn1OctetString(keys[i].Key));
				if (keys[i].MatchRule != null)
				{
					asn1Sequence.add(new Asn1Tagged(new Asn1Identifier(2, constructed: false, ORDERING_RULE), new Asn1OctetString(keys[i].MatchRule), explicit_Renamed: false));
				}
				if (keys[i].Reverse)
				{
					asn1Sequence.add(new Asn1Tagged(new Asn1Identifier(2, constructed: false, REVERSE_ORDER), new Asn1Boolean(content: true), explicit_Renamed: false));
				}
				asn1SequenceOf.add(asn1Sequence);
			}
			setValue(asn1SequenceOf.getEncoding(new LBEREncoder()));
		}

		static LdapSortControl()
		{
			ORDERING_RULE = 0;
			REVERSE_ORDER = 1;
			requestOID = "1.2.840.113556.1.4.473";
			responseOID = "1.2.840.113556.1.4.474";
			try
			{
				LdapControl.register(responseOID, Type.GetType("Novell.Directory.Ldap.Controls.LdapSortResponse"));
			}
			catch (Exception)
			{
			}
		}
	}
}

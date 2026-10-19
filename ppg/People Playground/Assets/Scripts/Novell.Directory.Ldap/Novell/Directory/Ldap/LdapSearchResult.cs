using System;
using Novell.Directory.Ldap.Asn1;
using Novell.Directory.Ldap.Rfc2251;

namespace Novell.Directory.Ldap
{
	public class LdapSearchResult : LdapMessage
	{
		private LdapEntry entry;

		public virtual LdapEntry Entry
		{
			get
			{
				if (entry == null)
				{
					LdapAttributeSet ldapAttributeSet = new LdapAttributeSet();
					Asn1Object[] array = ((RfcSearchResultEntry)message.Response).Attributes.toArray();
					for (int i = 0; i < array.Length; i++)
					{
						Asn1Sequence obj = (Asn1Sequence)array[i];
						LdapAttribute ldapAttribute = new LdapAttribute(((Asn1OctetString)obj.get_Renamed(0)).stringValue());
						object[] array2 = ((Asn1Set)obj.get_Renamed(1)).toArray();
						for (int j = 0; j < array2.Length; j++)
						{
							ldapAttribute.addValue(((Asn1OctetString)array2[j]).byteValue());
						}
						ldapAttributeSet.Add(ldapAttribute);
					}
					entry = new LdapEntry(((RfcSearchResultEntry)message.Response).ObjectName.stringValue(), ldapAttributeSet);
				}
				return entry;
			}
		}

		internal LdapSearchResult(RfcLdapMessage message)
			: base(message)
		{
		}

		public LdapSearchResult(LdapEntry entry, LdapControl[] cont)
		{
			if (entry == null)
			{
				throw new ArgumentException("Argument \"entry\" cannot be null");
			}
			this.entry = entry;
		}

		public override string ToString()
		{
			if (entry == null)
			{
				return base.ToString();
			}
			return entry.ToString();
		}
	}
}

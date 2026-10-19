using System.Collections;
using Novell.Directory.Ldap.Asn1;
using Novell.Directory.Ldap.Rfc2251;

namespace Novell.Directory.Ldap
{
	public class LdapAddRequest : LdapMessage
	{
		public virtual LdapEntry Entry
		{
			get
			{
				RfcAddRequest obj = (RfcAddRequest)Asn1Object.getRequest();
				LdapAttributeSet ldapAttributeSet = new LdapAttributeSet();
				Asn1Object[] array = obj.Attributes.toArray();
				for (int i = 0; i < array.Length; i++)
				{
					RfcAttributeTypeAndValues obj2 = (RfcAttributeTypeAndValues)array[i];
					LdapAttribute ldapAttribute = new LdapAttribute(((Asn1OctetString)obj2.get_Renamed(0)).stringValue());
					object[] array2 = ((Asn1SetOf)obj2.get_Renamed(1)).toArray();
					for (int j = 0; j < array2.Length; j++)
					{
						ldapAttribute.addValue(((Asn1OctetString)array2[j]).byteValue());
					}
					ldapAttributeSet.Add(ldapAttribute);
				}
				return new LdapEntry(Asn1Object.RequestDN, ldapAttributeSet);
			}
		}

		public LdapAddRequest(LdapEntry entry, LdapControl[] cont)
			: base(8, new RfcAddRequest(new RfcLdapDN(entry.DN), makeRfcAttrList(entry)), cont)
		{
		}

		private static RfcAttributeList makeRfcAttrList(LdapEntry entry)
		{
			LdapAttributeSet attributeSet = entry.getAttributeSet();
			RfcAttributeList rfcAttributeList = new RfcAttributeList(attributeSet.Count);
			IEnumerator enumerator = attributeSet.GetEnumerator();
			while (enumerator.MoveNext())
			{
				LdapAttribute ldapAttribute = (LdapAttribute)enumerator.Current;
				Asn1SetOf asn1SetOf = new Asn1SetOf(ldapAttribute.size());
				IEnumerator byteValues = ldapAttribute.ByteValues;
				while (byteValues.MoveNext())
				{
					asn1SetOf.add(new RfcAttributeValue((sbyte[])byteValues.Current));
				}
				rfcAttributeList.add(new RfcAttributeTypeAndValues(new RfcAttributeDescription(ldapAttribute.Name), asn1SetOf));
			}
			return rfcAttributeList;
		}

		public override string ToString()
		{
			return Asn1Object.ToString();
		}
	}
}

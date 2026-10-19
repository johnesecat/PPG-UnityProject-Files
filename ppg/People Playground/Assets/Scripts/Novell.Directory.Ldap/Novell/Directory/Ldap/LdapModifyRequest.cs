using System;
using System.Collections;
using Novell.Directory.Ldap.Asn1;
using Novell.Directory.Ldap.Rfc2251;

namespace Novell.Directory.Ldap
{
	public class LdapModifyRequest : LdapMessage
	{
		public virtual string DN => Asn1Object.RequestDN;

		public virtual LdapModification[] Modifications
		{
			get
			{
				Asn1Object[] array = ((RfcModifyRequest)Asn1Object.getRequest()).Modifications.toArray();
				LdapModification[] array2 = new LdapModification[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					Asn1Sequence asn1Sequence = (Asn1Sequence)array[i];
					if (asn1Sequence.size() != 2)
					{
						throw new SystemException("LdapModifyRequest: modification " + i + " is wrong size: " + asn1Sequence.size());
					}
					Asn1Object[] array3 = asn1Sequence.toArray();
					int op = ((Asn1Enumerated)array3[0]).intValue();
					Asn1Object[] array4 = ((Asn1Sequence)array3[1]).toArray();
					string attrName = ((RfcAttributeDescription)array4[0]).stringValue();
					Asn1Object[] array5 = ((Asn1SetOf)array4[1]).toArray();
					LdapAttribute ldapAttribute = new LdapAttribute(attrName);
					for (int j = 0; j < array5.Length; j++)
					{
						RfcAttributeValue rfcAttributeValue = (RfcAttributeValue)array5[j];
						ldapAttribute.addValue(rfcAttributeValue.byteValue());
					}
					array2[i] = new LdapModification(op, ldapAttribute);
				}
				return array2;
			}
		}

		public LdapModifyRequest(string dn, LdapModification[] mods, LdapControl[] cont)
			: base(6, new RfcModifyRequest(new RfcLdapDN(dn), encodeModifications(mods)), cont)
		{
		}

		private static Asn1SequenceOf encodeModifications(LdapModification[] mods)
		{
			Asn1SequenceOf asn1SequenceOf = new Asn1SequenceOf(mods.Length);
			for (int i = 0; i < mods.Length; i++)
			{
				LdapAttribute attribute = mods[i].Attribute;
				Asn1SetOf asn1SetOf = new Asn1SetOf(attribute.size());
				if (attribute.size() > 0)
				{
					IEnumerator byteValues = attribute.ByteValues;
					while (byteValues.MoveNext())
					{
						asn1SetOf.add(new RfcAttributeValue((sbyte[])byteValues.Current));
					}
				}
				Asn1Sequence asn1Sequence = new Asn1Sequence(2);
				asn1Sequence.add(new Asn1Enumerated(mods[i].Op));
				asn1Sequence.add(new RfcAttributeTypeAndValues(new RfcAttributeDescription(attribute.Name), asn1SetOf));
				asn1SequenceOf.add(asn1Sequence);
			}
			return asn1SequenceOf;
		}

		public override string ToString()
		{
			return Asn1Object.ToString();
		}
	}
}

using System;
using System.Collections;
using System.Text;

namespace Novell.Directory.Ldap
{
	public class LdapAttributeSet : SupportClass.AbstractSetSupport, ICloneable
	{
		private Hashtable map;

		public override int Count => map.Count;

		public LdapAttributeSet()
		{
			map = new Hashtable();
		}

		public override object Clone()
		{
			try
			{
				object obj = MemberwiseClone();
				IEnumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					((LdapAttributeSet)obj).Add(((LdapAttribute)enumerator.Current).Clone());
				}
				return obj;
			}
			catch (Exception)
			{
				throw new SystemException("Internal error, cannot create clone");
			}
		}

		public virtual LdapAttribute getAttribute(string attrName)
		{
			return (LdapAttribute)map[attrName.ToUpper()];
		}

		public virtual LdapAttribute getAttribute(string attrName, string lang)
		{
			string text = attrName + ";" + lang;
			return (LdapAttribute)map[text.ToUpper()];
		}

		public virtual LdapAttributeSet getSubset(string subtype)
		{
			LdapAttributeSet ldapAttributeSet = new LdapAttributeSet();
			IEnumerator enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				LdapAttribute ldapAttribute = (LdapAttribute)enumerator.Current;
				if (ldapAttribute.hasSubtype(subtype))
				{
					ldapAttributeSet.Add(ldapAttribute.Clone());
				}
			}
			return ldapAttributeSet;
		}

		public override IEnumerator GetEnumerator()
		{
			return map.Values.GetEnumerator();
		}

		public override bool IsEmpty()
		{
			return map.Count == 0;
		}

		public override bool Contains(object attr)
		{
			LdapAttribute ldapAttribute = (LdapAttribute)attr;
			return map.ContainsKey(ldapAttribute.Name.ToUpper());
		}

		public override bool Add(object attr)
		{
			LdapAttribute ldapAttribute = (LdapAttribute)attr;
			string key = ldapAttribute.Name.ToUpper();
			if (map.ContainsKey(key))
			{
				return false;
			}
			SupportClass.PutElement(map, key, ldapAttribute);
			return true;
		}

		public override bool Remove(object object_Renamed)
		{
			string text = ((!(object_Renamed is string)) ? ((LdapAttribute)object_Renamed).Name : ((string)object_Renamed));
			if (text == null)
			{
				return false;
			}
			return SupportClass.HashtableRemove(map, text.ToUpper()) != null;
		}

		public override void Clear()
		{
			map.Clear();
		}

		public override bool AddAll(ICollection c)
		{
			bool result = false;
			IEnumerator enumerator = c.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (Add(enumerator.Current))
				{
					result = true;
				}
			}
			return result;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("LdapAttributeSet: ");
			IEnumerator enumerator = GetEnumerator();
			bool flag = true;
			while (enumerator.MoveNext())
			{
				if (!flag)
				{
					stringBuilder.Append(" ");
				}
				flag = false;
				LdapAttribute ldapAttribute = (LdapAttribute)enumerator.Current;
				stringBuilder.Append(ldapAttribute.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}

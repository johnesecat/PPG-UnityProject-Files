using System;
using System.Text;

namespace Novell.Directory.Ldap
{
	public class LdapEntry : IComparable
	{
		protected internal string dn;

		protected internal LdapAttributeSet attrs;

		[CLSCompliant(false)]
		public virtual string DN => dn;

		public LdapEntry()
			: this(null, null)
		{
		}

		public LdapEntry(string dn)
			: this(dn, null)
		{
		}

		public LdapEntry(string dn, LdapAttributeSet attrs)
		{
			if (dn == null)
			{
				dn = "";
			}
			if (attrs == null)
			{
				attrs = new LdapAttributeSet();
			}
			this.dn = dn;
			this.attrs = attrs;
		}

		public virtual LdapAttribute getAttribute(string attrName)
		{
			return attrs.getAttribute(attrName);
		}

		public virtual LdapAttributeSet getAttributeSet()
		{
			return attrs;
		}

		public virtual LdapAttributeSet getAttributeSet(string subtype)
		{
			return attrs.getSubset(subtype);
		}

		public virtual int CompareTo(object entry)
		{
			return LdapDN.normalize(dn).CompareTo(LdapDN.normalize(((LdapEntry)entry).dn));
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("LdapEntry: ");
			if (dn != null)
			{
				stringBuilder.Append(dn + "; ");
			}
			if (attrs != null)
			{
				stringBuilder.Append(attrs.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}

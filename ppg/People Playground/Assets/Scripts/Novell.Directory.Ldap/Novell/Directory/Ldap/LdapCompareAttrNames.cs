using System.Collections;
using System.Globalization;

namespace Novell.Directory.Ldap
{
	public class LdapCompareAttrNames : IComparer
	{
		private string[] sortByNames;

		private bool[] sortAscending;

		private CultureInfo location;

		private CompareInfo collator;

		public virtual CultureInfo Locale
		{
			get
			{
				return location;
			}
			set
			{
				collator = value.CompareInfo;
				location = value;
			}
		}

		private void InitBlock()
		{
			location = CultureInfo.CurrentCulture;
			collator = CultureInfo.CurrentCulture.CompareInfo;
		}

		public LdapCompareAttrNames(string attrName)
		{
			InitBlock();
			sortByNames = new string[1];
			sortByNames[0] = attrName;
			sortAscending = new bool[1];
			sortAscending[0] = true;
		}

		public LdapCompareAttrNames(string attrName, bool ascendingFlag)
		{
			InitBlock();
			sortByNames = new string[1];
			sortByNames[0] = attrName;
			sortAscending = new bool[1];
			sortAscending[0] = ascendingFlag;
		}

		public LdapCompareAttrNames(string[] attrNames)
		{
			InitBlock();
			sortByNames = new string[attrNames.Length];
			sortAscending = new bool[attrNames.Length];
			for (int i = 0; i < attrNames.Length; i++)
			{
				sortByNames[i] = attrNames[i];
				sortAscending[i] = true;
			}
		}

		public LdapCompareAttrNames(string[] attrNames, bool[] ascendingFlags)
		{
			InitBlock();
			if (attrNames.Length != ascendingFlags.Length)
			{
				throw new LdapException("UNEQUAL_LENGTHS", 18, null);
			}
			sortByNames = new string[attrNames.Length];
			sortAscending = new bool[ascendingFlags.Length];
			for (int i = 0; i < attrNames.Length; i++)
			{
				sortByNames[i] = attrNames[i];
				sortAscending[i] = ascendingFlags[i];
			}
		}

		public virtual int Compare(object object1, object object2)
		{
			LdapEntry ldapEntry = (LdapEntry)object1;
			LdapEntry ldapEntry2 = (LdapEntry)object2;
			int num = 0;
			if (collator == null)
			{
				collator = CultureInfo.CurrentCulture.CompareInfo;
			}
			int num2;
			do
			{
				LdapAttribute attribute = ldapEntry.getAttribute(sortByNames[num]);
				LdapAttribute attribute2 = ldapEntry2.getAttribute(sortByNames[num]);
				if (attribute == null || attribute2 == null)
				{
					num2 = ((attribute != null) ? (-1) : ((attribute2 != null) ? 1 : 0));
				}
				else
				{
					string[] stringValueArray = attribute.StringValueArray;
					string[] stringValueArray2 = attribute2.StringValueArray;
					num2 = collator.Compare(stringValueArray[0], stringValueArray2[0]);
				}
				num++;
			}
			while (num2 == 0 && num < sortByNames.Length);
			if (sortAscending[num - 1])
			{
				return num2;
			}
			return -num2;
		}

		public override bool Equals(object comparator)
		{
			if (!(comparator is LdapCompareAttrNames))
			{
				return false;
			}
			LdapCompareAttrNames ldapCompareAttrNames = (LdapCompareAttrNames)comparator;
			if (ldapCompareAttrNames.sortByNames.Length != sortByNames.Length || ldapCompareAttrNames.sortAscending.Length != sortAscending.Length)
			{
				return false;
			}
			for (int i = 0; i < sortByNames.Length; i++)
			{
				if (ldapCompareAttrNames.sortAscending[i] != sortAscending[i])
				{
					return false;
				}
				if (!ldapCompareAttrNames.sortByNames[i].ToUpper().Equals(sortByNames[i].ToUpper()))
				{
					return false;
				}
			}
			return true;
		}
	}
}

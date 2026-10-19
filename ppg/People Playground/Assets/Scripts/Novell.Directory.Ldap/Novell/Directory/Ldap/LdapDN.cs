using System;
using System.Text;
using Novell.Directory.Ldap.Utilclass;

namespace Novell.Directory.Ldap
{
	public class LdapDN
	{
		[CLSCompliant(false)]
		public static bool equals(string dn1, string dn2)
		{
			DN dN = new DN(dn1);
			DN toDN = new DN(dn2);
			return dN.Equals(toDN);
		}

		public static string escapeRDN(string rdn)
		{
			StringBuilder stringBuilder = new StringBuilder(rdn);
			int i;
			for (i = 0; i < stringBuilder.Length && stringBuilder[i] != '='; i++)
			{
			}
			if (i == stringBuilder.Length)
			{
				throw new ArgumentException("Could not parse RDN: Attribute type and name must be separated by an equal symbol, '='");
			}
			i++;
			if (stringBuilder[i] == ' ' || stringBuilder[i] == '#')
			{
				stringBuilder.Insert(i++, '\\');
			}
			for (; i < stringBuilder.Length; i++)
			{
				if (stringBuilder[i] == ',' || stringBuilder[i] == '+' || stringBuilder[i] == '"' || stringBuilder[i] == '\\' || stringBuilder[i] == '<' || stringBuilder[i] == '>' || stringBuilder[i] == ';')
				{
					stringBuilder.Insert(i++, '\\');
				}
			}
			if (stringBuilder[stringBuilder.Length - 1] == ' ')
			{
				stringBuilder.Insert(stringBuilder.Length - 1, '\\');
			}
			return stringBuilder.ToString();
		}

		public static string[] explodeDN(string dn, bool noTypes)
		{
			return new DN(dn).explodeDN(noTypes);
		}

		public static string[] explodeRDN(string rdn, bool noTypes)
		{
			return new RDN(rdn).explodeRDN(noTypes);
		}

		public static bool isValid(string dn)
		{
			try
			{
				new DN(dn);
			}
			catch (ArgumentException)
			{
				return false;
			}
			return true;
		}

		public static string normalize(string dn)
		{
			return new DN(dn).ToString();
		}

		public static string unescapeRDN(string rdn)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int i;
			for (i = 0; i < rdn.Length && rdn[i] != '='; i++)
			{
			}
			if (i == rdn.Length)
			{
				throw new ArgumentException("Could not parse rdn: Attribute type and name must be separated by an equal symbol, '='");
			}
			i++;
			if (rdn[i] == '\\' && i + 1 < rdn.Length - 1 && (rdn[i + 1] == ' ' || rdn[i + 1] == '#'))
			{
				i++;
			}
			for (; i < rdn.Length; i++)
			{
				if (rdn[i] != '\\' || i == rdn.Length - 1 || (rdn[i + 1] != ',' && rdn[i + 1] != '+' && rdn[i + 1] != '"' && rdn[i + 1] != '\\' && rdn[i + 1] != '<' && rdn[i + 1] != '>' && rdn[i + 1] != ';' && (rdn[i + 1] != ' ' || i + 2 != rdn.Length)))
				{
					stringBuilder.Append(rdn[i]);
				}
			}
			return stringBuilder.ToString();
		}
	}
}

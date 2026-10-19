using System.IO;
using System.Text;
using Novell.Directory.Ldap.Utilclass;

namespace Novell.Directory.Ldap
{
	public class LdapMatchingRuleUseSchema : LdapSchemaElement
	{
		private string[] attributes;

		public virtual string[] Attributes => attributes;

		public LdapMatchingRuleUseSchema(string[] names, string oid, string description, bool obsolete, string[] attributes)
			: base(LdapSchema.schemaTypeNames[7])
		{
			base.names = new string[names.Length];
			names.CopyTo(base.names, 0);
			base.oid = oid;
			base.description = description;
			base.obsolete = obsolete;
			this.attributes = new string[attributes.Length];
			attributes.CopyTo(this.attributes, 0);
			base.Value = formatString();
		}

		public LdapMatchingRuleUseSchema(string raw)
			: base(LdapSchema.schemaTypeNames[7])
		{
			try
			{
				SchemaParser schemaParser = new SchemaParser(raw);
				names = new string[schemaParser.Names.Length];
				schemaParser.Names.CopyTo(names, 0);
				oid = schemaParser.ID;
				description = schemaParser.Description;
				obsolete = schemaParser.Obsolete;
				attributes = schemaParser.Applies;
				base.Value = formatString();
			}
			catch (IOException)
			{
			}
		}

		protected internal override string formatString()
		{
			StringBuilder stringBuilder = new StringBuilder("( ");
			string iD;
			if ((iD = ID) != null)
			{
				stringBuilder.Append(iD);
			}
			string[] array = Names;
			if (array != null)
			{
				stringBuilder.Append(" NAME ");
				if (array.Length == 1)
				{
					stringBuilder.Append("'" + array[0] + "'");
				}
				else
				{
					stringBuilder.Append("( ");
					for (int i = 0; i < array.Length; i++)
					{
						stringBuilder.Append(" '" + array[i] + "'");
					}
					stringBuilder.Append(" )");
				}
			}
			if ((iD = Description) != null)
			{
				stringBuilder.Append(" DESC ");
				stringBuilder.Append("'" + iD + "'");
			}
			if (Obsolete)
			{
				stringBuilder.Append(" OBSOLETE");
			}
			if ((array = Attributes) != null)
			{
				stringBuilder.Append(" APPLIES ");
				if (array.Length > 1)
				{
					stringBuilder.Append("( ");
				}
				for (int j = 0; j < array.Length; j++)
				{
					if (j > 0)
					{
						stringBuilder.Append(" $ ");
					}
					stringBuilder.Append(array[j]);
				}
				if (array.Length > 1)
				{
					stringBuilder.Append(" )");
				}
			}
			stringBuilder.Append(" )");
			return stringBuilder.ToString();
		}
	}
}

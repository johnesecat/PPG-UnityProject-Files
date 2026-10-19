using System.IO;
using System.Text;
using Novell.Directory.Ldap.Utilclass;

namespace Novell.Directory.Ldap
{
	public class LdapMatchingRuleSchema : LdapSchemaElement
	{
		private string syntaxString;

		private string[] attributes;

		public virtual string[] Attributes => attributes;

		public virtual string SyntaxString => syntaxString;

		public LdapMatchingRuleSchema(string[] names, string oid, string description, string[] attributes, bool obsolete, string syntaxString)
			: base(LdapSchema.schemaTypeNames[6])
		{
			base.names = new string[names.Length];
			names.CopyTo(base.names, 0);
			base.oid = oid;
			base.description = description;
			base.obsolete = obsolete;
			this.attributes = new string[attributes.Length];
			attributes.CopyTo(this.attributes, 0);
			this.syntaxString = syntaxString;
			base.Value = formatString();
		}

		public LdapMatchingRuleSchema(string rawMatchingRule, string rawMatchingRuleUse)
			: base(LdapSchema.schemaTypeNames[6])
		{
			try
			{
				SchemaParser schemaParser = new SchemaParser(rawMatchingRule);
				names = new string[schemaParser.Names.Length];
				schemaParser.Names.CopyTo(names, 0);
				oid = schemaParser.ID;
				description = schemaParser.Description;
				obsolete = schemaParser.Obsolete;
				syntaxString = schemaParser.Syntax;
				if (rawMatchingRuleUse != null)
				{
					SchemaParser schemaParser2 = new SchemaParser(rawMatchingRuleUse);
					attributes = schemaParser2.Applies;
				}
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
			if ((iD = SyntaxString) != null)
			{
				stringBuilder.Append(" SYNTAX ");
				stringBuilder.Append(iD);
			}
			stringBuilder.Append(" )");
			return stringBuilder.ToString();
		}
	}
}

using System.Text;

namespace Novell.Directory.Ldap.Events
{
	public class LdapEventArgs : DirectoryEventArgs
	{
		protected LdapEventType eType;

		public LdapEventType EventType
		{
			get
			{
				return eType;
			}
			set
			{
				eType = value;
			}
		}

		public LdapEventArgs(LdapMessage sourceMessage, EventClassifiers aClassification, LdapEventType aType)
			: base(sourceMessage, aClassification)
		{
			eType = aType;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[");
			stringBuilder.AppendFormat("{0}:", GetType());
			stringBuilder.AppendFormat("(Classification={0})", eClassification);
			stringBuilder.AppendFormat("(Type={0})", eType);
			stringBuilder.AppendFormat("(EventInformation:{0})", ldap_message);
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}
	}
}

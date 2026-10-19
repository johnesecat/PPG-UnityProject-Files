namespace Novell.Directory.Ldap.Events
{
	public class DirectoryEventArgs : BaseEventArgs
	{
		protected EventClassifiers eClassification;

		public EventClassifiers EventClassification
		{
			get
			{
				return eClassification;
			}
			set
			{
				eClassification = value;
			}
		}

		public DirectoryEventArgs(LdapMessage sourceMessage, EventClassifiers aClassification)
			: base(sourceMessage)
		{
			eClassification = aClassification;
		}
	}
}

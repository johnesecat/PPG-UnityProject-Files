namespace Novell.Directory.Ldap.Events.Edir
{
	public class EdirEventArgs : DirectoryEventArgs
	{
		public EdirEventIntermediateResponse IntermediateResponse
		{
			get
			{
				if (ldap_message is EdirEventIntermediateResponse)
				{
					return (EdirEventIntermediateResponse)ldap_message;
				}
				return null;
			}
		}

		public EdirEventArgs(LdapMessage sourceMessage, EventClassifiers aClassification)
			: base(sourceMessage, aClassification)
		{
		}
	}
}

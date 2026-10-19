using System.Runtime.CompilerServices;

namespace WatsonTcp
{
	public class AuthenticationFailedEventArgs
	{
		public string IpPort
		{
			[CompilerGenerated]
			get
			{
				return <IpPort>k__BackingField;
			}
		}

		internal AuthenticationFailedEventArgs(string ipPort)
		{
			<IpPort>k__BackingField = ipPort;
		}
	}
}

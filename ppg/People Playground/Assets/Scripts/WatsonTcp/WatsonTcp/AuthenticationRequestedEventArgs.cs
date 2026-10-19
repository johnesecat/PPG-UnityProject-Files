using System.Runtime.CompilerServices;

namespace WatsonTcp
{
	public class AuthenticationRequestedEventArgs
	{
		public string IpPort
		{
			[CompilerGenerated]
			get
			{
				return <IpPort>k__BackingField;
			}
		}

		internal AuthenticationRequestedEventArgs(string ipPort)
		{
			<IpPort>k__BackingField = ipPort;
		}
	}
}

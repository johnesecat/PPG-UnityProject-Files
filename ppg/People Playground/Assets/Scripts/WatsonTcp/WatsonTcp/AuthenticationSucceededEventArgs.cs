using System.Runtime.CompilerServices;

namespace WatsonTcp
{
	public class AuthenticationSucceededEventArgs
	{
		public string IpPort
		{
			[CompilerGenerated]
			get
			{
				return <IpPort>k__BackingField;
			}
		}

		internal AuthenticationSucceededEventArgs(string ipPort)
		{
			<IpPort>k__BackingField = ipPort;
		}
	}
}

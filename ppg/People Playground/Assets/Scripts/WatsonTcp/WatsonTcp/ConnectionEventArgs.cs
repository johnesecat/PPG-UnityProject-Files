using System;
using System.Runtime.CompilerServices;

namespace WatsonTcp
{
	public class ConnectionEventArgs : EventArgs
	{
		public string IpPort
		{
			[CompilerGenerated]
			get
			{
				return <IpPort>k__BackingField;
			}
		}

		internal ConnectionEventArgs(string ipPort)
			: this()
		{
			<IpPort>k__BackingField = ipPort;
		}
	}
}

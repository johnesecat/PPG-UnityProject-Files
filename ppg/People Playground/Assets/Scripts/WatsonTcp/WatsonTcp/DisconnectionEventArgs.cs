using System;
using System.Runtime.CompilerServices;

namespace WatsonTcp
{
	public class DisconnectionEventArgs : EventArgs
	{
		public string IpPort
		{
			[CompilerGenerated]
			get
			{
				return <IpPort>k__BackingField;
			}
		}

		public DisconnectReason Reason
		{
			[CompilerGenerated]
			get
			{
				return <Reason>k__BackingField;
			}
		}

		internal DisconnectionEventArgs(string ipPort, DisconnectReason reason)
			: this()
		{
			<IpPort>k__BackingField = ipPort;
			<Reason>k__BackingField = reason;
		}
	}
}

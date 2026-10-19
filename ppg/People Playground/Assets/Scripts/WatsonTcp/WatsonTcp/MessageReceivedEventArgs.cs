using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WatsonTcp
{
	public class MessageReceivedEventArgs : EventArgs
	{
		public string IpPort
		{
			[CompilerGenerated]
			get
			{
				return <IpPort>k__BackingField;
			}
		}

		public Dictionary<object, object> Metadata
		{
			[CompilerGenerated]
			get
			{
				return <Metadata>k__BackingField;
			}
		}

		public byte[] Data
		{
			[CompilerGenerated]
			get
			{
				return <Data>k__BackingField;
			}
		}

		internal MessageReceivedEventArgs(string ipPort, Dictionary<object, object> metadata, byte[] data)
			: this()
		{
			<IpPort>k__BackingField = ipPort;
			<Metadata>k__BackingField = metadata;
			<Data>k__BackingField = data;
		}
	}
}

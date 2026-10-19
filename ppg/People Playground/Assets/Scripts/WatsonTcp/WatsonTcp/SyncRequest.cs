using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WatsonTcp
{
	public class SyncRequest
	{
		public string IpPort
		{
			[CompilerGenerated]
			get
			{
				return <IpPort>k__BackingField;
			}
		}

		public global::System.DateTime ExpirationUtc
		{
			[CompilerGenerated]
			get
			{
				return <ExpirationUtc>k__BackingField;
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

		internal string ConversationGuid
		{
			[CompilerGenerated]
			get
			{
				return <ConversationGuid>k__BackingField;
			}
		}

		internal SyncRequest(string ipPort, string convGuid, global::System.DateTime expirationUtc, Dictionary<object, object> metadata, byte[] data)
		{
			<IpPort>k__BackingField = ipPort;
			<ConversationGuid>k__BackingField = convGuid;
			<ExpirationUtc>k__BackingField = expirationUtc;
			<Metadata>k__BackingField = metadata;
			if (data != null)
			{
				<Data>k__BackingField = new byte[data.Length];
				Buffer.BlockCopy((global::System.Array)data, 0, (global::System.Array)Data, 0, data.Length);
			}
		}
	}
}

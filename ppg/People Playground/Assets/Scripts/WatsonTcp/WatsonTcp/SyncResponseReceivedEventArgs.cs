using System.Runtime.CompilerServices;

namespace WatsonTcp
{
	internal class SyncResponseReceivedEventArgs
	{
		public WatsonMessage Message
		{
			[CompilerGenerated]
			get
			{
				return <Message>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<Message>k__BackingField = value;
			}
		}

		public byte[] Data
		{
			[CompilerGenerated]
			get
			{
				return <Data>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<Data>k__BackingField = value;
			}
		}

		public SyncResponseReceivedEventArgs(WatsonMessage msg, byte[] data)
		{
			Message = msg;
			Data = data;
		}
	}
}

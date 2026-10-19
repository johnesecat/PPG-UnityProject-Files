using System;
using System.Runtime.CompilerServices;

namespace Activations
{
	[Serializable]
	public struct ActivationEvent
	{
		public readonly int Depth;

		public readonly Channel Channels;

		public readonly ActivationEventType EventType;

		public readonly ActivationSource Source;

		public ActivationEvent(int depth, Channel channels, ActivationEventType eventType, ActivationSource source = ActivationSource.Node)
		{
			Depth = depth;
			Channels = channels;
			EventType = eventType;
			Source = source;
		}

		[IsReadOnly]
		public ActivationEvent GetBranch()
		{
			return new ActivationEvent(Depth + 1, Channels, EventType);
		}
	}
}

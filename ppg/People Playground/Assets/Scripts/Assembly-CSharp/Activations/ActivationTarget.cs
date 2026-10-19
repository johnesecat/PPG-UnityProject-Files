using System;

namespace Activations
{
	[Serializable]
	public class ActivationTarget
	{
		public readonly NodeBehaviour Node;

		public readonly Channel Channels;

		public ActivationTarget(NodeBehaviour node, Channel channelMask)
		{
			Node = node;
			Channels = channelMask;
		}

		public override string ToString()
		{
			return $"To {Node} through channels {Channels}";
		}
	}
}

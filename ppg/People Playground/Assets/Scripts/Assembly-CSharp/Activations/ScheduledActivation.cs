using System;
using System.Runtime.CompilerServices;

namespace Activations
{
	[Serializable]
	public struct ScheduledActivation
	{
		public float TimeRemaining;

		public readonly ActivationEvent Activation;

		public ScheduledActivation(float timeRemaining, in ActivationEvent activation)
		{
			TimeRemaining = timeRemaining;
			Activation = activation;
		}

		[IsReadOnly]
		public bool HasExpired()
		{
			return TimeRemaining < float.Epsilon;
		}

		public static bool HasExpiredPredicate(ScheduledActivation s)
		{
			return s.HasExpired();
		}
	}
}

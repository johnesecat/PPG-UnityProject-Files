using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Activations
{
	[DisallowMultipleComponent]
	public class NodeBehaviour : MonoBehaviour, Messages.IUse, Messages.IOnBeforeSerialise, Messages.IOnAfterDeserialise
	{
		[Serializable]
		public struct SerialisableEvent
		{
			public float RemainingTime;

			public Channel ActivationChannels;

			public int ActivationDepth;

			public ActivationEventType EventType;
		}

		public float DelaySeconds = 0.1f;

		[SkipSerialisation]
		public List<ActivationTarget> Targets = new List<ActivationTarget>();

		[SkipSerialisation]
		public UnityEvent<Channel> OnUseStart = new UnityEvent<Channel>();

		[SkipSerialisation]
		public UnityEvent<Channel> OnUseEnd = new UnityEvent<Channel>();

		[SkipSerialisation]
		public Channel CurrentlyActive;

		[SkipSerialisation]
		public UnityEvent<Channel> OnEmit = new UnityEvent<Channel>();

		[HideInInspector]
		public Channel Started;

		[HideInInspector]
		public Channel Ended;

		private readonly List<ScheduledActivation> scheduledActivations = new List<ScheduledActivation>();

		private int emittedThisFrame;

		public static int MaxSignalsPerFrame = 512;

		[HideInInspector]
		public SerialisableEvent[] SerialisableState;

		public void Use(ActivationPropagation activation)
		{
		}

		public void OnBeforeSerialise()
		{
		}

		public void OnAfterDeserialise(List<GameObject> gameObjects)
		{
		}
	}
}

using System;
using UnityEngine;
using UnityEngine.Events;

public class DoAtTime : MonoBehaviour
{
	[Serializable]
	public class Event
	{
		public string Name;

		public float Time;

		public UnityEvent Actions;

		[NonSerialized]
		public bool Passed;
	}

	public Event[] Events;

	private float time;

	private void Update()
	{
		if (!base.enabled)
		{
			return;
		}
		time += Time.deltaTime;
		Event[] events = Events;
		foreach (Event @event in events)
		{
			if (!@event.Passed && time > @event.Time)
			{
				@event.Passed = true;
				@event.Actions?.Invoke();
			}
		}
	}
}

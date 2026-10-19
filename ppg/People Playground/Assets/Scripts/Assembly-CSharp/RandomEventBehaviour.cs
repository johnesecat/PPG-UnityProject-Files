using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RandomEventBehaviour : MonoBehaviour
{
	[Serializable]
	public class RandomEvent
	{
		public UnityEvent Actions = new UnityEvent();

		public float Delay;
	}

	[SkipSerialisation]
	public List<RandomEvent> Events = new List<RandomEvent>();

	[SkipSerialisation]
	public float ChancePerSecond;

	[SkipSerialisation]
	public float Cooldown;

	private float lastDispatchTime;

	private void Update()
	{
		if (Time.time - lastDispatchTime <= Cooldown)
		{
			return;
		}
		float num = Time.deltaTime * ChancePerSecond;
		if (UnityEngine.Random.value > num)
		{
			return;
		}
		lastDispatchTime = Time.time;
		foreach (RandomEvent item in Events)
		{
			if (item.Actions == null)
			{
				continue;
			}
			if (item.Delay < float.Epsilon)
			{
				item.Actions.Invoke();
				continue;
			}
			StartCoroutine(Utils.DelayCoroutine(item.Delay, delegate
			{
				if (base.enabled)
				{
					item.Actions.Invoke();
				}
			}));
		}
	}
}

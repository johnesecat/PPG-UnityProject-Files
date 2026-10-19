using System;
using UnityEngine;
using UnityEngine.Events;

public class UseEventTrigger : MonoBehaviour, Messages.IUse
{
	[SkipSerialisation]
	public UnityEvent Event = new UnityEvent();

	[SkipSerialisation]
	public bool OnlyOnce;

	[SkipSerialisation]
	public Action Action;

	private int useCount;

	public void Use(ActivationPropagation activation)
	{
		if ((!OnlyOnce || useCount <= 0) && base.enabled)
		{
			useCount++;
			Event.Invoke();
			Action?.Invoke();
		}
	}
}

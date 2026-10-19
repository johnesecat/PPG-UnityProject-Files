using System;
using UnityEngine.EventSystems;

namespace ImmersiveVRTools.Runtime.Common.Extensions
{
	public static class EventTriggerExtensions
	{
		public static void AddListener(this EventTrigger trigger, EventTriggerType type, Action<PointerEventData> callback)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry
			{
				eventID = type
			};
			entry.callback.AddListener(delegate(BaseEventData data)
			{
				callback((PointerEventData)data);
			});
			trigger.triggers.Add(entry);
		}
	}
}

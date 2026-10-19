using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnPointerUpEvent : MonoBehaviour, IPointerUpHandler, IEventSystemHandler
{
	public UnityEvent onPointerUp = new UnityEvent();

	public void OnPointerUp(PointerEventData eventData)
	{
		onPointerUp?.Invoke();
	}
}

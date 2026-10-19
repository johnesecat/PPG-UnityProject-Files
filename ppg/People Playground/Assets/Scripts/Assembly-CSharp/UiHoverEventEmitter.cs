using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UiHoverEventEmitter : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public UnityEvent OnMouseEnter = new UnityEvent();

	public UnityEvent OnMouseExit = new UnityEvent();

	public void OnPointerEnter(PointerEventData eventData)
	{
		OnMouseEnter?.Invoke();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		OnMouseExit?.Invoke();
	}
}

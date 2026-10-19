using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UiEventEmitter : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
	public UnityEvent OnMouseEnter = new UnityEvent();

	public UnityEvent OnMouseExit = new UnityEvent();

	public UnityEvent OnMouseDown = new UnityEvent();

	public UnityEvent OnMouseUp = new UnityEvent();

	public UnityEvent OnMouseClick = new UnityEvent();

	public void OnPointerClick(PointerEventData eventData)
	{
		OnMouseClick.Invoke();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		OnMouseDown.Invoke();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		OnMouseEnter.Invoke();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		OnMouseExit.Invoke();
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		OnMouseUp.Invoke();
	}
}

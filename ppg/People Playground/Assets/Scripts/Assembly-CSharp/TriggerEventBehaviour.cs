using UnityEngine;
using UnityEngine.Events;

public class TriggerEventBehaviour : MonoBehaviour
{
	public UnityAction<Collider2D> OnTriggerExit;

	public UnityAction<Collider2D> OnTriggerEnter;

	private void OnTriggerExit2D(Collider2D collision)
	{
		OnTriggerExit?.Invoke(collision);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		OnTriggerEnter?.Invoke(collision);
	}
}

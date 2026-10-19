using UnityEngine;
using UnityEngine.Events;

public abstract class EventDataTypeConverterBase<T> : MonoBehaviour
{
	public UnityEvent<T> Converted;

	public abstract void Convert(object convertValue);
}

using UnityEngine;
using UnityEngine.Events;

public class ActOnUserDelete : MonoBehaviour, Messages.IOnUserDelete
{
	[SkipSerialisation]
	public UnityEvent Event;

	[SkipSerialisation]
	public GameObject[] DestroyWith;

	public void OnUserDelete()
	{
		if (DestroyWith != null)
		{
			GameObject[] destroyWith = DestroyWith;
			foreach (GameObject gameObject in destroyWith)
			{
				if ((bool)gameObject)
				{
					Object.Destroy(gameObject);
				}
			}
		}
		Event?.Invoke();
	}
}

using UnityEngine;
using UnityEngine.Events;

public class ActOnDestroy : MonoBehaviour
{
	public UnityEvent Event = new UnityEvent();

	[SkipSerialisation]
	public GameObject[] DestroyWith;

	private void OnDestroy()
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

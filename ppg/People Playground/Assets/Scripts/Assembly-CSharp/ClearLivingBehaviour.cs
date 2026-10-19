using UnityEngine;

public class ClearLivingBehaviour : MonoBehaviour
{
	public void Clear()
	{
		AliveBehaviour[] array = Object.FindObjectsOfType<AliveBehaviour>();
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy(array[i].transform.root.gameObject);
		}
		NotificationControllerBehaviour.Show("Cleared living");
	}
}

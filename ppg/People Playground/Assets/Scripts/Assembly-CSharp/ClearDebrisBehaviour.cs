using UnityEngine;

public class ClearDebrisBehaviour : MonoBehaviour
{
	public LayerMask Debris;

	public void Clear()
	{
		DecalControllerBehaviour[] array = Object.FindObjectsOfType<DecalControllerBehaviour>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Clear();
		}
		DebrisComponent[] array2 = Object.FindObjectsOfType<DebrisComponent>();
		foreach (DebrisComponent obj in array2)
		{
			Object.Destroy(obj.gameObject);
			if (UndoControllerBehaviour.FindRelevantAction(obj.gameObject, out var result))
			{
				UndoControllerBehaviour.DeregisterAction(result);
			}
		}
		NotificationControllerBehaviour.Show("Cleared debris");
	}
}

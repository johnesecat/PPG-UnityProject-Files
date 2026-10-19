using UnityEngine;

public class DeregisterBehaviour : MonoBehaviour
{
	private void OnDestroy()
	{
		if (UndoControllerBehaviour.FindRelevantAction(base.gameObject, out var result))
		{
			UndoControllerBehaviour.DeregisterAction(result);
		}
	}
}

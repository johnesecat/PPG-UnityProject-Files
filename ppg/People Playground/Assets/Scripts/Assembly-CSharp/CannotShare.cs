using System.Collections.Generic;
using UnityEngine;

public class CannotShare : MonoBehaviour, Messages.IOnAfterDeserialise
{
	public SpawnableAsset Substitute;

	public void OnAfterDeserialise(List<GameObject> gameObjects)
	{
		GameObject obj = Object.Instantiate(Substitute.Prefab, base.transform.position, base.transform.rotation);
		obj.AddComponent<AudioSourceTimeScaleBehaviour>();
		obj.AddComponent<SerialiseInstructions>().OriginalSpawnableAsset = Substitute;
		UndoControllerBehaviour.RegisterAction(new ObjectCreationAction(obj));
		Object.Destroy(base.gameObject);
	}
}

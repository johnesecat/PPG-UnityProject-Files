using UnityEngine;

public class ShotMessagePropagator : MonoBehaviour, Messages.IShot
{
	public GameObject[] Targets;

	public void Shot(Shot shot)
	{
		GameObject[] targets = Targets;
		foreach (GameObject gameObject in targets)
		{
			if ((bool)gameObject)
			{
				gameObject.SendMessage("Shot", shot, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}

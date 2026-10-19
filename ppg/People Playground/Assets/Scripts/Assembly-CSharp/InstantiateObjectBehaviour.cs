using NaughtyAttributes;
using UnityEngine;

public class InstantiateObjectBehaviour : MonoBehaviour
{
	public GameObject Prefab;

	public Vector3 Offset;

	public bool InstantiateOnStart;

	public bool WorldSpace;

	public bool DestroySelf;

	public bool CopyRotation;

	public bool CopyScale;

	[ShowIf("DestroySelf")]
	public float DestroySelfDelay;

	private void Start()
	{
		if (InstantiateOnStart)
		{
			Instantiate();
		}
	}

	public void Instantiate()
	{
		Vector3 position = (WorldSpace ? Offset : (base.transform.position + Offset));
		GameObject gameObject = Object.Instantiate(Prefab, position, CopyRotation ? base.transform.rotation : Quaternion.identity);
		if (CopyScale)
		{
			gameObject.transform.localScale = base.transform.localScale;
		}
		if (DestroySelf)
		{
			Object.Destroy(base.gameObject, DestroySelfDelay);
		}
	}
}

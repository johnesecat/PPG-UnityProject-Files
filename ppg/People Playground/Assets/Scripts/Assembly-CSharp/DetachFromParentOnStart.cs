using UnityEngine;

public class DetachFromParentOnStart : MonoBehaviour
{
	public bool DestroyIfParentDestroyed;

	private void Start()
	{
		if ((bool)base.transform.parent)
		{
			if (DestroyIfParentDestroyed)
			{
				base.transform.parent.gameObject.GetOrAddComponent<ActOnDestroy>().Event.AddListener(DestroyThis);
			}
			base.transform.SetParent(null);
		}
	}

	private void DestroyThis()
	{
		Object.Destroy(base.gameObject);
	}
}

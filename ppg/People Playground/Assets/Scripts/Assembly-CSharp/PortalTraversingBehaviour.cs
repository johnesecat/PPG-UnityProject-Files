using UnityEngine;

public class PortalTraversingBehaviour : MonoBehaviour
{
	private int OriginalLayer;

	private int PortalLayer;

	private void Awake()
	{
		PortalLayer = LayerMask.NameToLayer("Portal");
		OriginalLayer = base.gameObject.layer;
	}

	public void OnEnable()
	{
		base.gameObject.layer = PortalLayer;
		Debug.Log(base.gameObject.layer);
	}

	public void OnDisable()
	{
		base.gameObject.layer = OriginalLayer;
	}
}

using System.Linq;
using UnityEngine;

public class AppearWhenMouseNear : MonoBehaviour
{
	public float MinDistance;

	public Renderer Renderer;

	public Transform Center;

	public bool ConsiderParentRenderers;

	private Camera camera;

	private Renderer[] parentRenderers;

	private void Awake()
	{
		camera = Camera.main;
		if (!Center)
		{
			Center = base.transform;
		}
	}

	private void Start()
	{
		if (ConsiderParentRenderers)
		{
			parentRenderers = GetComponentsInParent<Renderer>();
		}
	}

	private void Update()
	{
		if (ConsiderParentRenderers && parentRenderers.Any((Renderer r) => r.gameObject != base.gameObject && !r.enabled))
		{
			Renderer.enabled = false;
			return;
		}
		Vector3 position = Center.position;
		Vector2 b = camera.WorldToScreenPoint(new Vector3(position.x, position.y, camera.transform.position.z));
		bool flag = Vector2.Distance(Input.mousePosition, b) < MinDistance;
		Renderer.enabled = flag;
	}
}

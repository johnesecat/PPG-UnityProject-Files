using UnityEngine;

public class DeterminedAnomaly : MonoBehaviour
{
	public GameObject Prefab;

	private Vector3 position;

	private Quaternion rotation;

	private Vector3 scale;

	private SpriteRenderer spriteRenderer;

	private bool sentBack;

	private void Start()
	{
		position = base.transform.position;
		rotation = base.transform.rotation;
		scale = base.transform.localScale;
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		ToolBehaviour currentTool = Global.main.ToolControllerBehaviour.CurrentTool;
		if ((bool)currentTool && (bool)currentTool.ActiveSingleSelected && currentTool.ActiveSingleSelected.gameObject == base.gameObject)
		{
			return;
		}
		if (sentBack)
		{
			Vector3 vector = Global.main.camera.WorldToViewportPoint(position);
			if (vector.x > 1.1f || vector.x < -0.1f || vector.y > 1.1f || vector.y < -0.1f)
			{
				GameObject obj = Object.Instantiate(Prefab);
				obj.name = base.gameObject.name;
				if (obj.TryGetComponent<Rigidbody2D>(out var component))
				{
					component.angularVelocity = 0f;
					component.velocity = default(Vector2);
				}
				obj.transform.position = position;
				obj.transform.localScale = scale;
				obj.transform.rotation = rotation;
				Object.Destroy(base.gameObject);
			}
		}
		else if ((base.transform.position - position).sqrMagnitude > 1f && !spriteRenderer.isVisible)
		{
			sentBack = true;
		}
	}
}

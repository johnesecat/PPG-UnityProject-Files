using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FitCameraBackgroundBehaviour : MonoBehaviour
{
	public Camera Camera;

	private SpriteRenderer spriteRenderer;

	private Vector2 spriteSize;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteSize = spriteRenderer.sprite.bounds.size;
	}

	private void Update()
	{
		if (!spriteRenderer.isVisible)
		{
			base.transform.position = new Vector3(Camera.transform.position.x, Camera.transform.position.y, base.transform.position.z);
		}
	}

	private void OnWillRenderObject()
	{
		base.transform.position = new Vector3(Camera.transform.position.x, Camera.transform.position.y, base.transform.position.z);
		Vector2 vector = Camera.orthographicSize * 2f * new Vector2(Camera.aspect, 1f);
		vector /= spriteSize;
		base.transform.localScale = vector;
	}
}

using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ParallaxBehaviour : MonoBehaviour
{
	public float Depth;

	public Vector2 Offset;

	public Vector2 Scale = new Vector2(1f, 1f);

	public Transform Container;

	private Camera cam;

	private void Awake()
	{
		cam = GetComponent<Camera>();
	}

	private void OnPreRender()
	{
		float num = Depth / cam.orthographicSize + 1f;
		Vector2 vector = (Offset - (Vector2)cam.transform.localPosition) / num;
		Container.localPosition = new Vector3(vector.x, vector.y, Container.localPosition.z);
		Container.localScale = Scale * Depth / num;
	}
}

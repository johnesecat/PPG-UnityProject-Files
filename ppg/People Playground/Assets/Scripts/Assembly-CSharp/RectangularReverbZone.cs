using UnityEngine;

[ExecuteAlways]
public class RectangularReverbZone : MonoBehaviour
{
	public AudioReverbZone Zone;

	public Vector2 Size = new Vector2(1f, 1f);

	public float MaxDistance = 10f;

	private AudioListener listener;

	private void OnEnable()
	{
		if (!Zone)
		{
			Zone = GetComponent<AudioReverbZone>();
		}
	}

	private void Update()
	{
		if (!Zone)
		{
			return;
		}
		if (!listener)
		{
			if (!Camera.main)
			{
				return;
			}
			listener = Camera.main.GetComponent<AudioListener>();
		}
		Vector3 position = listener.transform.position;
		Vector3 position2 = base.transform.position;
		float num = Vector2.Distance(b: new Vector2(Mathf.Clamp(position.x, position2.x - Size.x / 2f, position2.x + Size.x / 2f), Mathf.Clamp(position.y, position2.y - Size.y / 2f, position2.y + Size.y / 2f)), a: position2) + 5f;
		Zone.minDistance = num;
		Zone.maxDistance = num + MaxDistance / 2f;
	}
}

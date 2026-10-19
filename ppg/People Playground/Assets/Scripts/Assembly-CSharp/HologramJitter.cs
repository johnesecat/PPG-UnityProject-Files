using TMPro;
using UnityEngine;

public class HologramJitter : MonoBehaviour
{
	[SkipSerialisation]
	public TextMeshPro TextMesh;

	[SkipSerialisation]
	public SpriteRenderer Sprite;

	[SkipSerialisation]
	public Gradient Colors;

	public float JitterIntensity = 0.01f;

	private float offset;

	private void Awake()
	{
		offset = Random.Range(-1f, 1f) * 1500f;
	}

	private void OnWillRenderObject()
	{
		float x = Time.time * 250f + offset;
		float time = Mathf.PerlinNoise(x, -384.4597f);
		float t = Mathf.PerlinNoise(x, 908.354f);
		if ((bool)TextMesh)
		{
			TextMesh.color = Colors.Evaluate(time);
		}
		if ((bool)Sprite)
		{
			Sprite.color = Colors.Evaluate(time);
		}
		base.transform.localPosition = new Vector3(0f, Mathf.Lerp(0f - JitterIntensity, JitterIntensity, t), 0f);
	}
}

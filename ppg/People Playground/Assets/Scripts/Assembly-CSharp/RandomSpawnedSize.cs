using NaughtyAttributes;
using UnityEngine;

public class RandomSpawnedSize : MonoBehaviour
{
	[MinMaxSlider(0f, 2f)]
	public Vector2 Range = new Vector2(0.95f, 1f);

	private void Awake()
	{
		base.transform.localScale = Vector2.one * Random.Range(Range.x, Range.y);
	}
}

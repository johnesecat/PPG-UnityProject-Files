using NaughtyAttributes;
using UnityEngine;

public class RandomPrefabSpawnerBehaviour : MonoBehaviour
{
	public enum Shape
	{
		Point,
		Line
	}

	public GameObject[] Prefabs;

	[Range(0f, 1f)]
	public float SpawnChancePerPeriod = 1f;

	public float PeriodInSeconds = 1f;

	public Shape SpawnShape;

	public Vector2 Offset;

	[ShowIf("IsLine")]
	[BoxGroup("Line")]
	public Vector2 LocalFrom;

	[ShowIf("IsLine")]
	[BoxGroup("Line")]
	public Vector2 LocalTo;

	private float t;

	internal bool IsLine()
	{
		return SpawnShape == Shape.Line;
	}

	private void Update()
	{
		t += Time.deltaTime;
		if (!(t > PeriodInSeconds))
		{
			return;
		}
		t = 0f;
		if (Random.value <= SpawnChancePerPeriod)
		{
			Vector3 position = default(Vector3);
			switch (SpawnShape)
			{
			case Shape.Point:
				position = base.transform.TransformPoint(Offset);
				break;
			case Shape.Line:
				position = Vector3.Lerp(base.transform.TransformPoint(Offset + LocalFrom), base.transform.TransformPoint(Offset + LocalTo), Random.value);
				break;
			}
			Object.Instantiate(Prefabs.PickRandom(), position, Quaternion.identity);
		}
	}
}

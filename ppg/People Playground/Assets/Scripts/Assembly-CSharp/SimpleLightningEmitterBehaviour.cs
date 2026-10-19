using System;
using System.Linq;
using UnityEngine;

public class SimpleLightningEmitterBehaviour : MonoBehaviour
{
	public static SimpleLightningEmitterBehaviour Instance;

	public GameObject PoolableLightningPrefab;

	private static readonly Vector3[] vertices = new Vector3[64];

	private void Start()
	{
		Instance = this;
	}

	public void Emit(Vector2 origin, Vector2 direction, float maxSearchRange, float width, Action<PhysicalBehaviour, Vector2> onHit, Color color, LayerMask layers, in float attractionInfluence = 3f, in int maxIterations = 128, in float maxStep = 0.07f, in float randomness = 0.05f, PhysicalBehaviour[] exclude = null)
	{
		float num = maxSearchRange * maxSearchRange;
		vertices[0] = origin;
		Vector2 vector = origin;
		Vector2 vector2 = direction.normalized * maxStep;
		int i = 0;
		for (int j = 0; j < maxIterations; j++)
		{
			Collider2D collider2D = Physics2D.OverlapPoint(vector, layers);
			if ((bool)collider2D && Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collider2D.transform, out var value) && (exclude == null || !exclude.Contains(value)))
			{
				onHit?.Invoke(value, vector);
				break;
			}
			vector2 = Vector2.Lerp(vector2, UnityEngine.Random.insideUnitCircle, randomness);
			foreach (PhysicalBehaviour item in Global.main.GetPhysicsObjectsNearPosition(vector, maxSearchRange))
			{
				if ((bool)item && layers.HasLayer(item.gameObject.layer) && (exclude == null || !exclude.Contains(item)))
				{
					Vector2 vector3 = item.GetClosestPointTo(vector) - vector;
					float sqrMagnitude = vector3.sqrMagnitude;
					if (sqrMagnitude > 0f && sqrMagnitude <= num && (item.Properties.Conducting || UnityEngine.Random.value > 0.1f))
					{
						vector2 += vector3.normalized / sqrMagnitude * attractionInfluence;
					}
				}
			}
			vector2 = Vector2.ClampMagnitude(vector2, maxStep);
			vector += vector2;
			int num2 = Mathf.FloorToInt((float)j / ((float)maxIterations / (float)vertices.Length));
			vertices[num2] = vector;
			int num3 = num2 - i;
			if (num3 > 1)
			{
				for (int k = i; k < num2; k++)
				{
					float t = (float)(k - i) / (float)(num3 - 1);
					vertices[k] = Vector2.Lerp(vertices[i], vector, t);
				}
			}
			i = num2;
		}
		for (; i < vertices.Length; i++)
		{
			vertices[i] = vector;
		}
		vertices[vertices.Length - 1] = vector;
		GameObject gameObject = PoolGenerator.Instance.RequestPrefab(PoolableLightningPrefab, origin);
		if ((bool)gameObject && gameObject.TryGetComponent<LineRenderer>(out var component))
		{
			component.SetPositions(vertices);
			component.widthMultiplier = width;
			component.endColor = color;
			component.startColor = color;
			component.colorGradient.colorKeys = new GradientColorKey[2]
			{
				new GradientColorKey(color, 0f),
				new GradientColorKey(color, 0f)
			};
		}
	}
}

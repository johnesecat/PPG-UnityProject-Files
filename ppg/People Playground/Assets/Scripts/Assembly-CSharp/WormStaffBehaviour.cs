using System;
using UnityEngine;

public class WormStaffBehaviour : MonoBehaviour
{
	private class Worm
	{
		public Vector2 velocity;

		public Vector2 endPosition;

		public Vector2 originalEndpos;

		public LineRenderer lineRenderer;

		public Vector3[] vertices;

		public float seed;

		public Worm(LineRenderer lineRenderer, float seed)
		{
			this.lineRenderer = lineRenderer;
			this.seed = seed;
		}
	}

	[SkipSerialisation]
	public int VertexCount = 4;

	[SkipSerialisation]
	public float Range;

	[SkipSerialisation]
	public float Damage;

	[SkipSerialisation]
	public float CutChance;

	[SkipSerialisation]
	public Vector3 Center;

	[SkipSerialisation]
	public LayerMask Layer;

	[SkipSerialisation]
	public Rigidbody2D Rigidbody;

	[SkipSerialisation]
	public LineRenderer[] LineRenderers;

	private Worm[] worms;

	private Collider2D[] collisionBuffer;

	private void Start()
	{
		collisionBuffer = new Collider2D[LineRenderers.Length + 1];
		worms = new Worm[LineRenderers.Length];
		for (int i = 0; i < LineRenderers.Length; i++)
		{
			worms[i] = new Worm(LineRenderers[i], UnityEngine.Random.value * 1000f);
		}
		float num = 0f;
		for (int j = 0; j < worms.Length; j++)
		{
			Worm worm = worms[j];
			Vector3 a = Vector2.zero;
			Vector3 vector = new Vector2(Mathf.Cos(num), Mathf.Sin(num)) * Range;
			worm.lineRenderer.positionCount = VertexCount;
			for (int k = 0; k < VertexCount; k++)
			{
				worm.lineRenderer.SetPosition(k, Vector3.Lerp(a, vector, (float)k / (float)VertexCount));
			}
			worm.endPosition = vector;
			worm.originalEndpos = vector;
			worm.vertices = new Vector3[VertexCount];
			worm.lineRenderer.GetPositions(worm.vertices);
			num += (float)Math.PI * 2f / (float)LineRenderers.Length;
		}
	}

	private void Update()
	{
		float num = Time.time * 1.2f;
		float num2 = Time.deltaTime * 60f;
		float num3 = Mathf.Pow(0.4f, num2);
		Vector2 point = base.transform.TransformPoint(Center);
		Vector2 pointVelocity = Rigidbody.GetPointVelocity(point);
		int collCount = Physics2D.OverlapCircleNonAlloc(point, Range, collisionBuffer, Layer);
		for (int i = 0; i < worms.Length; i++)
		{
			Worm worm = worms[i];
			worm.velocity += pointVelocity * -0.5f;
			if (!ActOnTarget(collCount, worm))
			{
				worm.velocity += worm.originalEndpos - worm.endPosition;
			}
			worm.velocity *= num3;
			worm.endPosition += (Vector2)base.transform.InverseTransformVector(0.01f * num2 * worm.velocity);
			worm.endPosition = Vector2.ClampMagnitude(worm.endPosition, Range * 1.5f);
			for (int j = 0; j < VertexCount; j++)
			{
				float num4 = (float)j / (float)VertexCount;
				worm.vertices[j] = Vector3.Lerp(Vector2.zero, worm.endPosition, num4) + 0.2f * num4 * Utils.GetPerlin2Mapped(num - (float)j * 0.1f, worm.seed);
			}
			worm.lineRenderer.SetPositions(worm.vertices);
		}
	}

	private bool ActOnTarget(int collCount, Worm worm)
	{
		if (collCount <= 1)
		{
			return false;
		}
		Collider2D target = collisionBuffer.PickRandom(collCount);
		if (target == null)
		{
			return false;
		}
		if (target.transform == base.transform)
		{
			return false;
		}
		Vector2 worldEndPos = worm.lineRenderer.transform.TransformPoint(worm.endPosition);
		if (target.TryGetComponent<LimbBehaviour>(out var component))
		{
			moveToTargetAndTarget(component.PhysicalBehaviour.rigidbody);
			if (Mathf.PerlinNoise(worm.seed, Time.time * 15f) < CutChance)
			{
				component.Damage(Damage);
				component.CirculationBehaviour.StabWoundCount++;
				component.SkinMaterialHandler.AddDamagePoint(DamageType.Stab, worldEndPos, 6f);
				component.CirculationBehaviour.Cut(worldEndPos, UnityEngine.Random.insideUnitCircle);
			}
			return true;
		}
		if (target.TryGetComponent<Rigidbody2D>(out var component2))
		{
			moveToTargetAndTarget(component2);
			return true;
		}
		return false;
		void moveToTargetAndTarget(Rigidbody2D rb)
		{
			Vector2 vector = target.bounds.ClosestPoint(worldEndPos);
			worm.velocity += vector - worldEndPos;
			rb.AddForceAtPosition(UnityEngine.Random.insideUnitCircle, vector);
		}
	}
}

using System;
using System.Buffers;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class LocalFirePropagationMap : MonoBehaviour
{
	[Serializable]
	public struct BurnPoint
	{
		public Vector2 LocalPoint;

		public float FireIntensity;

		public float Softness;

		public int[] NeighbourIndices;
	}

	[SkipSerialisation]
	public Collider2D Collider;

	[SkipSerialisation]
	public BurnPoint[] Points;

	[SkipSerialisation]
	public Vector2 CalculationOffset;

	[SkipSerialisation]
	public float Randomisation = 0.4f;

	[SkipSerialisation]
	public float MergeDistanceMultiplier = 0.6f;

	[SkipSerialisation]
	public float GridStep = 0.2f;

	[SkipSerialisation]
	public float FireIntensityDampening = 3f;

	[SkipSerialisation]
	public float FireSpreadSpeed = 1f;

	[SkipSerialisation]
	public int[] RandomisedIndices;

	private PhysicalBehaviour phys;

	public void Awake()
	{
		phys = GetComponent<PhysicalBehaviour>();
		if (Points == null || Points.Length == 0)
		{
			CalculatePoints();
		}
	}

	public int GetRandomFireyIndex(float minimumFireIntensity)
	{
		int result = -1;
		float num = 1f / (float)Points.Length;
		int num2 = UnityEngine.Random.Range(0, Points.Length);
		for (int i = 0; i < Points.Length; i++)
		{
			num2 = (num2 + 1) % Points.Length;
			if (Points[num2].FireIntensity > minimumFireIntensity)
			{
				result = num2;
			}
			if (num > UnityEngine.Random.value)
			{
				return result;
			}
		}
		return result;
	}

	private void Start()
	{
		RandomisedIndices = new int[Points.Length];
		for (int i = 0; i < Points.Length; i++)
		{
			Points[i].Softness = UnityEngine.Random.Range(0.001f, 1f);
			RandomisedIndices[i] = i;
		}
		Array.Sort(RandomisedIndices, (int a, int b) => (UnityEngine.Random.value > 0f) ? 1 : (-1));
	}

	public void IgniteAt(Vector2 worldPoint)
	{
		Vector2 vector = base.transform.InverseTransformPoint(Collider.ClosestPoint(worldPoint));
		float num = GridStep * GridStep;
		float num2 = float.MaxValue;
		int num3 = 0;
		for (int i = 0; i < Points.Length; i++)
		{
			float sqrMagnitude = (Points[i].LocalPoint - vector).sqrMagnitude;
			if (sqrMagnitude < num2)
			{
				num2 = sqrMagnitude;
				num3 = i;
			}
			if (sqrMagnitude < num)
			{
				Points[i].FireIntensity = 1f;
			}
		}
		Points[num3].FireIntensity = 1f;
	}

	private void FixedUpdate()
	{
		for (int i = 0; i < Points.Length; i++)
		{
			for (int j = 0; j < Points[i].NeighbourIndices.Length; j++)
			{
				int num = Points[i].NeighbourIndices[j];
				BurnPoint burnPoint = Points[num];
				if (burnPoint.FireIntensity > Points[i].FireIntensity)
				{
					Points[i].FireIntensity = Utils.SmoothApproach(Points[i].FireIntensity, burnPoint.FireIntensity, FireSpreadSpeed * Points[i].Softness, Time.fixedDeltaTime);
				}
				else
				{
					Points[num].FireIntensity = Utils.SmoothApproach(burnPoint.FireIntensity, Points[i].FireIntensity, FireSpreadSpeed * Points[num].Softness, Time.fixedDeltaTime);
				}
			}
			if (!phys.OnFire)
			{
				Points[i].FireIntensity = Utils.SmoothApproach(Points[i].FireIntensity, 0f, FireIntensityDampening * Points[i].Softness, Time.fixedDeltaTime);
			}
		}
	}

	[Button("CalculatePoints", EButtonEnableMode.Always)]
	private void CalculatePoints()
	{
		int num = Mathf.CeilToInt(Collider.bounds.size.x / GridStep);
		int num2 = Mathf.CeilToInt(Collider.bounds.size.y / GridStep);
		Points = new BurnPoint[num * num2];
		int newSize = 0;
		for (float num3 = Collider.bounds.min.x; num3 < Collider.bounds.max.x; num3 += GridStep)
		{
			for (float num4 = Collider.bounds.min.y; num4 < Collider.bounds.max.y; num4 += GridStep)
			{
				Vector2 vector = new Vector2(num3, num4) + CalculationOffset + Randomisation * GridStep * UnityEngine.Random.insideUnitCircle;
				if (Collider.OverlapPoint(vector))
				{
					Points[newSize++] = new BurnPoint
					{
						LocalPoint = base.transform.InverseTransformPoint(vector)
					};
				}
			}
		}
		Array.Resize(ref Points, newSize);
	}

	[Button("StepRepulse", EButtonEnableMode.Always)]
	private void StepRepulse()
	{
		for (int i = 0; i < Points.Length; i++)
		{
			for (int j = i + 1; j < Points.Length; j++)
			{
				Vector2 vector = Points[i].LocalPoint - Points[j].LocalPoint;
				Points[i].LocalPoint += 0.002f * Time.deltaTime * (vector.normalized / (vector.sqrMagnitude * vector.sqrMagnitude * 4f / GridStep));
			}
		}
		for (int k = 0; k < Points.Length; k++)
		{
			Vector3 vector2 = base.transform.TransformPoint(Points[k].LocalPoint);
			Points[k].LocalPoint = base.transform.InverseTransformPoint(Collider.ClosestPoint(vector2));
		}
	}

	[Button("MergeByDistance", EButtonEnableMode.Always)]
	private void MergeByDistance()
	{
		float num = MergeDistanceMultiplier * GridStep * GridStep;
		BurnPoint[] array = ArrayPool<BurnPoint>.Shared.Rent(Points.Length);
		int num2 = 0;
		for (int i = 0; i < Points.Length; i++)
		{
			bool flag = true;
			for (int j = i + 1; j < Points.Length; j++)
			{
				if ((Points[i].LocalPoint - Points[j].LocalPoint).sqrMagnitude < num)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				array[num2++] = Points[i];
			}
		}
		Array.Resize(ref Points, num2);
		Array.Copy(array, Points, num2);
		ArrayPool<BurnPoint>.Shared.Return(array);
	}

	[Button("CalculateNeighbours", EButtonEnableMode.Always)]
	private void CalculateNeighbours()
	{
		float[] array = new float[3];
		for (int i = 0; i < Points.Length; i++)
		{
			int[] array2 = (Points[i].NeighbourIndices = new int[3]);
			array2.Fill(-1);
			int val = 0;
			array.Fill(float.MaxValue);
			for (int j = 0; j < Points.Length; j++)
			{
				if (j == i)
				{
					continue;
				}
				float sqrMagnitude = (Points[j].LocalPoint - Points[i].LocalPoint).sqrMagnitude;
				for (int k = 0; k < array.Length; k++)
				{
					if (sqrMagnitude < array[k] && sqrMagnitude < GridStep * 4f * GridStep && !array2.Contains(j))
					{
						array2[k] = j;
						array[k] = sqrMagnitude;
						val = Math.Max(k + 1, val);
					}
				}
			}
			Points[i].NeighbourIndices = array2.Where((int ii) => ii != -1).ToArray();
		}
	}
}

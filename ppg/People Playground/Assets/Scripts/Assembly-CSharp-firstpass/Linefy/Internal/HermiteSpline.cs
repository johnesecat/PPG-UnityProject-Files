using System;
using Linefy.Serialization;
using UnityEngine;

namespace Linefy.Internal
{
	public class HermiteSpline : Drawable
	{
		protected Vector3[] knots;

		public readonly Vector3[] points;

		private float[] distances;

		private Vector3[] constantPoints;

		private int segments;

		internal float step;

		protected Vector3 minusKnot;

		protected Vector3 plusKnot;

		private float normalizedPointsStep;

		protected int sectorsCount;

		protected bool knotsIsModified = true;

		protected bool polylineIsModified = true;

		private Polyline _polyline;

		public SerializationData_Polyline properties = new SerializationData_Polyline(2f, Color.green, 1f, isClosed: false);

		public readonly bool constantSpeed;

		private float _tension;

		private float _splineLength;

		[Obsolete]
		public int PointsCount;

		[Obsolete]
		public Vector3[] Points;

		public int segmentsCount => segments;

		public int knotsCount => knots.Length;

		public float tension
		{
			get
			{
				return _tension;
			}
			set
			{
				if (value != _tension)
				{
					_tension = value;
					knotsIsModified = true;
					polylineIsModified = true;
				}
			}
		}

		public float splineLength => _splineLength;

		protected virtual int getPointCount => (knotsCount - 1) * segmentsCount + 1;

		protected virtual Vector3 getMinusKnot => knots[0] - (knots[1] - knots[0]);

		protected virtual Vector3 getPlusKnot => knots[knots.Length - 1] + (knots[knots.Length - 1] - knots[knots.Length - 2]);

		protected virtual Vector3 getLastPoint => this[knots.Length - 1];

		public virtual Vector3 this[int knotIdx]
		{
			get
			{
				knotIdx = MathUtility.RepeatIdx(knotIdx, knots.Length);
				return knots[knotIdx];
			}
			set
			{
				knotIdx = MathUtility.RepeatIdx(knotIdx, knots.Length);
				knots[knotIdx] = value;
				polylineIsModified = true;
				knotsIsModified = true;
			}
		}

		public virtual bool isClosed => false;

		public HermiteSpline(int _knotsCount, int segmentsCount, bool constantSpeed, float tension)
		{
			if (_knotsCount < 2)
			{
				Debug.LogErrorFormat("HermiteSpline.ctor() knots count {0}<2", knotsCount);
				_knotsCount = 2;
			}
			if (segmentsCount < 1)
			{
				Debug.LogErrorFormat("HermiteSpline.ctor() segments count {0}<1", this.segmentsCount);
				segmentsCount = 1;
			}
			_tension = tension;
			this.constantSpeed = constantSpeed;
			segments = segmentsCount;
			knots = new Vector3[_knotsCount];
			int num = getPointCount;
			points = new Vector3[num];
			step = 1f / (float)this.segmentsCount;
			int num2 = num - 1;
			normalizedPointsStep = 1f / (float)num2;
			sectorsCount = _knotsCount - 1;
			if (constantSpeed)
			{
				distances = new float[num];
				constantPoints = new Vector3[num];
			}
		}

		public void SetKnots(Vector3[] knotsPositions)
		{
			if (knotsPositions != null && knotsPositions.Length == knots.Length)
			{
				knotsPositions.CopyTo(knots, 0);
				knotsIsModified = true;
				polylineIsModified = true;
				ApplyKnotsPositions();
			}
		}

		public virtual void ApplyKnotsPositions()
		{
			if (!knotsIsModified)
			{
				return;
			}
			_splineLength = 0f;
			minusKnot = getMinusKnot;
			plusKnot = getPlusKnot;
			int num = 0;
			for (int i = 0; i < sectorsCount; i++)
			{
				points[num] = knots[i];
				num++;
				Vector3 y = this[i - 1];
				Vector3 y2 = this[i];
				Vector3 y3 = this[i + 1];
				Vector3 y4 = this[i + 2];
				for (int j = 1; j < segmentsCount; j++)
				{
					points[num] = Vector3Utility.HermiteInterpolate(y, y2, y3, y4, (float)j * step, tension);
					num++;
				}
			}
			points[num] = getLastPoint;
			if (constantSpeed)
			{
				for (int k = 1; k < points.Length; k++)
				{
					_splineLength += Vector3.Distance(points[k], points[k - 1]);
					distances[k] = _splineLength;
				}
				constantPoints[0] = points[0];
				int findedIndex = 1;
				for (int l = 1; l < constantPoints.Length; l++)
				{
					constantPoints[l] = GetConstantPoint((float)l * normalizedPointsStep, ref findedIndex);
				}
				constantPoints[constantPoints.Length - 1] = getLastPoint;
			}
			knotsIsModified = false;
		}

		private Vector3 GetConstantPoint(float persentage, ref int findedIndex)
		{
			float num = persentage * _splineLength;
			for (int i = findedIndex; i < distances.Length; i++)
			{
				float num2 = distances[i - 1];
				float num3 = distances[i];
				if (num2 <= num && num3 > num)
				{
					float t = (num - num2) / (num3 - num2);
					findedIndex = i;
					return Vector3.LerpUnclamped(points[i - 1], points[i], t);
				}
			}
			return points[0];
		}

		public void DrawDebugEndPoints(float size)
		{
		}

		public void DrawDebug(Color c)
		{
			for (int i = 0; i < points.Length - 1; i++)
			{
				Debug.DrawLine(points[i], points[i + 1], c);
			}
		}

		public void DrawDebug(Color c, Matrix4x4 tm)
		{
			for (int i = 0; i < points.Length - 1; i++)
			{
				Debug.DrawLine(tm.MultiplyPoint3x4(points[i]), tm.MultiplyPoint3x4(points[i + 1]), c);
			}
		}

		public virtual Vector3 GetPoint(float t)
		{
			if (t >= 1f)
			{
				return points[points.Length - 1];
			}
			int num = Mathf.FloorToInt(t / normalizedPointsStep);
			float t2 = (t - normalizedPointsStep * (float)num) / normalizedPointsStep;
			if (constantSpeed)
			{
				return Vector3.LerpUnclamped(constantPoints[num], constantPoints[num + 1], t2);
			}
			return Vector3.LerpUnclamped(points[num], points[num + 1], t2);
		}

		private void PreDraw()
		{
			if (!polylineIsModified)
			{
				return;
			}
			ApplyKnotsPositions();
			if (_polyline == null)
			{
				_polyline = new Polyline(points.Length);
			}
			_polyline.LoadSerializationData(properties);
			if (isClosed)
			{
				_polyline.isClosed = true;
				_polyline.count = points.Length - 1;
				if (constantSpeed)
				{
					for (int i = 0; i < points.Length - 1; i++)
					{
						_polyline.SetPosition(i, constantPoints[i]);
					}
				}
				else
				{
					for (int j = 0; j < points.Length - 1; j++)
					{
						_polyline.SetPosition(j, points[j]);
					}
				}
			}
			else
			{
				_polyline.isClosed = false;
				_polyline.count = points.Length;
				if (constantSpeed)
				{
					for (int k = 0; k < points.Length; k++)
					{
						_polyline.SetPosition(k, constantPoints[k]);
					}
				}
				else
				{
					for (int l = 0; l < points.Length; l++)
					{
						_polyline.SetPosition(l, points[l]);
					}
				}
			}
			polylineIsModified = false;
		}

		public override void DrawNow(Matrix4x4 matrix)
		{
			PreDraw();
			_polyline.DrawNow(matrix);
		}

		public override void Draw(Matrix4x4 matrix, Camera cam, int layer)
		{
			PreDraw();
			_polyline.Draw(matrix, cam, layer);
			_polyline.isClosed = isClosed;
		}

		public override void Dispose()
		{
			throw new NotImplementedException();
		}

		public override void GetStatistic(ref int linesCount, ref int totallinesCount, ref int dotsCount, ref int totalDotsCount, ref int polylinesCount, ref int totalPolylineVerticesCount)
		{
			if (_polyline != null)
			{
				polylinesCount++;
				totalPolylineVerticesCount += _polyline.count;
			}
		}
	}
}

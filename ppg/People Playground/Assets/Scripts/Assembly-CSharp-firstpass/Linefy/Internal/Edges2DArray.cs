using UnityEngine;

namespace Linefy.Internal
{
	public class Edges2DArray
	{
		private int _length;

		private Edge2D[] edges;

		public int length => _length;

		public Edges2DArray(int count)
		{
			edges = new Edge2D[count];
			_length = count;
		}

		public void SetEdge(int index, Vector2 a, Vector2 b)
		{
			edges[index] = new Edge2D(a, b);
		}

		public float GetDistanceToPoint(ref Vector2 nearestA, ref Vector2 nearestB, Vector2 point)
		{
			float num = float.MaxValue;
			for (int i = 0; i < edges.Length; i++)
			{
				float distance = edges[i].GetDistance(point);
				if (distance < num)
				{
					num = distance;
					nearestA = edges[i].a;
					nearestB = edges[i].b;
				}
			}
			return num;
		}

		public float GetDistanceToPoint(Vector2 point)
		{
			float num = float.MaxValue;
			for (int i = 0; i < edges.Length; i++)
			{
				float distance = edges[i].GetDistance(point);
				if (distance < num)
				{
					num = distance;
				}
			}
			return num;
		}
	}
}

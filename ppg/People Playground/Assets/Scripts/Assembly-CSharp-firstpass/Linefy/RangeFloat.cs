using System;
using UnityEngine;

namespace Linefy
{
	[Serializable]
	public struct RangeFloat
	{
		public float from;

		public float to;

		public RangeFloat(float from, float to)
		{
			this.from = from;
			this.to = to;
		}

		public bool InRange(float t)
		{
			if (t >= from && t <= to)
			{
				return true;
			}
			return false;
		}

		public bool InRange(float t, ref float lv)
		{
			if (InRange(t))
			{
				lv = Mathf.InverseLerp(from, to, t);
				return true;
			}
			return false;
		}
	}
}

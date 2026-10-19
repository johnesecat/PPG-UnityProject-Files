using UnityEngine;

namespace Linefy.Internal
{
	public class HermiteSplineClosed : HermiteSpline
	{
		protected override Vector3 getMinusKnot => knots[knots.Length - 2];

		protected override Vector3 getPlusKnot => knots[0];

		protected override int getPointCount => base.knotsCount * base.segmentsCount + 1;

		protected override Vector3 getLastPoint => points[0];

		public override bool isClosed => true;

		public HermiteSplineClosed(int knotsCount, int segmentsCount)
			: base(knotsCount, segmentsCount, constantSpeed: false, 1f)
		{
			sectorsCount = knotsCount;
		}

		public HermiteSplineClosed(int knotsCount, int segmentsCount, bool constantSpeed, float tension)
			: base(knotsCount, segmentsCount, constantSpeed, tension)
		{
			sectorsCount = knotsCount;
		}
	}
}

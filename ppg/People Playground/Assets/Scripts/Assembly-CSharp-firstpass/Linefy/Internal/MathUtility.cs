using System;
using UnityEngine;

namespace Linefy.Internal
{
	public static class MathUtility
	{
		public static float HermiteValue(float y0, float y1, float y2, float y3, float t)
		{
			float num = t * t;
			float num2 = num * t;
			float num3 = (y1 - y0) / 2f;
			num3 += (y2 - y1) / 2f;
			float num4 = (y2 - y1) / 2f;
			num4 += (y3 - y2) / 2f;
			float num5 = 2f * num2 - 3f * num + 1f;
			float num6 = num2 - 2f * num + t;
			float num7 = num2 - num;
			float num8 = -2f * num2 + 3f * num;
			return num5 * y1 + num6 * num3 + num7 * num4 + num8 * y2;
		}

		public static int RoundedArrayIdx(int idx, int arrLength)
		{
			if (arrLength == 0)
			{
				return 0;
			}
			idx %= arrLength;
			if (idx < 0)
			{
				idx = (arrLength + idx) % arrLength;
			}
			return idx;
		}

		public static float GetValue(this float[] arr, float time)
		{
			if (arr.Length == 0)
			{
				return 0f;
			}
			if (time.LessOrEqualsThan(0f))
			{
				return arr[0];
			}
			if (time.GreaterOrEqualsThan(1f))
			{
				return arr[arr.Length - 1];
			}
			float num = 1f / (float)(arr.Length - 1);
			int num2 = Mathf.FloorToInt(time / num);
			float t = (time - (float)num2 * num) / num;
			return Mathf.LerpUnclamped(arr[num2], arr[num2 + 1], t);
		}

		public static float LinearToSin(float t)
		{
			return 1f - (Mathf.Sin(t * 3.141592f + 1.5708f) * 0.49999f + 0.5f);
		}

		public static float InverseLerpUnclamped(float a, float b, float value)
		{
			if (a != b)
			{
				return (value - a) / (b - a);
			}
			return 0f;
		}

		public static bool ApproximatelyZero(float f)
		{
			if (f < 1E-05f)
			{
				return f > -1E-05f;
			}
			return false;
		}

		public static bool ApproximatelyEquals(float a, float b)
		{
			return ApproximatelyZero(a - b);
		}

		public static bool EqualsApproximately(this float a, float b)
		{
			return ApproximatelyZero(a - b);
		}

		public static float ToDegrees(this float f)
		{
			return f * 57.29578f;
		}

		public static float ToRadians(this float f)
		{
			return f * ((float)Math.PI / 180f);
		}

		public static float DeltaAngleRad(float current, float target)
		{
			float num = Mathf.Repeat(target - current, 6.283185f);
			if (num >= (float)Math.PI)
			{
				num -= 6.283185f;
			}
			return num;
		}

		public static int RepeatIdx(int idx, int length)
		{
			if (length == 0)
			{
				Debug.LogError("Zero length");
				return 0;
			}
			idx %= length;
			if (idx < 0)
			{
				idx = length + idx;
			}
			return idx;
		}

		public static bool LessOrEqualsThan(this float f, float val)
		{
			if (!(f < val))
			{
				return ApproximatelyEquals(f, val);
			}
			return true;
		}

		public static bool GreaterOrEqualsThan(this float f, float val)
		{
			if (!(f > val))
			{
				return ApproximatelyEquals(f, val);
			}
			return true;
		}
	}
}

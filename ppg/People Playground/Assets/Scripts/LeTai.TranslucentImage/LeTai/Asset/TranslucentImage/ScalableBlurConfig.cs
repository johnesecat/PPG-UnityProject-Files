using UnityEngine;

namespace LeTai.Asset.TranslucentImage
{
	[CreateAssetMenu(fileName = "New Scalable Blur Config", menuName = "Translucent Image/ Scalable Blur Config", order = 100)]
	public class ScalableBlurConfig : BlurConfig
	{
		[SerializeField]
		private float radius = 4f;

		[SerializeField]
		private int iteration = 4;

		[SerializeField]
		private int maxDepth = 6;

		[SerializeField]
		private float strength;

		public float Radius
		{
			get
			{
				return radius;
			}
			set
			{
				radius = Mathf.Max(0f, value);
			}
		}

		public int Iteration
		{
			get
			{
				return iteration;
			}
			set
			{
				iteration = Mathf.Max(0, value);
			}
		}

		public int MaxDepth
		{
			get
			{
				return maxDepth;
			}
			set
			{
				maxDepth = Mathf.Max(1, value);
			}
		}

		public float Strength
		{
			get
			{
				return strength = Radius * Mathf.Pow(2f, Iteration);
			}
			set
			{
				strength = Mathf.Max(0f, value);
				SetAdvancedFieldFromSimple();
			}
		}

		protected virtual void SetAdvancedFieldFromSimple()
		{
			float num = Mathf.Pow(2f, Iteration);
			Radius = strength / num;
			while (Radius < 1f && Iteration > 0)
			{
				Iteration--;
				Radius *= 2f;
			}
			while (Radius > num)
			{
				Radius /= 2f;
				Iteration++;
			}
		}
	}
}

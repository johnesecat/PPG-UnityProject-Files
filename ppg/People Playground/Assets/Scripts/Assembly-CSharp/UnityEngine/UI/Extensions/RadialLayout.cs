using System;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("Layout/Extensions/Radial Layout")]
	public class RadialLayout : LayoutGroup
	{
		public float fDistance;

		[Range(0f, 360f)]
		public float MinAngle;

		[Range(0f, 360f)]
		public float MaxAngle;

		[Range(0f, 360f)]
		public float StartAngle;

		public bool OnlyLayoutVisible;

		protected override void OnEnable()
		{
			base.OnEnable();
			CalculateRadial();
		}

		public override void SetLayoutHorizontal()
		{
		}

		public override void SetLayoutVertical()
		{
		}

		public override void CalculateLayoutInputVertical()
		{
			CalculateRadial();
		}

		public override void CalculateLayoutInputHorizontal()
		{
			CalculateRadial();
		}

		private void CalculateRadial()
		{
			m_Tracker.Clear();
			if (base.transform.childCount == 0)
			{
				return;
			}
			int num = 0;
			if (OnlyLayoutVisible)
			{
				for (int i = 0; i < base.transform.childCount; i++)
				{
					RectTransform rectTransform = (RectTransform)base.transform.GetChild(i);
					if (rectTransform != null && rectTransform.gameObject.activeSelf)
					{
						num++;
					}
				}
			}
			else
			{
				num = base.transform.childCount;
			}
			float num2 = (MaxAngle - MinAngle) / (float)num;
			float num3 = StartAngle;
			for (int j = 0; j < base.transform.childCount; j++)
			{
				RectTransform rectTransform2 = (RectTransform)base.transform.GetChild(j);
				if (rectTransform2 != null && (!OnlyLayoutVisible || rectTransform2.gameObject.activeSelf))
				{
					m_Tracker.Add(this, rectTransform2, DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.Pivot);
					Vector3 vector = new Vector3(Mathf.Cos(num3 * ((float)Math.PI / 180f)), Mathf.Sin(num3 * ((float)Math.PI / 180f)), 0f);
					rectTransform2.localPosition = vector * fDistance;
					Vector2 vector3 = (rectTransform2.pivot = new Vector2(0.5f, 0.5f));
					Vector2 vector6 = (rectTransform2.anchorMin = (rectTransform2.anchorMax = vector3));
					num3 += num2;
				}
			}
		}
	}
}

using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/Segmented Control/Segmented Control")]
	[RequireComponent(typeof(RectTransform))]
	public class SegmentedControl : UIBehaviour
	{
		[Serializable]
		public class SegmentSelectedEvent : UnityEvent<int>
		{
		}

		private Selectable[] m_segments;

		[SerializeField]
		[Tooltip("A GameObject with an Image to use as a separator between segments. Size of the RectTransform will determine the size of the separator used.\nNote, make sure to disable the separator GO so that it does not affect the scene")]
		private Graphic m_separator;

		private float m_separatorWidth;

		[SerializeField]
		[Tooltip("When True, it allows each button to be toggled on/off")]
		private bool m_allowSwitchingOff;

		[SerializeField]
		[Tooltip("The selected default for the control (zero indexed array)")]
		private int m_selectedSegmentIndex = -1;

		[SerializeField]
		[Tooltip("Event to fire once the selection has been changed")]
		private SegmentSelectedEvent m_onValueChanged = new SegmentSelectedEvent();

		internal Selectable selectedSegment;

		protected float SeparatorWidth
		{
			get
			{
				if (m_separatorWidth == 0f && (bool)separator)
				{
					m_separatorWidth = separator.rectTransform.rect.width;
					Image component = separator.GetComponent<Image>();
					if ((bool)component)
					{
						m_separatorWidth /= component.pixelsPerUnit;
					}
				}
				return m_separatorWidth;
			}
		}

		public Selectable[] segments
		{
			get
			{
				if (m_segments == null || m_segments.Length == 0)
				{
					m_segments = GetChildSegments();
				}
				return m_segments;
			}
		}

		public Graphic separator
		{
			get
			{
				return m_separator;
			}
			set
			{
				m_separator = value;
				m_separatorWidth = 0f;
				LayoutSegments();
			}
		}

		public bool allowSwitchingOff
		{
			get
			{
				return m_allowSwitchingOff;
			}
			set
			{
				m_allowSwitchingOff = value;
			}
		}

		public int selectedSegmentIndex
		{
			get
			{
				return Array.IndexOf(segments, selectedSegment);
			}
			set
			{
				value = Math.Max(value, -1);
				value = Math.Min(value, segments.Length - 1);
				if (m_selectedSegmentIndex == value)
				{
					return;
				}
				m_selectedSegmentIndex = value;
				if ((bool)selectedSegment)
				{
					Segment component = selectedSegment.GetComponent<Segment>();
					if ((bool)component)
					{
						component.selected = false;
					}
					selectedSegment = null;
				}
				if (value != -1)
				{
					selectedSegment = segments[value];
					Segment component2 = selectedSegment.GetComponent<Segment>();
					if ((bool)component2)
					{
						component2.selected = true;
					}
				}
			}
		}

		public SegmentSelectedEvent onValueChanged
		{
			get
			{
				return m_onValueChanged;
			}
			set
			{
				m_onValueChanged = value;
			}
		}

		protected SegmentedControl()
		{
		}

		protected override void Start()
		{
			base.Start();
			if (base.isActiveAndEnabled)
			{
				StartCoroutine(DelayedInit());
			}
		}

		protected override void OnEnable()
		{
			StartCoroutine(DelayedInit());
		}

		private IEnumerator DelayedInit()
		{
			yield return null;
			LayoutSegments();
			if (m_selectedSegmentIndex != -1)
			{
				selectedSegmentIndex = m_selectedSegmentIndex;
			}
		}

		private Selectable[] GetChildSegments()
		{
			Selectable[] componentsInChildren = GetComponentsInChildren<Selectable>();
			if (componentsInChildren.Length < 2)
			{
				throw new InvalidOperationException("A segmented control must have at least two Button children");
			}
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Segment component = componentsInChildren[i].GetComponent<Segment>();
				if (component != null)
				{
					component.index = i;
					component.segmentedControl = this;
				}
			}
			return componentsInChildren;
		}

		private void RecreateSprites()
		{
			for (int i = 0; i < segments.Length; i++)
			{
				if (!(segments[i].image == null))
				{
					Sprite sprite = CutSprite(segments[i].image.sprite, i == 0, i == segments.Length - 1);
					Segment component = segments[i].GetComponent<Segment>();
					if ((bool)component)
					{
						component.cutSprite = sprite;
					}
					segments[i].image.overrideSprite = sprite;
				}
			}
		}

		internal static Sprite CutSprite(Sprite sprite, bool leftmost, bool rightmost)
		{
			if (sprite.border.x == 0f || sprite.border.z == 0f)
			{
				return sprite;
			}
			Rect rect = sprite.rect;
			Vector4 border = sprite.border;
			if (!leftmost)
			{
				rect.xMin = border.x;
				border.x = 0f;
			}
			if (!rightmost)
			{
				rect.xMax = border.z;
				border.z = 0f;
			}
			return Sprite.Create(sprite.texture, rect, sprite.pivot, sprite.pixelsPerUnit, 0u, SpriteMeshType.FullRect, border);
		}

		public void LayoutSegments()
		{
			RecreateSprites();
			RectTransform rectTransform = base.transform as RectTransform;
			float num = rectTransform.rect.width / (float)segments.Length - SeparatorWidth * (float)(segments.Length - 1);
			for (int i = 0; i < segments.Length; i++)
			{
				float num2 = (num + SeparatorWidth) * (float)i;
				RectTransform component = segments[i].GetComponent<RectTransform>();
				component.anchorMin = Vector2.zero;
				component.anchorMax = Vector2.zero;
				component.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, num2, num);
				component.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, rectTransform.rect.height);
				if ((bool)separator && i > 0)
				{
					Transform transform = base.gameObject.transform.Find("Separator " + i);
					Graphic obj = ((transform != null) ? transform.GetComponent<Graphic>() : Object.Instantiate(separator.gameObject).GetComponent<Graphic>());
					obj.gameObject.name = "Separator " + i;
					obj.gameObject.SetActive(value: true);
					obj.rectTransform.SetParent(base.transform, worldPositionStays: false);
					obj.rectTransform.anchorMin = Vector2.zero;
					obj.rectTransform.anchorMax = Vector2.zero;
					obj.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, num2 - SeparatorWidth, SeparatorWidth);
					obj.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, rectTransform.rect.height);
				}
			}
		}
	}
}

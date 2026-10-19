using UnityEngine;
using UnityEngine.Rendering;

namespace Linefy
{
	[HelpURL("https://polyflow.xyz/content/linefy/documentation-1-1/linefy-documentation.html#LabelsRenderer")]
	[ExecuteInEditMode]
	public class LinefyLabelsRenderer : EditableDrawableBase
	{
		public DotsAtlas atlas;

		public bool transparent;

		[Tooltip("The positioning of the label reliative to its world position.")]
		public TextAlignment horizontalAlignment = TextAlignment.Center;

		public Color textColor = Color.white;

		public Color backgroundColor = Color.green;

		public float size = 1f;

		[Tooltip("Displays background rect under text. Background is calculated using 9-grid slice technique. The indices of background rects defines in DotAtlas settings.")]
		public bool drawBackground;

		public WidthMode widthMode;

		[Tooltip("The size to be added to the automatically calculated background size.")]
		public Vector2 backgroundExtraSize = Vector2.zero;

		public CompareFunction zTest;

		[Tooltip("Render queue")]
		public int renderOrder;

		[Tooltip("Guarantee exact pixel dimensions of glyphs.")]
		public bool pixelPerfect;

		public float fadeAlphaDistanceFrom = 100000f;

		public float fadeAlphaDistanceTo = 100001f;

		public Label[] labels = new Label[1]
		{
			new Label("Lorem Ipsum Dolar", Vector3.zero, new Vector2(0f, 0f))
		};

		private LabelsRenderer _labelsRenderer;

		private LabelsRenderer labelsRenderer
		{
			get
			{
				int labelsCount = ((labels != null) ? labels.Length : 0);
				if (_labelsRenderer == null)
				{
					_labelsRenderer = new LabelsRenderer(atlas, labelsCount);
				}
				return _labelsRenderer;
			}
		}

		public override Drawable drawable => labelsRenderer;

		protected override void PreDraw()
		{
			base.PreDraw();
			if (propertiesModificationId != labelsRenderer.propertiesModificationId)
			{
				int count = ((labels != null) ? labels.Length : 0);
				labelsRenderer.count = count;
				labelsRenderer.transparent = transparent;
				labelsRenderer.atlas = atlas;
				labelsRenderer.size = size;
				labelsRenderer.textColor = textColor;
				labelsRenderer.drawBackground = drawBackground;
				labelsRenderer.horizontalAlignment = horizontalAlignment;
				labelsRenderer.backgroundColor = backgroundColor;
				labelsRenderer.backgroundExtraSize = backgroundExtraSize;
				labelsRenderer.zTest = zTest;
				labelsRenderer.renderOrder = renderOrder;
				labelsRenderer.widthMode = widthMode;
				labelsRenderer.fadeAlphaDistanceFrom = fadeAlphaDistanceFrom;
				labelsRenderer.fadeAlphaDistanceTo = fadeAlphaDistanceTo;
				labelsRenderer.pixelPerfect = pixelPerfect;
				labelsRenderer.propertiesModificationId = propertiesModificationId;
			}
			if (itemsModificationId != labelsRenderer.itemsModificationId)
			{
				for (int i = 0; i < labelsRenderer.count; i++)
				{
					labelsRenderer[i] = labels[i];
				}
				labelsRenderer.itemsModificationId = itemsModificationId;
			}
		}

		public static LinefyLabelsRenderer CreateInstance()
		{
			return new GameObject("New LabelsRenderer").AddComponent<LinefyLabelsRenderer>();
		}
	}
}

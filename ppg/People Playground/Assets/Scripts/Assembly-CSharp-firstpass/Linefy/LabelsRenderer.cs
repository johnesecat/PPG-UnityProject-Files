using System;
using Linefy.Internal;
using UnityEngine;
using UnityEngine.Rendering;

namespace Linefy
{
	public class LabelsRenderer : Drawable
	{
		private class label
		{
			public struct GlyphInfo
			{
				public int rectIdx;

				public int dotidx;

				public Vector2 pixelOffset;

				public bool isWhitespace;

				public GlyphInfo(int rectIdx, bool isWhitespace)
				{
					this.rectIdx = rectIdx;
					dotidx = 0;
					pixelOffset = default(Vector2);
					this.isWhitespace = isWhitespace;
				}
			}

			public GlyphInfo[] glyphInfos;

			public float linePixelWidth;

			private LabelsRenderer root;

			public DFlag d_text = new DFlag("d_text", initialValue: true);

			public DFlag d_positions = new DFlag("d_positions", initialValue: true);

			public DFlag d_pixelOffset = new DFlag("d_pixelOffset", initialValue: true);

			private string _text = string.Empty;

			private Vector3 _position;

			private Vector2 _offset;

			public string text
			{
				get
				{
					return _text;
				}
				set
				{
					if (value != _text)
					{
						d_text.Set();
						root.d_anyText.Set();
						_text = value;
					}
				}
			}

			public Vector3 position
			{
				get
				{
					return _position;
				}
				set
				{
					if (value != _position)
					{
						root.d_anyPositions.Set();
						d_positions.Set();
						_position = value;
					}
				}
			}

			public Vector2 offset
			{
				get
				{
					return _offset;
				}
				set
				{
					if (value != _offset)
					{
						_offset = value;
						d_pixelOffset.Set();
						root.d_anyPixelOffset.Set();
					}
				}
			}

			public void ParceText()
			{
				if (root._drawBackground)
				{
					if (_text.Length == 0)
					{
						Array.Resize(ref glyphInfos, 0);
						return;
					}
					Array.Resize(ref glyphInfos, _text.Length + 9);
					glyphInfos[0] = new GlyphInfo(root.atlas.background9SliseIndices.topLeft, isWhitespace: false);
					glyphInfos[1] = new GlyphInfo(root.atlas.background9SliseIndices.top, isWhitespace: false);
					glyphInfos[2] = new GlyphInfo(root.atlas.background9SliseIndices.topRight, isWhitespace: false);
					glyphInfos[3] = new GlyphInfo(root.atlas.background9SliseIndices.left, isWhitespace: false);
					glyphInfos[4] = new GlyphInfo(root.atlas.background9SliseIndices.center, isWhitespace: false);
					glyphInfos[5] = new GlyphInfo(root.atlas.background9SliseIndices.right, isWhitespace: false);
					glyphInfos[6] = new GlyphInfo(root.atlas.background9SliseIndices.bottomLeft, isWhitespace: false);
					glyphInfos[7] = new GlyphInfo(root.atlas.background9SliseIndices.bottom, isWhitespace: false);
					glyphInfos[8] = new GlyphInfo(root.atlas.background9SliseIndices.bottomRight, isWhitespace: false);
					for (int i = 0; i < _text.Length; i++)
					{
						int num = char.ConvertToUtf32(_text, i);
						glyphInfos[i + 9] = new GlyphInfo(root.atlas.GetRectByUtf32(num), num == 32);
					}
				}
				else
				{
					Array.Resize(ref glyphInfos, _text.Length);
					for (int j = 0; j < _text.Length; j++)
					{
						int num2 = char.ConvertToUtf32(_text, j);
						glyphInfos[j] = new GlyphInfo(root.atlas.GetRectByUtf32(num2), num2 == 32);
					}
				}
			}

			public label(LabelsRenderer root, Vector3 position, string text)
			{
				this.root = root;
				this.position = position;
				this.text = text;
			}

			public void UpdatePixelOffset()
			{
				if (_text.Length == 0)
				{
					return;
				}
				float sizeMultiplier = root._sizeMultiplier;
				float num = (float)root.atlas.horizontalSpacing * sizeMultiplier;
				float num2 = (float)root.atlas.whitespaceWidth * sizeMultiplier;
				Vector2 size = (Vector2)root.atlas.rectPixelSize * sizeMultiplier;
				Vector2 vector = new Vector2(0f, 0f);
				if (glyphInfos.Length != 0)
				{
					vector.x += size.x / 2f;
				}
				int num3 = (root._drawBackground ? 9 : 0);
				if (root.atlas.monowidth)
				{
					for (int i = num3; i < glyphInfos.Length; i++)
					{
						if (glyphInfos[i].isWhitespace)
						{
							vector.x += num2;
							continue;
						}
						glyphInfos[i].pixelOffset = new Vector2(vector.x, vector.y);
						vector.x += num;
					}
					linePixelWidth = vector.x;
				}
				else
				{
					for (int j = num3; j < glyphInfos.Length; j++)
					{
						if (glyphInfos[j].isWhitespace)
						{
							vector.x += num2;
							continue;
						}
						int rectIdx = glyphInfos[j].rectIdx;
						DotsAtlas.Rect rect = root.atlas.rects[rectIdx % root.atlas.rects.Length];
						glyphInfos[j].pixelOffset = new Vector2(vector.x - (float)rect.bounds.xMin * sizeMultiplier, vector.y);
						float num4 = (float)rect.bounds.width * sizeMultiplier;
						vector.x += num4;
						vector.x += num;
					}
					linePixelWidth = vector.x - num - size.x / 2f;
				}
				float num5 = linePixelWidth / 2f;
				if (root._horizontalAlignment == TextAlignment.Center)
				{
					float num6 = num5;
					for (int k = num3; k < glyphInfos.Length; k++)
					{
						glyphInfos[k].pixelOffset.x -= num6;
					}
				}
				else if (root._horizontalAlignment == TextAlignment.Right)
				{
					float num7 = linePixelWidth;
					for (int l = num3; l < glyphInfos.Length; l++)
					{
						glyphInfos[l].pixelOffset.x -= num7;
					}
				}
				for (int m = num3; m < glyphInfos.Length; m++)
				{
					int dotidx = glyphInfos[m].dotidx;
					root.dots.SetPixelOffset(glyphInfos[m].dotidx, glyphInfos[m].pixelOffset + offset);
					root.dots.SetSize(dotidx, size);
				}
				if (root._drawBackground)
				{
					Vector2 size2 = new Vector2(linePixelWidth, size.y) + root._backgroundExtraSize * sizeMultiplier;
					float num8 = 0f - (size2.x / 2f + size.x / 2f);
					float num9 = 0f;
					float num10 = 0f - num8;
					float y = 0f;
					float num11 = (0f - size2.y) / 2f - size.y / 2f;
					float y2 = 0f - num11;
					if (root._horizontalAlignment == TextAlignment.Left)
					{
						num8 += num5;
						num9 += num5;
						num10 += num5;
					}
					else if (root._horizontalAlignment == TextAlignment.Right)
					{
						num8 -= num5;
						num9 -= num5;
						num10 -= num5;
					}
					root.dots.SetPixelOffset(glyphInfos[0].dotidx, new Vector2(num8, num11) + offset);
					root.dots.SetSize(glyphInfos[0].dotidx, size);
					root.dots.SetPixelOffset(glyphInfos[1].dotidx, new Vector2(num9, num11) + offset);
					root.dots.SetSize(glyphInfos[1].dotidx, new Vector2(size2.x, size.y));
					root.dots.SetPixelOffset(glyphInfos[2].dotidx, new Vector2(num10, num11) + offset);
					root.dots.SetSize(glyphInfos[2].dotidx, size);
					root.dots.SetPixelOffset(glyphInfos[3].dotidx, new Vector2(num8, y) + offset);
					root.dots.SetSize(glyphInfos[3].dotidx, new Vector2(size.x, size2.y));
					root.dots.SetPixelOffset(glyphInfos[4].dotidx, new Vector2(num9, y) + offset);
					root.dots.SetSize(glyphInfos[4].dotidx, size2);
					root.dots.SetPixelOffset(glyphInfos[5].dotidx, new Vector2(num10, y) + offset);
					root.dots.SetSize(glyphInfos[5].dotidx, new Vector2(size.x, size2.y));
					root.dots.SetPixelOffset(glyphInfos[6].dotidx, new Vector2(num8, y2) + offset);
					root.dots.SetSize(glyphInfos[6].dotidx, size);
					root.dots.SetPixelOffset(glyphInfos[7].dotidx, new Vector2(num9, y2) + offset);
					root.dots.SetSize(glyphInfos[7].dotidx, new Vector2(size2.x, size.y));
					root.dots.SetPixelOffset(glyphInfos[8].dotidx, new Vector2(num10, y2) + offset);
					root.dots.SetSize(glyphInfos[8].dotidx, size);
				}
			}

			public void UpdateRectIndices()
			{
				for (int i = 0; i < glyphInfos.Length; i++)
				{
					GlyphInfo glyphInfo = glyphInfos[i];
					root.dots.SetRectIndex(glyphInfo.dotidx, glyphInfo.rectIdx);
					root.dots.SetEnabled(glyphInfo.dotidx, !glyphInfo.isWhitespace);
				}
			}

			public void UpdateColors(Color textColor)
			{
				for (int i = 0; i < glyphInfos.Length; i++)
				{
					GlyphInfo glyphInfo = glyphInfos[i];
					root.dots.SetColor(glyphInfo.dotidx, textColor);
				}
			}

			public void UpdateColors(Color textColor, Color backgroundColor)
			{
				if (_text.Length != 0)
				{
					root.dots.SetColor(glyphInfos[0].dotidx, backgroundColor);
					root.dots.SetColor(glyphInfos[1].dotidx, backgroundColor);
					root.dots.SetColor(glyphInfos[2].dotidx, backgroundColor);
					root.dots.SetColor(glyphInfos[3].dotidx, backgroundColor);
					root.dots.SetColor(glyphInfos[4].dotidx, backgroundColor);
					root.dots.SetColor(glyphInfos[5].dotidx, backgroundColor);
					root.dots.SetColor(glyphInfos[6].dotidx, backgroundColor);
					root.dots.SetColor(glyphInfos[7].dotidx, backgroundColor);
					root.dots.SetColor(glyphInfos[8].dotidx, backgroundColor);
					for (int i = 9; i < glyphInfos.Length; i++)
					{
						GlyphInfo glyphInfo = glyphInfos[i];
						root.dots.SetColor(glyphInfo.dotidx, textColor);
					}
				}
			}

			public void UpdatePositions()
			{
				if ((bool)d_positions)
				{
					for (int i = 0; i < glyphInfos.Length; i++)
					{
						GlyphInfo glyphInfo = glyphInfos[i];
						root.dots.SetPosition(glyphInfo.dotidx, _position);
					}
					d_positions.Reset();
				}
			}
		}

		private DFlag d_background = new DFlag("d_background", initialValue: true);

		private DFlag d_anyText = new DFlag("d_anyText", initialValue: true);

		private DFlag d_anyPixelOffset = new DFlag("d_anyPixelOffset", initialValue: true);

		private DFlag d_anyColors = new DFlag("d_anyColors", initialValue: true);

		private DFlag d_anyPositions = new DFlag("d_anyPositions", initialValue: true);

		private DIntValue atlasInstanceID;

		private DIntValue fontSettingsHash;

		public int itemsModificationId = -1;

		public int propertiesModificationId = -1;

		private TextAlignment _horizontalAlignment = TextAlignment.Center;

		private float _sizeMultiplier = 1f;

		private bool _drawBackground;

		private Color _textColor = Color.white;

		private Color _backgroundColor = Color.gray;

		private WidthMode _widthMode;

		private Vector2 _backgroundExtraSize = new Vector2(0f, 0f);

		private Dots dots;

		private label[] labels = new label[0];

		public CompareFunction zTest
		{
			get
			{
				return dots.zTest;
			}
			set
			{
				dots.zTest = value;
			}
		}

		public int renderOrder
		{
			get
			{
				return dots.renderOrder;
			}
			set
			{
				dots.renderOrder = value;
			}
		}

		public TextAlignment horizontalAlignment
		{
			get
			{
				return _horizontalAlignment;
			}
			set
			{
				if (_horizontalAlignment != value)
				{
					_horizontalAlignment = value;
					d_anyPixelOffset.Set();
				}
			}
		}

		public float size
		{
			get
			{
				return _sizeMultiplier;
			}
			set
			{
				if (_sizeMultiplier != value)
				{
					d_anyPixelOffset.Set();
					_sizeMultiplier = value;
				}
			}
		}

		public bool drawBackground
		{
			get
			{
				return _drawBackground;
			}
			set
			{
				if (_drawBackground != value)
				{
					_drawBackground = value;
					d_anyText.Set();
					d_background.Set();
				}
			}
		}

		public Color textColor
		{
			get
			{
				return _textColor;
			}
			set
			{
				if (_textColor != value)
				{
					_textColor = value;
					d_anyColors.Set();
				}
			}
		}

		public Color backgroundColor
		{
			get
			{
				return _backgroundColor;
			}
			set
			{
				if (_backgroundColor != value)
				{
					_backgroundColor = value;
					d_anyColors.Set();
				}
			}
		}

		public WidthMode widthMode
		{
			get
			{
				return _widthMode;
			}
			set
			{
				if (value != _widthMode)
				{
					_widthMode = value;
					if (_widthMode == WidthMode.WorldspaceBillboard)
					{
						dots.widthMode = WidthMode.PixelsBillboard;
					}
					else
					{
						dots.widthMode = _widthMode;
					}
				}
			}
		}

		public Vector2 backgroundExtraSize
		{
			get
			{
				return _backgroundExtraSize;
			}
			set
			{
				if (_backgroundExtraSize != value)
				{
					_backgroundExtraSize = value;
					d_anyPixelOffset.Set();
				}
			}
		}

		public bool transparent
		{
			get
			{
				return dots.transparent;
			}
			set
			{
				dots.transparent = value;
			}
		}

		public bool pixelPerfect
		{
			get
			{
				return dots.pixelPerfect;
			}
			set
			{
				dots.pixelPerfect = value;
			}
		}

		public float fadeAlphaDistanceFrom
		{
			get
			{
				return dots.fadeAlphaDistanceFrom;
			}
			set
			{
				dots.fadeAlphaDistanceFrom = value;
			}
		}

		public float fadeAlphaDistanceTo
		{
			get
			{
				return dots.fadeAlphaDistanceTo;
			}
			set
			{
				dots.fadeAlphaDistanceTo = value;
			}
		}

		public DotsAtlas atlas
		{
			get
			{
				return dots.atlas;
			}
			set
			{
				if (value == null)
				{
					value = DotsAtlas.DefaultFont11px;
				}
				dots.atlas = value;
			}
		}

		public int count
		{
			get
			{
				if (labels != null)
				{
					return labels.Length;
				}
				return 0;
			}
			set
			{
				value = Mathf.Max(0, value);
				int num = ((labels != null) ? labels.Length : 0);
				if (num != value)
				{
					Array.Resize(ref labels, value);
					for (int i = num; i < value; i++)
					{
						labels[i] = new label(this, Vector3.zero, string.Empty);
					}
					d_anyText.Set();
				}
			}
		}

		public Label this[int idx]
		{
			get
			{
				if (validateLabelIdx(idx))
				{
					label label = labels[idx];
					return new Label(label.text, label.position, label.offset);
				}
				return default(Label);
			}
			set
			{
				if (validateLabelIdx(idx))
				{
					label obj = labels[idx];
					obj.position = value.position;
					obj.text = value.text;
					obj.offset = value.offset;
				}
			}
		}

		public LabelsRenderer(int labelsCount)
		{
			InternalCtor(null, labelsCount);
		}

		public LabelsRenderer(DotsAtlas atlas, int labelsCount)
		{
			InternalCtor(atlas, labelsCount);
		}

		private void InternalCtor(DotsAtlas atlas, int labelsCount)
		{
			dots = new Dots(16);
			dots.capacityChangeStep = 8;
			dots.widthMode = WidthMode.PixelsBillboard;
			dots.widthMultiplier = 1f;
			dots.colorMultiplier = Color.white;
			dots.pixelPerfect = false;
			this.atlas = atlas;
			fontSettingsHash = new DIntValue(-1, d_anyText);
			atlasInstanceID = new DIntValue(-1, d_anyPixelOffset);
			count = labelsCount;
		}

		public void SetPosition(int idx, Vector3 position)
		{
			if (validateLabelIdx(idx))
			{
				labels[idx].position = position;
			}
		}

		public void SetText(int idx, string text)
		{
			if (validateLabelIdx(idx))
			{
				labels[idx].text = text;
			}
		}

		public void SetOffset(int idx, Vector2 offset)
		{
			if (validateLabelIdx(idx))
			{
				labels[idx].offset = offset;
			}
		}

		private bool validateLabelIdx(int idx)
		{
			return true;
		}

		public override void DrawNow(Matrix4x4 matrix)
		{
			PreDraw();
			dots.DrawNow(matrix);
		}

		public override void Draw(Matrix4x4 matrix, Camera cam, int layer)
		{
			PreDraw();
			dots.Draw(matrix, cam, layer);
		}

		private void PreDraw()
		{
			fontSettingsHash.SetValue(atlas.fontSettingsHash);
			atlasInstanceID.SetValue(atlas.GetInstanceID());
			if ((bool)d_background)
			{
				label[] array = labels;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].d_text.Set();
				}
				d_background.Reset();
			}
			if ((bool)d_anyText)
			{
				d_anyColors.Set();
				d_anyPixelOffset.Set();
				d_anyPositions.Set();
				int num = 0;
				label[] array = labels;
				foreach (label label in array)
				{
					label.ParceText();
					label.d_positions.Set();
					for (int j = 0; j < label.glyphInfos.Length; j++)
					{
						label.glyphInfos[j].dotidx = num;
						num++;
					}
				}
				dots.count = num;
				array = labels;
				foreach (label obj in array)
				{
					obj.UpdateRectIndices();
					obj.d_text.Reset();
				}
				d_anyText.Reset();
			}
			if ((bool)d_anyPositions)
			{
				label[] array = labels;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].UpdatePositions();
				}
				d_anyPositions.Reset();
			}
			if ((bool)d_anyPixelOffset)
			{
				label[] array = labels;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].UpdatePixelOffset();
				}
				d_anyPixelOffset.Reset();
			}
			if (!d_anyColors)
			{
				return;
			}
			if (_drawBackground)
			{
				label[] array = labels;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].UpdateColors(_textColor, _backgroundColor);
				}
			}
			else
			{
				label[] array = labels;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].UpdateColors(_textColor);
				}
			}
			d_anyColors.Reset();
		}

		public override void Dispose()
		{
			if (dots != null)
			{
				dots.Dispose();
			}
		}

		public override void GetStatistic(ref int linesCount, ref int totallinesCount, ref int dotsCount, ref int totalDotsCount, ref int polylinesCount, ref int totalPolylineVerticesCount)
		{
			if (dots != null)
			{
				dotsCount++;
				totalDotsCount += dots.count;
			}
		}
	}
}

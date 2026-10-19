using System;
using System.Collections.Generic;
using Linefy.Internal;
using UnityEngine;
using UnityEngine.Rendering;

namespace Linefy
{
	[HelpURL("https://polyflow.xyz/content/linefy/documentation-1-1/linefy-documentation.html#DotsAtlas")]
	[CreateAssetMenu(menuName = "Linefy/Dots Atlas")]
	public class DotsAtlas : ScriptableObject
	{
		[Serializable]
		public struct Apperance
		{
			[Range(0f, 1f)]
			public float backgroundBrightness;

			[Range(1f, 2f)]
			public float labelSize;

			public Color labelsColor;

			public Apperance(float labelSize)
			{
				backgroundBrightness = 0.5f;
				this.labelSize = labelSize;
				labelsColor = Color.white;
			}
		}

		[Serializable]
		public struct BackgroundIndices
		{
			public int topLeft;

			public int top;

			public int topRight;

			public int left;

			public int center;

			public int right;

			public int bottomLeft;

			public int bottom;

			public int bottomRight;

			public BackgroundIndices(int atlasRowLength)
			{
				topLeft = 0;
				top = 1;
				topRight = 2;
				left = 8;
				center = 9;
				right = 10;
				bottomLeft = 16;
				bottom = 17;
				bottomRight = 18;
			}
		}

		[Serializable]
		public struct Rect
		{
			public Vector2 v0;

			public Vector2 v1;

			public Vector2 v2;

			public Vector2 v3;

			public RectInt bounds;
		}

		[Tooltip("Atlas texture")]
		public Texture texture;

		[Range(1f, 64f)]
		public int xCount = 1;

		[Range(1f, 64f)]
		public int yCount = 1;

		public Vector2Int rectPixelSize = new Vector2Int(16, 16);

		public string statisticString;

		public int modificationHash;

		public int fontSettingsHash;

		public bool flipVertical;

		[Tooltip("Visual parameters of the preview area.")]
		public Apperance apperance = new Apperance(1f);

		public Rect[] rects;

		[SerializeField]
		private bool fontSettingsFoldout;

		[Tooltip("Is atlas used as font?")]
		public bool isFontAtlas;

		[Tooltip("Whitespace width")]
		public int whitespaceWidth = 4;

		[Tooltip("An empty space amount between glyphs.")]
		public int horizontalSpacing = 2;

		[Tooltip("When On, glyph each occupy the same amount (defined with Horizontal Spacing property) of horizontal space.  When off, the  width of visual glyph bounds + Horizontal Spacing will be used for horizontal offset. ")]
		public bool monowidth;

		[Tooltip("Enables the following Remapping Index Table where  Unicode indices binds with Atlas rects. When off, the rect indices is identical to the Unicode decimal value.")]
		public bool enableRemapping;

		public int resetIndexOffset = 32;

		[Tooltip("The custom Unicode indices for each rect.")]
		public int[] remappingIndexTable = new int[0];

		[Tooltip("The rect indices of 9-slice scaling used to draw the background.")]
		public BackgroundIndices background9SliseIndices = new BackgroundIndices(8);

		private Dictionary<int, int> _remappingDictionary;

		public Dictionary<int, int> remappingDictionary
		{
			get
			{
				if (_remappingDictionary == null)
				{
					buildRemappingDictionary();
				}
				return _remappingDictionary;
			}
		}

		public static DotsAtlas DefaultFont11px
		{
			get
			{
				string text = "Default Font 11px DotsAtlas/Default Font 11px DotsAtlas";
				DotsAtlas dotsAtlas = Resources.Load<DotsAtlas>(text);
				if (dotsAtlas == null)
				{
					Debug.LogWarningFormat("'Linefy/Resources/{0}' asset not founded. Please reinstall Linefy package", text);
					dotsAtlas = ScriptableObject.CreateInstance<DotsAtlas>();
				}
				return dotsAtlas;
			}
		}

		public static DotsAtlas DefaultFont11pxShadow
		{
			get
			{
				string text = "Default Font 11px DotsAtlas Shadow/Default Font 11px DotsAtlas Shadow";
				DotsAtlas dotsAtlas = Resources.Load<DotsAtlas>(text);
				if (dotsAtlas == null)
				{
					Debug.LogWarningFormat("'Linefy/Resources/{0}' asset not founded. Please reinstall Linefy package", text);
					dotsAtlas = ScriptableObject.CreateInstance<DotsAtlas>();
				}
				return dotsAtlas;
			}
		}

		public static DotsAtlas Default
		{
			get
			{
				string text = "Default DotsAtlas/Default DotsAtlas";
				DotsAtlas dotsAtlas = Resources.Load<DotsAtlas>(text);
				if (dotsAtlas == null)
				{
					Debug.LogWarningFormat("'Linefy/Resources/{0}' asset not founded. Please reinstall Linefy package", text);
					dotsAtlas = ScriptableObject.CreateInstance<DotsAtlas>();
				}
				return dotsAtlas;
			}
		}

		private void OnEnable()
		{
			RecalculateRectsCoordinates();
		}

		private void GetUVCoords(int index, ref Vector2 v0, ref Vector2 v1, ref Vector2 v2, ref Vector2 v3)
		{
			int num = index % xCount;
			int num2 = yCount - 1 - index / xCount;
			float num3 = 1f / (float)xCount;
			float num4 = 1f / (float)yCount;
			float x = (float)num * num3;
			float x2 = (float)num * num3 + num3;
			float y = (float)num2 * num4;
			float y2 = (float)num2 * num4 + num4;
			if (flipVertical)
			{
				v1.x = x;
				v1.y = y;
				v0.x = x;
				v0.y = y2;
				v3.x = x2;
				v3.y = y2;
				v2.x = x2;
				v2.y = y;
			}
			else
			{
				v0.x = x;
				v0.y = y;
				v1.x = x;
				v1.y = y2;
				v2.x = x2;
				v2.y = y2;
				v3.x = x2;
				v3.y = y;
			}
		}

		public static int GetDefaultDotAtlasRectIndex(DefaultDotAtlasShape shape, int outlineWidth)
		{
			int length = Enum.GetValues(typeof(DefaultDotAtlasShape)).Length;
			int num = Mathf.Clamp((int)shape, 0, length);
			outlineWidth = Mathf.Clamp(outlineWidth, 0, 16);
			return num * 16 + outlineWidth;
		}

		public static int GetSettingsHash(int _xCount, int _yCount, bool _flipVertical)
		{
			return $"{_xCount} {_yCount} {_flipVertical}".GetHashCode();
		}

		public void RecalculateRectsCoordinates()
		{
			int num = xCount * yCount;
			if (rects == null || rects.Length != xCount * yCount)
			{
				rects = new Rect[num];
			}
			int num2 = 0;
			for (int i = 0; i < yCount; i++)
			{
				for (int j = 0; j < xCount; j++)
				{
					Rect rect = rects[num2];
					GetUVCoords(num2, ref rect.v0, ref rect.v1, ref rect.v2, ref rect.v3);
					rects[num2] = rect;
					num2++;
				}
			}
			if (texture != null)
			{
				rectPixelSize = new Vector2Int(texture.width / xCount, texture.height / yCount);
			}
		}

		public string[] ApplyFontSettings()
		{
			if (texture != null && texture.GetType() == typeof(Texture2D))
			{
				if (texture.dimension != TextureDimension.Tex2D)
				{
					return new string[2]
					{
						"Apply font settings failed",
						$"Recalculation of characters bounds requires an Tex2D texture dimension"
					};
				}
				Texture2D readableCopy = ((Texture2D)texture).GetReadableCopy();
				int num = readableCopy.width / xCount;
				int num2 = readableCopy.height / yCount;
				int num3 = 0;
				for (int i = 0; i < yCount; i++)
				{
					for (int j = 0; j < xCount; j++)
					{
						rects[num3].bounds = GetCharBounds(readableCopy, j * num, (yCount - 1 - i) * num2, num, num2);
						num3++;
					}
				}
				if (enableRemapping)
				{
					buildRemappingDictionary();
				}
				fontSettingsHash = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
				return null;
			}
			if (!(texture == null))
			{
				return new string[2]
				{
					"Apply font settings failed",
					$"{texture.name} has not supported texture type. Recalculation of characters bounds requires an Texture2D"
				};
			}
			return new string[2] { "Apply font settings failed", "texture is null" };
		}

		private RectInt GetCharBounds(Texture2D tex, int xPos, int yPos, int rectWidth, int rectHeight)
		{
			RectInt result = default(RectInt);
			int num = rectWidth;
			int num2 = rectHeight;
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < rectHeight; i++)
			{
				for (int j = 0; j < rectWidth; j++)
				{
					if (tex.GetPixel(j + xPos, i + yPos).a > 0.5f)
					{
						num = Mathf.Min(j, num);
						num2 = Mathf.Min(i, num2);
						num3 = Mathf.Max(j, num3);
						num4 = Mathf.Max(i, num4);
					}
				}
			}
			if (num == rectWidth)
			{
				result = new RectInt(0, 0, rectWidth, rectHeight);
			}
			else
			{
				result.width = num3 - num + 1;
				result.height = num4 - num2 + 1;
				result.position = new Vector2Int(num, num2);
			}
			return result;
		}

		private void buildRemappingDictionary()
		{
			_remappingDictionary = new Dictionary<int, int>();
			for (int i = 0; i < remappingIndexTable.Length; i++)
			{
				_remappingDictionary[remappingIndexTable[i]] = i;
			}
		}

		public int GetRectByUtf32(int utf)
		{
			int value = utf;
			if (enableRemapping)
			{
				remappingDictionary.TryGetValue(utf, out value);
			}
			return value;
		}
	}
}

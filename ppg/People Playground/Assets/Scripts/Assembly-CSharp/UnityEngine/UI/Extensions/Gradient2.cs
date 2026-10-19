using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Effects/Extensions/Gradient2")]
	public class Gradient2 : BaseMeshEffect
	{
		public enum Type
		{
			Horizontal,
			Vertical,
			Radial,
			Diamond
		}

		public enum Blend
		{
			Override,
			Add,
			Multiply
		}

		[SerializeField]
		private Type _gradientType;

		[SerializeField]
		private Blend _blendMode = Blend.Multiply;

		[SerializeField]
		[Tooltip("Add vertices to display complex gradients. Turn off if your shape is already very complex, like text.")]
		private bool _modifyVertices = true;

		[SerializeField]
		[Range(-1f, 1f)]
		private float _offset;

		[SerializeField]
		[Range(0.1f, 10f)]
		private float _zoom = 1f;

		[SerializeField]
		private UnityEngine.Gradient _effectGradient = new UnityEngine.Gradient
		{
			colorKeys = new GradientColorKey[2]
			{
				new GradientColorKey(Color.black, 0f),
				new GradientColorKey(Color.white, 1f)
			}
		};

		public Blend BlendMode
		{
			get
			{
				return _blendMode;
			}
			set
			{
				_blendMode = value;
				base.graphic.SetVerticesDirty();
			}
		}

		public UnityEngine.Gradient EffectGradient
		{
			get
			{
				return _effectGradient;
			}
			set
			{
				_effectGradient = value;
				base.graphic.SetVerticesDirty();
			}
		}

		public Type GradientType
		{
			get
			{
				return _gradientType;
			}
			set
			{
				_gradientType = value;
				base.graphic.SetVerticesDirty();
			}
		}

		public bool ModifyVertices
		{
			get
			{
				return _modifyVertices;
			}
			set
			{
				_modifyVertices = value;
				base.graphic.SetVerticesDirty();
			}
		}

		public float Offset
		{
			get
			{
				return _offset;
			}
			set
			{
				_offset = Mathf.Clamp(value, -1f, 1f);
				base.graphic.SetVerticesDirty();
			}
		}

		public float Zoom
		{
			get
			{
				return _zoom;
			}
			set
			{
				_zoom = Mathf.Clamp(value, 0.1f, 10f);
				base.graphic.SetVerticesDirty();
			}
		}

		public override void ModifyMesh(VertexHelper helper)
		{
			if (!IsActive() || helper.currentVertCount == 0)
			{
				return;
			}
			List<UIVertex> list = new List<UIVertex>();
			helper.GetUIVertexStream(list);
			int count = list.Count;
			switch (GradientType)
			{
			case Type.Horizontal:
			case Type.Vertical:
			{
				Rect bounds2 = GetBounds(list);
				float num9 = bounds2.xMin;
				float num10 = bounds2.width;
				Func<UIVertex, float> func = (UIVertex v) => v.position.x;
				if (GradientType == Type.Vertical)
				{
					num9 = bounds2.yMin;
					num10 = bounds2.height;
					func = (UIVertex v) => v.position.y;
				}
				float num11 = ((num10 == 0f) ? 0f : (1f / num10 / Zoom));
				float num12 = (1f - 1f / Zoom) * 0.5f;
				float num13 = Offset * (1f - num12) - num12;
				if (ModifyVertices)
				{
					SplitTrianglesAtGradientStops(list, bounds2, num12, helper);
				}
				UIVertex vertex2 = default(UIVertex);
				for (int l = 0; l < helper.currentVertCount; l++)
				{
					helper.PopulateUIVertex(ref vertex2, l);
					vertex2.color = BlendColor(vertex2.color, EffectGradient.Evaluate((func(vertex2) - num9) * num11 - num13));
					helper.SetUIVertex(vertex2, l);
				}
				break;
			}
			case Type.Diamond:
			{
				Rect bounds3 = GetBounds(list);
				float num14 = ((bounds3.height == 0f) ? 0f : (1f / bounds3.height / Zoom));
				float num15 = bounds3.center.y / 2f;
				Vector3 vector = (Vector3.right + Vector3.up) * num15 + Vector3.forward * list[0].position.z;
				if (ModifyVertices)
				{
					helper.Clear();
					for (int m = 0; m < count; m++)
					{
						helper.AddVert(list[m]);
					}
					UIVertex v4 = default(UIVertex);
					v4.position = vector;
					v4.normal = list[0].normal;
					v4.uv0 = new Vector2(0.5f, 0.5f);
					v4.color = Color.white;
					helper.AddVert(v4);
					for (int n = 1; n < count; n++)
					{
						helper.AddTriangle(n - 1, n, count);
					}
					helper.AddTriangle(0, count - 1, count);
				}
				UIVertex vertex3 = default(UIVertex);
				for (int num16 = 0; num16 < helper.currentVertCount; num16++)
				{
					helper.PopulateUIVertex(ref vertex3, num16);
					vertex3.color = BlendColor(vertex3.color, EffectGradient.Evaluate(Vector3.Distance(vertex3.position, vector) * num14 - Offset));
					helper.SetUIVertex(vertex3, num16);
				}
				break;
			}
			case Type.Radial:
			{
				Rect bounds = GetBounds(list);
				float num = ((bounds.width == 0f) ? 0f : (1f / bounds.width / Zoom));
				float num2 = ((bounds.height == 0f) ? 0f : (1f / bounds.height / Zoom));
				if (ModifyVertices)
				{
					helper.Clear();
					float num3 = bounds.width / 2f;
					float num4 = bounds.height / 2f;
					UIVertex v2 = default(UIVertex);
					v2.position = Vector3.right * bounds.center.x + Vector3.up * bounds.center.y + Vector3.forward * list[0].position.z;
					v2.normal = list[0].normal;
					v2.uv0 = new Vector2(0.5f, 0.5f);
					v2.color = Color.white;
					int num5 = 64;
					for (int i = 0; i < num5; i++)
					{
						UIVertex v3 = default(UIVertex);
						float num6 = (float)i * 360f / (float)num5;
						float num7 = Mathf.Cos((float)Math.PI / 180f * num6);
						float num8 = Mathf.Sin((float)Math.PI / 180f * num6);
						v3.position = Vector3.right * num7 * num3 + Vector3.up * num8 * num4 + Vector3.forward * list[0].position.z;
						v3.normal = list[0].normal;
						v3.uv0 = new Vector2((num7 + 1f) * 0.5f, (num8 + 1f) * 0.5f);
						v3.color = Color.white;
						helper.AddVert(v3);
					}
					helper.AddVert(v2);
					for (int j = 1; j < num5; j++)
					{
						helper.AddTriangle(j - 1, j, num5);
					}
					helper.AddTriangle(0, num5 - 1, num5);
				}
				UIVertex vertex = default(UIVertex);
				for (int k = 0; k < helper.currentVertCount; k++)
				{
					helper.PopulateUIVertex(ref vertex, k);
					vertex.color = BlendColor(vertex.color, EffectGradient.Evaluate(Mathf.Sqrt(Mathf.Pow(Mathf.Abs(vertex.position.x - bounds.center.x) * num, 2f) + Mathf.Pow(Mathf.Abs(vertex.position.y - bounds.center.y) * num2, 2f)) * 2f - Offset));
					helper.SetUIVertex(vertex, k);
				}
				break;
			}
			}
		}

		private Rect GetBounds(List<UIVertex> vertices)
		{
			float num = vertices[0].position.x;
			float num2 = num;
			float num3 = vertices[0].position.y;
			float num4 = num3;
			for (int num5 = vertices.Count - 1; num5 >= 1; num5--)
			{
				float x = vertices[num5].position.x;
				float y = vertices[num5].position.y;
				if (x > num2)
				{
					num2 = x;
				}
				else if (x < num)
				{
					num = x;
				}
				if (y > num4)
				{
					num4 = y;
				}
				else if (y < num3)
				{
					num3 = y;
				}
			}
			return new Rect(num, num3, num2 - num, num4 - num3);
		}

		private void SplitTrianglesAtGradientStops(List<UIVertex> _vertexList, Rect bounds, float zoomOffset, VertexHelper helper)
		{
			List<float> list = FindStops(zoomOffset, bounds);
			if (list.Count <= 0)
			{
				return;
			}
			helper.Clear();
			int count = _vertexList.Count;
			for (int i = 0; i < count; i += 3)
			{
				float[] positions = GetPositions(_vertexList, i);
				List<int> list2 = new List<int>(3);
				List<UIVertex> list3 = new List<UIVertex>(3);
				List<UIVertex> list4 = new List<UIVertex>(2);
				for (int j = 0; j < list.Count; j++)
				{
					int currentVertCount = helper.currentVertCount;
					bool flag = list4.Count > 0;
					bool flag2 = false;
					for (int k = 0; k < 3; k++)
					{
						if (!list2.Contains(k) && positions[k] < list[j])
						{
							int num = (k + 1) % 3;
							UIVertex item = _vertexList[k + i];
							if (positions[num] > list[j])
							{
								list2.Insert(0, k);
								list3.Insert(0, item);
								flag2 = true;
							}
							else
							{
								list2.Add(k);
								list3.Add(item);
							}
						}
					}
					if (list2.Count == 0)
					{
						continue;
					}
					if (list2.Count == 3)
					{
						break;
					}
					foreach (UIVertex item3 in list3)
					{
						helper.AddVert(item3);
					}
					list4.Clear();
					foreach (int item4 in list2)
					{
						int num2 = (item4 + 1) % 3;
						if (positions[num2] < list[j])
						{
							num2 = (num2 + 1) % 3;
						}
						list4.Add(CreateSplitVertex(_vertexList[item4 + i], _vertexList[num2 + i], list[j]));
					}
					if (list4.Count == 1)
					{
						int num3 = (list2[0] + 2) % 3;
						list4.Add(CreateSplitVertex(_vertexList[list2[0] + i], _vertexList[num3 + i], list[j]));
					}
					foreach (UIVertex item5 in list4)
					{
						helper.AddVert(item5);
					}
					if (flag)
					{
						helper.AddTriangle(currentVertCount - 2, currentVertCount, currentVertCount + 1);
						helper.AddTriangle(currentVertCount - 2, currentVertCount + 1, currentVertCount - 1);
						if (list3.Count > 0)
						{
							if (flag2)
							{
								helper.AddTriangle(currentVertCount - 2, currentVertCount + 3, currentVertCount);
							}
							else
							{
								helper.AddTriangle(currentVertCount + 1, currentVertCount + 3, currentVertCount - 1);
							}
						}
					}
					else
					{
						int currentVertCount2 = helper.currentVertCount;
						helper.AddTriangle(currentVertCount, currentVertCount2 - 2, currentVertCount2 - 1);
						if (list3.Count > 1)
						{
							helper.AddTriangle(currentVertCount, currentVertCount2 - 1, currentVertCount + 1);
						}
					}
					list3.Clear();
				}
				if (list4.Count > 0)
				{
					if (list3.Count == 0)
					{
						for (int l = 0; l < 3; l++)
						{
							if (!list2.Contains(l) && positions[l] > list[list.Count - 1])
							{
								int num4 = (l + 1) % 3;
								UIVertex item2 = _vertexList[l + i];
								if (positions[num4] > list[list.Count - 1])
								{
									list3.Insert(0, item2);
								}
								else
								{
									list3.Add(item2);
								}
							}
						}
					}
					foreach (UIVertex item6 in list3)
					{
						helper.AddVert(item6);
					}
					int currentVertCount3 = helper.currentVertCount;
					if (list3.Count > 1)
					{
						helper.AddTriangle(currentVertCount3 - 4, currentVertCount3 - 2, currentVertCount3 - 1);
						helper.AddTriangle(currentVertCount3 - 4, currentVertCount3 - 1, currentVertCount3 - 3);
					}
					else if (list3.Count > 0)
					{
						helper.AddTriangle(currentVertCount3 - 3, currentVertCount3 - 1, currentVertCount3 - 2);
					}
				}
				else
				{
					helper.AddVert(_vertexList[i]);
					helper.AddVert(_vertexList[i + 1]);
					helper.AddVert(_vertexList[i + 2]);
					int currentVertCount4 = helper.currentVertCount;
					helper.AddTriangle(currentVertCount4 - 3, currentVertCount4 - 2, currentVertCount4 - 1);
				}
			}
		}

		private float[] GetPositions(List<UIVertex> _vertexList, int index)
		{
			float[] array = new float[3];
			if (GradientType == Type.Horizontal)
			{
				array[0] = _vertexList[index].position.x;
				array[1] = _vertexList[index + 1].position.x;
				array[2] = _vertexList[index + 2].position.x;
			}
			else
			{
				array[0] = _vertexList[index].position.y;
				array[1] = _vertexList[index + 1].position.y;
				array[2] = _vertexList[index + 2].position.y;
			}
			return array;
		}

		private List<float> FindStops(float zoomOffset, Rect bounds)
		{
			List<float> list = new List<float>();
			float num = Offset * (1f - zoomOffset);
			float num2 = zoomOffset - num;
			float num3 = 1f - zoomOffset - num;
			GradientColorKey[] colorKeys = EffectGradient.colorKeys;
			for (int i = 0; i < colorKeys.Length; i++)
			{
				GradientColorKey gradientColorKey = colorKeys[i];
				if (gradientColorKey.time >= num3)
				{
					break;
				}
				if (gradientColorKey.time > num2)
				{
					list.Add((gradientColorKey.time - num2) * Zoom);
				}
			}
			GradientAlphaKey[] alphaKeys = EffectGradient.alphaKeys;
			for (int i = 0; i < alphaKeys.Length; i++)
			{
				GradientAlphaKey gradientAlphaKey = alphaKeys[i];
				if (gradientAlphaKey.time >= num3)
				{
					break;
				}
				if (gradientAlphaKey.time > num2)
				{
					list.Add((gradientAlphaKey.time - num2) * Zoom);
				}
			}
			float num4 = bounds.xMin;
			float num5 = bounds.width;
			if (GradientType == Type.Vertical)
			{
				num4 = bounds.yMin;
				num5 = bounds.height;
			}
			list.Sort();
			for (int j = 0; j < list.Count; j++)
			{
				list[j] = list[j] * num5 + num4;
				if (j > 0 && Math.Abs(list[j] - list[j - 1]) < 2f)
				{
					list.RemoveAt(j);
					j--;
				}
			}
			return list;
		}

		private UIVertex CreateSplitVertex(UIVertex vertex1, UIVertex vertex2, float stop)
		{
			if (GradientType == Type.Horizontal)
			{
				float num = vertex1.position.x - stop;
				float num2 = vertex1.position.x - vertex2.position.x;
				float num3 = vertex1.position.y - vertex2.position.y;
				float num4 = vertex1.uv0.x - vertex2.uv0.x;
				float num5 = vertex1.uv0.y - vertex2.uv0.y;
				float num6 = num / num2;
				float y = vertex1.position.y - num3 * num6;
				UIVertex result = default(UIVertex);
				result.position = new Vector3(stop, y, vertex1.position.z);
				result.normal = vertex1.normal;
				result.uv0 = new Vector2(vertex1.uv0.x - num4 * num6, vertex1.uv0.y - num5 * num6);
				result.color = Color.white;
				return result;
			}
			float num7 = vertex1.position.y - stop;
			float num8 = vertex1.position.y - vertex2.position.y;
			float num9 = vertex1.position.x - vertex2.position.x;
			float num10 = vertex1.uv0.x - vertex2.uv0.x;
			float num11 = vertex1.uv0.y - vertex2.uv0.y;
			float num12 = num7 / num8;
			float x = vertex1.position.x - num9 * num12;
			UIVertex result2 = default(UIVertex);
			result2.position = new Vector3(x, stop, vertex1.position.z);
			result2.normal = vertex1.normal;
			result2.uv0 = new Vector2(vertex1.uv0.x - num10 * num12, vertex1.uv0.y - num11 * num12);
			result2.color = Color.white;
			return result2;
		}

		private Color BlendColor(Color colorA, Color colorB)
		{
			switch (BlendMode)
			{
			default:
				return colorB;
			case Blend.Add:
				return colorA + colorB;
			case Blend.Multiply:
				return colorA * colorB;
			}
		}
	}
}

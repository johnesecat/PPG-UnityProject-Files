using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace Linefy
{
	[PreferBinarySerialization]
	[HelpURL("https://polyflow.xyz/content/linefy/documentation-1-1/linefy-documentation.html#SerializedPolygonalMesh")]
	public class SerializedPolygonalMesh : ScriptableObject
	{
		[Serializable]
		public struct SPM_Position
		{
			public Vector3 positionValue;

			public List<int> linkedVertices;

			public List<int> adjacentPolygons;

			public List<int> adjacentEdges;

			public SPM_Position(Vector3 pos)
			{
				positionValue = pos;
				linkedVertices = new List<int>(4);
				adjacentPolygons = new List<int>(4);
				adjacentEdges = new List<int>(4);
			}
		}

		[Serializable]
		public struct SPM_Material
		{
			public string name;

			private int hash;

			public SPM_Material(string name)
			{
				this.name = name;
				hash = this.name.GetHashCode();
			}

			public override int GetHashCode()
			{
				return hash;
			}

			public override bool Equals(object obj)
			{
				return ((SPM_Material)obj).name == name;
			}
		}

		[Serializable]
		public struct SPM_Edge
		{
			public int a;

			public int b;

			private int hash;

			private string strHash;

			public SPM_Edge(int a, int b)
			{
				this.a = Mathf.Min(a, b);
				this.b = Mathf.Max(a, b);
				strHash = $"{this.a} | {this.b}";
				hash = strHash.GetHashCode();
			}

			public override int GetHashCode()
			{
				return hash;
			}

			public override bool Equals(object obj)
			{
				return ((SPM_Edge)obj).strHash == strHash;
			}
		}

		[Serializable]
		public struct SPM_Polygon
		{
			public int materialId;

			public int smoothingGroup;

			public int[] corners;

			public SPM_Polygon(int materialId, int smoothingGroup, int cornersCount)
			{
				this.materialId = materialId;
				this.smoothingGroup = smoothingGroup;
				corners = new int[cornersCount];
			}

			public void FlipNormals()
			{
				Array.Reverse((Array)corners);
			}
		}

		[Serializable]
		public struct SPM_Vertex
		{
			public int posIdx;

			public int normIdx;

			public int uvIdx;

			public int colorIdx;

			private string strHash;

			private int hash;

			public SPM_Vertex(int posIdx, int normalIdx, int uvIdx, int colorIdx)
			{
				this.posIdx = posIdx;
				normIdx = normalIdx;
				this.uvIdx = uvIdx;
				this.colorIdx = colorIdx;
				strHash = $"{posIdx} {normIdx} {uvIdx} {colorIdx}";
				hash = strHash.GetHashCode();
			}

			public override int GetHashCode()
			{
				return hash;
			}

			public override bool Equals(object obj)
			{
				return ((SPM_Vertex)obj).strHash == strHash;
			}
		}

		[Serializable]
		public struct SPM_Normal
		{
			public int parentPos;

			public int smoothingGroupIdx;

			public List<int> adjacentPolygons;

			public List<int> linkedVertices;

			private string strHash;

			private int hash;

			public SPM_Normal(int parentPos, int smoothingGroup)
			{
				this.parentPos = parentPos;
				smoothingGroupIdx = smoothingGroup;
				adjacentPolygons = new List<int>();
				linkedVertices = new List<int>();
				strHash = $"{parentPos} | {smoothingGroupIdx}";
				hash = strHash.GetHashCode();
			}

			public override int GetHashCode()
			{
				return hash;
			}

			public override bool Equals(object obj)
			{
				if (smoothingGroupIdx < 0)
				{
					return false;
				}
				return ((SPM_Normal)obj).strHash == strHash;
			}
		}

		[Serializable]
		public struct SPM_uv
		{
			public Vector2 uvValue;

			public List<int> linkedVertices;

			public SPM_uv(Vector2 uv)
			{
				uvValue = uv;
				linkedVertices = new List<int>(4);
			}
		}

		[Serializable]
		public struct SPM_color
		{
			public Color colorValue;

			public List<int> linkedVertices;

			public SPM_color(Color color)
			{
				colorValue = color;
				linkedVertices = new List<int>(4);
			}
		}

		private enum ObjLineIdEnum
		{
			v,
			vt,
			f,
			s,
			s_off,
			usemtl,
			other
		}

		private class FaceLineParser
		{
			public class Corner
			{
				public int posIdx = -1;

				public int uvIdx = -1;

				public int normalIdx = -1;

				public List<char>[] chars = new List<char>[3]
				{
					new List<char>(),
					new List<char>(),
					new List<char>()
				};

				public int ci;

				public void Parse()
				{
					if (chars[0].Count > 0)
					{
						int.TryParse(new string(chars[0].ToArray()), out posIdx);
						posIdx--;
					}
					if (chars[1].Count > 0)
					{
						int.TryParse(new string(chars[1].ToArray()), out uvIdx);
						uvIdx--;
					}
					if (chars[2].Count > 0)
					{
						int.TryParse(new string(chars[2].ToArray()), out normalIdx);
						normalIdx--;
					}
				}
			}

			public List<Corner> corners = new List<Corner>();

			public void Parse(string str)
			{
				corners.Clear();
				char[] array = str.ToCharArray();
				Corner corner = null;
				for (int i = 2; i < array.Length; i++)
				{
					char c = array[i];
					if (char.IsDigit(c))
					{
						if (corner == null)
						{
							corner = new Corner();
						}
						corner.chars[corner.ci].Add(c);
					}
					if (array[i] == '/')
					{
						corner.ci++;
					}
					else if (array[i] == ' ' && corner != null)
					{
						corners.Add(corner);
						corner = null;
					}
				}
				if (corner != null)
				{
					corners.Add(corner);
				}
				for (int j = 0; j < corners.Count; j++)
				{
					corners[j].Parse();
				}
			}

			public void PrintDebug()
			{
				string text = $"corners count {corners.Count} ";
				for (int i = 0; i < corners.Count; i++)
				{
					text += $" {corners[i].posIdx}/{corners[i].uvIdx}/{corners[i].normalIdx} ";
				}
				Debug.Log(text);
			}
		}

		public SPM_Position[] positions = new SPM_Position[0];

		public SPM_uv[] uvs = new SPM_uv[0];

		public SPM_color[] colors = new SPM_color[0];

		public SPM_Normal[] normals = new SPM_Normal[0];

		public SPM_Polygon[] polygons = new SPM_Polygon[0];

		public SPM_Edge[] positionEdges = new SPM_Edge[0];

		public SPM_Material[] materials = new SPM_Material[0];

		public SPM_Vertex[] vertices = new SPM_Vertex[0];

		public int trianglesCount;

		public ModificationInfo modificationInfo;

		private ObjLineIdEnum GetObjLineId(string str)
		{
			char[] array = str.ToCharArray();
			if (array.Length < 2)
			{
				return ObjLineIdEnum.other;
			}
			if (array[0] == 'v' && array[1] == ' ')
			{
				return ObjLineIdEnum.v;
			}
			if (array[0] == 'v' && array[1] == 't')
			{
				return ObjLineIdEnum.vt;
			}
			if (array[0] == 'f' && array[1] == ' ')
			{
				return ObjLineIdEnum.f;
			}
			if (array[0] == 's' && array[1] == ' ')
			{
				return ObjLineIdEnum.s;
			}
			if (array[0] == 'u' && array[1] == 's' && array[2] == 'e' && array[3] == 'm' && array[4] == 't' && array[5] == 'l')
			{
				return ObjLineIdEnum.usemtl;
			}
			if (array[0] == 's' && array[1] == ' ' && array[2] == 'o' && array[3] == 'f' && array[4] == 'f')
			{
				return ObjLineIdEnum.s_off;
			}
			return ObjLineIdEnum.other;
		}

		private static float ToFloat(string s)
		{
			return float.Parse(s, CultureInfo.InvariantCulture);
		}

		private int ToInt(string s)
		{
			int result = 0;
			int.TryParse(s, out result);
			return result;
		}

		private void OnEOF(TextReader sr)
		{
			sr.Close();
		}

		public void ReadObjFromTextReader(TextReader textReader, SmoothingGroupsImportMode sgMode, bool flipNormals, float scaleFactor, bool swapYZ)
		{
			char[] separator = " ".ToCharArray();
			FaceLineParser faceLineParser = new FaceLineParser();
			List<Vector3> list = new List<Vector3>();
			List<Vector2> list2 = new List<Vector2>();
			List<Polygon> list3 = new List<Polygon>();
			int materialId = -1;
			int num = -1;
			HashedCollection<string> hashedCollection = new HashedCollection<string>(null);
			while (true)
			{
				string text = textReader.ReadLine();
				if (text == null)
				{
					break;
				}
				switch (GetObjLineId(text))
				{
				case ObjLineIdEnum.vt:
				{
					string[] array = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
					Vector2 item3 = new Vector2(ToFloat(array[1]), ToFloat(array[2]));
					list2.Add(item3);
					break;
				}
				case ObjLineIdEnum.v:
				{
					string[] array = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
					Vector3 item4 = new Vector3(ToFloat(array[1]), ToFloat(array[2]), ToFloat(array[3]));
					if (swapYZ)
					{
						item4.Set(item4.x, item4.z, item4.y);
					}
					item4 *= scaleFactor;
					list.Add(item4);
					break;
				}
				case ObjLineIdEnum.usemtl:
				{
					string item2 = text.Remove(0, 7);
					materialId = hashedCollection.FindOrAddIdx(item2);
					break;
				}
				case ObjLineIdEnum.s_off:
					num = -1;
					break;
				case ObjLineIdEnum.s:
					num = ToInt(text.Remove(0, 2));
					break;
				case ObjLineIdEnum.f:
				{
					faceLineParser.Parse(text);
					int sg = num;
					switch (sgMode)
					{
					case SmoothingGroupsImportMode.PerPolygon:
						sg = -1;
						break;
					case SmoothingGroupsImportMode.ForceSmoothAll:
						sg = 0;
						break;
					}
					Polygon item = new Polygon(sg, materialId, faceLineParser.corners.Count);
					for (int i = 0; i < faceLineParser.corners.Count; i++)
					{
						int posIdx = faceLineParser.corners[i].posIdx;
						int uvIdx = faceLineParser.corners[i].uvIdx;
						item.corners[i] = new PolygonCorner(posIdx, uvIdx, -1);
					}
					if (flipNormals)
					{
						Array.Reverse((Array)item.corners);
					}
					list3.Add(item);
					break;
				}
				}
			}
			OnEOF(textReader);
			BuildProcedural(list.ToArray(), list2.ToArray(), null, list3.ToArray());
			modificationInfo = new ModificationInfo("imported from TextReader");
		}

		public void ReadObjFromFile(string filePath, SmoothingGroupsImportMode sgMode, bool flipNormals, float scaleFactor, bool swapYZ)
		{
			if (!new FileInfo(filePath).Exists)
			{
				Debug.LogWarningFormat("obj file not found {0} ", filePath);
				return;
			}
			using (TextReader textReader = File.OpenText(filePath))
			{
				ReadObjFromTextReader(textReader, sgMode, flipNormals, scaleFactor, swapYZ);
			}
			modificationInfo = new ModificationInfo($"imported {filePath}");
		}

		public void Clear()
		{
			positions = new SPM_Position[0];
			normals = new SPM_Normal[0];
			uvs = new SPM_uv[0];
			polygons = new SPM_Polygon[0];
			positionEdges = new SPM_Edge[0];
			materials = new SPM_Material[0];
			vertices = new SPM_Vertex[0];
			trianglesCount = 0;
		}

		public void BuildProcedural(Vector3[] posData, Vector2[] uvsData, Color[] colorsData, Polygon[] _polygonsData)
		{
			Clear();
			if (colorsData == null || colorsData.Length == 0)
			{
				colorsData = new Color[1] { Color.white };
			}
			if (uvsData == null || uvsData.Length == 0)
			{
				uvsData = new Vector2[1] { Vector2.zero };
			}
			if (posData == null || posData.Length == 0)
			{
				posData = new Vector3[1] { Vector3.zero };
			}
			List<Polygon> list = new List<Polygon>();
			for (int i = 0; i < _polygonsData.Length; i++)
			{
				if (_polygonsData[i].isValid)
				{
					list.Add(_polygonsData[i]);
				}
			}
			Polygon[] array = list.ToArray();
			for (int j = 0; j < array.Length; j++)
			{
				array[j].ClampCornerIndices(posData.Length - 1, colorsData.Length - 1, uvsData.Length - 1);
			}
			positions = new SPM_Position[posData.Length];
			for (int k = 0; k < posData.Length; k++)
			{
				positions[k] = new SPM_Position(posData[k]);
			}
			polygons = new SPM_Polygon[array.Length];
			HashedCollection<SPM_Normal> hashedCollection = new HashedCollection<SPM_Normal>(null);
			HashedCollection<SPM_Vertex> hashedCollection2 = new HashedCollection<SPM_Vertex>(null);
			for (int l = 0; l < array.Length; l++)
			{
				Polygon polygon = array[l];
				SPM_Polygon sPM_Polygon = new SPM_Polygon(polygon.materialId, polygon.smoothingGroup, polygon.CornersCount);
				for (int m = 0; m < polygon.CornersCount; m++)
				{
					int position = polygon[m].position;
					SPM_Normal item = new SPM_Normal(position, polygon.smoothingGroup);
					int num = hashedCollection.FindOrAddIdx(item);
					item = hashedCollection[num];
					item.adjacentPolygons.Add(l);
					hashedCollection[num] = item;
					SPM_Vertex item2 = new SPM_Vertex(position, num, polygon[m].uv, polygon[m].color);
					int num2 = hashedCollection2.FindOrAddIdx(item2);
					positions[position].adjacentPolygons.Add(l);
					positions[position].linkedVertices.Add(num2);
					sPM_Polygon.corners[m] = num2;
				}
				polygons[l] = sPM_Polygon;
			}
			normals = hashedCollection.ToArray();
			vertices = hashedCollection2.ToArray();
			for (int n = 0; n < vertices.Length; n++)
			{
				normals[vertices[n].normIdx].linkedVertices.Add(n);
			}
			uvs = new SPM_uv[uvsData.Length];
			for (int num3 = 0; num3 < uvsData.Length; num3++)
			{
				uvs[num3] = new SPM_uv(uvsData[num3]);
			}
			for (int num4 = 0; num4 < vertices.Length; num4++)
			{
				uvs[vertices[num4].uvIdx].linkedVertices.Add(num4);
			}
			colors = new SPM_color[colorsData.Length];
			for (int num5 = 0; num5 < colorsData.Length; num5++)
			{
				colors[num5] = new SPM_color(colorsData[num5]);
			}
			for (int num6 = 0; num6 < vertices.Length; num6++)
			{
				colors[vertices[num6].colorIdx].linkedVertices.Add(num6);
			}
			trianglesCount = 0;
			HashedCollection<SPM_Edge> hashedCollection3 = new HashedCollection<SPM_Edge>(null);
			for (int num7 = 0; num7 < array.Length; num7++)
			{
				int num8 = polygons[num7].corners.Length;
				trianglesCount += num8 - 2;
				for (int num9 = 0; num9 < num8; num9++)
				{
					int posIdx = vertices[polygons[num7].corners[num9]].posIdx;
					int posIdx2 = vertices[polygons[num7].corners[(num9 + 1) % num8]].posIdx;
					int item3 = hashedCollection3.FindOrAddIdx(new SPM_Edge(posIdx, posIdx2));
					positions[posIdx].adjacentEdges.Add(item3);
					positions[posIdx2].adjacentEdges.Add(item3);
				}
			}
			positionEdges = hashedCollection3.ToArray();
			modificationInfo = new ModificationInfo("Built procedurally");
		}

		public static SerializedPolygonalMesh GetProcedural(Vector3[] positions, Vector2[] uvs, Color[] colors, Polygon[] polygons)
		{
			SerializedPolygonalMesh serializedPolygonalMesh = ScriptableObject.CreateInstance<SerializedPolygonalMesh>();
			serializedPolygonalMesh.BuildProcedural(positions, uvs, colors, polygons);
			return serializedPolygonalMesh;
		}
	}
}

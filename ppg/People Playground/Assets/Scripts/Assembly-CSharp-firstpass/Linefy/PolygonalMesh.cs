using System;
using Linefy.Internal;
using Linefy.Serialization;
using UnityEngine;

namespace Linefy
{
	public class PolygonalMesh : LinefyDrawcall
	{
		public class PositionEdge
		{
			public bool positionsDirty = true;

			public Position a;

			public Position b;

			public PositionEdge(Position a, Position b)
			{
				this.a = a;
				this.b = b;
			}
		}

		public class Vertex
		{
			public int meshIndex;

			public Position pos;

			public Normal norm;

			public UV uv;

			public VColor color;
		}

		public class Face
		{
			public Vertex a;

			public Vertex b;

			public Vertex c;

			private Vector3 normal;

			public Vector3 RecalculateNormal()
			{
				Vector3 vector = a.pos;
				Vector3 vector2 = b.pos;
				Vector3 vector3 = c.pos;
				normal = Vector3.Cross(vector2 - vector, vector3 - vector);
				return normal;
			}

			public void SetVertices(Vertex a, Vertex b, Vertex c)
			{
				this.a = a;
				this.b = b;
				this.c = c;
			}
		}

		public class PM_Polygon
		{
			private struct TriangulationCorner
			{
				public bool isUsed;

				public Vector2 position;
			}

			public int smoothingGroup;

			public int materialId;

			private bool flatPolygon;

			public bool triangulationDirty;

			public bool normalsDirty;

			public Vertex[] corners;

			private TriangulationCorner[] tcorners;

			public Face[] nonConvexFaces;

			public Face[] convexFaces;

			public Vector3 normal;

			public int trisArrayIndicesOffset;

			private Vertex cross00;

			private Vertex cross01;

			private Vertex cross10;

			private Vertex cross11;

			public PM_Polygon(int cornersCount, ref int trisIndicesCounter, int smoothingGroup, int materialId)
			{
				corners = new Vertex[cornersCount];
				tcorners = new TriangulationCorner[cornersCount];
				trisArrayIndicesOffset = trisIndicesCounter;
				nonConvexFaces = new Face[cornersCount - 2];
				convexFaces = new Face[nonConvexFaces.Length];
				for (int i = 0; i < nonConvexFaces.Length; i++)
				{
					nonConvexFaces[i] = new Face();
					convexFaces[i] = new Face();
				}
				this.smoothingGroup = smoothingGroup;
				this.materialId = materialId;
				trisIndicesCounter += nonConvexFaces.Length * 3;
				triangulationDirty = true;
				normalsDirty = true;
			}

			public void TriangulateNonConvex(int[] mTriangles)
			{
				triangulationDirty = false;
				Vector3 zero = Vector3.zero;
				float num = 1f / (float)corners.Length;
				for (int i = 0; i < corners.Length; i++)
				{
					zero += corners[i].pos.positionValue * num;
				}
				if ((zero - corners[0].pos.positionValue).magnitude < 0.001f)
				{
					fillTrisIndices(mTriangles, convexFaces);
					return;
				}
				RecalculateNormalUnweighted();
				Vector3 normalized = (corners[0].pos.positionValue - zero).normalized;
				Matrix4x4 matrix4x = Matrix4x4Utility.UnscaledTRSInverse(zero, normal, normalized);
				for (int j = 0; j < tcorners.Length; j++)
				{
					tcorners[j].isUsed = false;
					tcorners[j].position = matrix4x.MultiplyPoint3x4(corners[j].pos.positionValue);
				}
				for (int k = 0; k < nonConvexFaces.Length; k++)
				{
					int t = 0;
					int t2 = 0;
					int t3 = 0;
					Face face = nonConvexFaces[k];
					if (FindAllowedTriangleHigh(ref t, ref t2, ref t3))
					{
						tcorners[t2].isUsed = true;
						face.a = corners[t];
						face.b = corners[t2];
						face.c = corners[t3];
						continue;
					}
					fillTrisIndices(mTriangles, convexFaces);
					return;
				}
				fillTrisIndices(mTriangles, nonConvexFaces);
			}

			private bool FindAllowedTriangle(ref int t0, ref int t1, ref int t2)
			{
				for (int i = 0; i < tcorners.Length; i++)
				{
					if (!tcorners[i].isUsed)
					{
						int unusedAdjacentTCorner = GetUnusedAdjacentTCorner(i, -1);
						int unusedAdjacentTCorner2 = GetUnusedAdjacentTCorner(i, 1);
						Vector2 position = tcorners[unusedAdjacentTCorner].position;
						Vector2 position2 = tcorners[i].position;
						if (Vector2Unility.SignedAngle(dirB: tcorners[unusedAdjacentTCorner2].position - position2, dirA: position - position2) < 180f && isValidTriangulationFace(unusedAdjacentTCorner, i, unusedAdjacentTCorner2))
						{
							t0 = unusedAdjacentTCorner;
							t1 = i;
							t2 = unusedAdjacentTCorner2;
							return true;
						}
					}
				}
				return false;
			}

			private bool FindAllowedTriangleHigh(ref int t0, ref int t1, ref int t2)
			{
				float num = float.MaxValue;
				bool result = false;
				for (int i = 0; i < tcorners.Length; i++)
				{
					if (!tcorners[i].isUsed)
					{
						int unusedAdjacentTCorner = GetUnusedAdjacentTCorner(i, -1);
						int unusedAdjacentTCorner2 = GetUnusedAdjacentTCorner(i, 1);
						Vector2 position = tcorners[unusedAdjacentTCorner].position;
						Vector2 position2 = tcorners[i].position;
						float num2 = Vector2Unility.SignedAngle(dirB: tcorners[unusedAdjacentTCorner2].position - position2, dirA: position - position2);
						if (num2 < num && isValidTriangulationFace(unusedAdjacentTCorner, i, unusedAdjacentTCorner2))
						{
							t0 = unusedAdjacentTCorner;
							t1 = i;
							t2 = unusedAdjacentTCorner2;
							num = num2;
							result = true;
						}
					}
				}
				return result;
			}

			private int GetUnusedAdjacentTCorner(int idx, int sign)
			{
				for (int i = 1; i < tcorners.Length; i++)
				{
					int idx2 = idx + i * sign;
					idx2 = MathUtility.RoundedArrayIdx(idx2, tcorners.Length);
					if (!tcorners[idx2].isUsed)
					{
						return idx2;
					}
				}
				Debug.Log("Not found!");
				return 0;
			}

			private bool isValidTriangulationFace(int ta, int tb, int tc)
			{
				Vector2 position = tcorners[ta].position;
				Vector2 position2 = tcorners[tb].position;
				Vector2 position3 = tcorners[tc].position;
				for (int i = 0; i < tcorners.Length; i++)
				{
					if (i != ta && i != tb && i != tc)
					{
						Vector2 position4 = tcorners[i].position;
						if (Triangle2D.PointTestDoublesided(position, position2, position3, position4))
						{
							return false;
						}
					}
				}
				return true;
			}

			public void RecalculateNormal(NormalsRecalculationMode mode)
			{
				if (mode == NormalsRecalculationMode.Unweighted)
				{
					RecalculateNormalUnweighted();
				}
				else
				{
					RecalculateNormalWeighted();
				}
				normalsDirty = false;
			}

			private void RecalculateNormalUnweighted()
			{
				normal = Vector3.Cross(cross01.pos.positionValue - cross00.pos.positionValue, cross11.pos.positionValue - cross10.pos.positionValue).normalized;
			}

			private void RecalculateNormalWeighted()
			{
				normal.Set(0f, 0f, 0f);
				for (int i = 0; i < nonConvexFaces.Length; i++)
				{
					normal += convexFaces[i].RecalculateNormal();
				}
			}

			public void TriangulateConvex(int[] mTriangles)
			{
				int num = 0;
				int num2 = 1;
				int num3 = corners.Length - 1;
				bool flag = true;
				for (int i = 0; i < convexFaces.Length; i++)
				{
					Face obj = convexFaces[i];
					obj.c = corners[num2];
					obj.b = corners[num];
					obj.a = corners[num3];
					if (flag)
					{
						num = num2;
						num2++;
					}
					else
					{
						num = num3;
						num3--;
					}
					flag = !flag;
				}
				fillTrisIndices(mTriangles, convexFaces);
			}

			private void fillTrisIndices(int[] mTriangles, Face[] _faces)
			{
				for (int i = 0; i < _faces.Length; i++)
				{
					Face face = _faces[i];
					int num = i * 3;
					int num2 = num + 1;
					int num3 = num + 2;
					mTriangles[trisArrayIndicesOffset + num] = face.a.meshIndex;
					mTriangles[trisArrayIndicesOffset + num2] = face.b.meshIndex;
					mTriangles[trisArrayIndicesOffset + num3] = face.c.meshIndex;
				}
			}

			public void SetNormalCross()
			{
				if (corners.Length == 3)
				{
					cross00 = corners[0];
					cross01 = corners[1];
					cross10 = corners[1];
					cross11 = corners[2];
				}
				else if (corners.Length == 4 || corners.Length == 5)
				{
					cross00 = corners[0];
					cross01 = corners[2];
					cross10 = corners[1];
					cross11 = corners[3];
				}
				else if (corners.Length == 6 || corners.Length == 7)
				{
					cross00 = corners[0];
					cross01 = corners[3];
					cross10 = corners[2];
					cross11 = corners[5];
				}
				else
				{
					int num = corners.Length / 4;
					cross00 = corners[0];
					cross01 = corners[num * 2];
					cross10 = corners[num];
					cross11 = corners[num * 3];
				}
			}

			private string info()
			{
				string text = $"polygon cornersCount:{corners.Length} positions: ";
				for (int i = 0; i < corners.Length; i++)
				{
					text += $"{corners[i].pos.positionValue}, ";
				}
				return text;
			}
		}

		public class Position
		{
			public int idx;

			public Vector3 positionValue;

			public Vertex[] linkedVertices;

			internal PM_Polygon[] adjacentPolygons;

			public PositionEdge[] adjacentEdges;

			public Position(int id)
			{
				idx = id;
			}

			public static implicit operator Vector3(Position pv)
			{
				return pv.positionValue;
			}
		}

		public class Normal
		{
			public bool dirty;

			public PM_Polygon[] adjacentPolygons;

			public Vertex[] linkedVertices;

			public Vector3 normal;

			public Normal(int adjacentCount)
			{
				adjacentPolygons = new PM_Polygon[adjacentCount];
			}

			public void Recalculate()
			{
				normal.Set(0f, 0f, 0f);
				for (int i = 0; i < adjacentPolygons.Length; i++)
				{
					normal += adjacentPolygons[i].normal;
				}
				dirty = false;
			}

			public static implicit operator Vector3(Normal pv)
			{
				return pv.normal;
			}
		}

		public class UV
		{
			public int idx;

			public Vertex[] linkedVertices;

			public Vector2 uvValue;

			public UV(int idx, Vector2 value)
			{
				this.idx = idx;
				uvValue = value;
			}
		}

		public class VColor
		{
			public int idx;

			public Vertex[] linkedVertices;

			public Color colorValue;

			public VColor(int idx, Color color)
			{
				this.idx = idx;
				colorValue = color;
			}
		}

		private bool formIsDirty;

		private bool mTrianglesDirty;

		private bool mPositionsDirty;

		private bool mUVDirty;

		private bool mColorDirty;

		private bool mNormalsDirty;

		private int[] mTriangles;

		private Vector3[] mPositions;

		private Vector2[] mUVs;

		private Color[] mColors;

		private Vector3[] mNormals;

		private bool _autoRecalculateBounds = true;

		private Lines _positionWireframe;

		protected PM_Polygon[] polygons = new PM_Polygon[0];

		private PM_Polygon[] dynamicalyTriangulatedPolygons = new PM_Polygon[0];

		protected Position[] positions = new Position[0];

		protected Normal[] normals;

		protected UV[] uvs;

		protected VColor[] colors;

		protected Vertex[] vertices;

		protected PositionEdge[] positionEdges = new PositionEdge[0];

		public ModificationInfo modificationInfo;

		private int _dynamicTriangulationThreshold = 5;

		private float _ambient = 1f;

		private bool _doublesided = true;

		private Vector4 _textureTransform = new Vector4(1f, 1f, 0f, 0f);

		private LightingMode _lightingMode = LightingMode.Lit;

		private NormalsRecalculationMode _normalsRecalculationMode;

		public bool autoRecalculateBounds
		{
			get
			{
				return _autoRecalculateBounds;
			}
			set
			{
				if (_autoRecalculateBounds != value)
				{
					if (_autoRecalculateBounds)
					{
						formIsDirty = true;
					}
					_autoRecalculateBounds = value;
				}
			}
		}

		public Lines positionEdgesWireframe
		{
			get
			{
				return _positionWireframe;
			}
			set
			{
				if (value != null && value != _positionWireframe)
				{
					formIsDirty = true;
				}
				_positionWireframe = value;
			}
		}

		public int dynamicTriangulationThreshold
		{
			get
			{
				return _dynamicTriangulationThreshold;
			}
			set
			{
				if (_dynamicTriangulationThreshold != value)
				{
					_dynamicTriangulationThreshold = Mathf.Max(4, value);
					OnTriangulationParamsChanged();
				}
			}
		}

		public float ambient
		{
			get
			{
				return _ambient;
			}
			set
			{
				if (_ambient != value)
				{
					_ambient = value;
					base.material.SetFloat("_Ambient", _ambient);
				}
			}
		}

		public bool doublesided
		{
			get
			{
				return _doublesided;
			}
			set
			{
				if (_doublesided != value)
				{
					_doublesided = value;
					base.material.SetFloat("_Culling", (!_doublesided) ? 1 : 0);
				}
			}
		}

		public Vector4 textureTransform
		{
			get
			{
				return _textureTransform;
			}
			set
			{
				if (_textureTransform != value)
				{
					_textureTransform = value;
					base.material.SetVector("_TextureTransform", _textureTransform);
				}
			}
		}

		public LightingMode lighingMode
		{
			get
			{
				return _lightingMode;
			}
			set
			{
				if (value == _lightingMode)
				{
					return;
				}
				_lightingMode = value;
				formIsDirty = true;
				mNormalsDirty = true;
				if (_lightingMode > LightingMode.Unlit)
				{
					PM_Polygon[] array = polygons;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].normalsDirty = true;
					}
				}
			}
		}

		public NormalsRecalculationMode normalsRecalculationMode
		{
			get
			{
				return _normalsRecalculationMode;
			}
			set
			{
				if (value != _normalsRecalculationMode)
				{
					_normalsRecalculationMode = value;
					PM_Polygon[] array = polygons;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].normalsDirty = true;
					}
					formIsDirty = true;
				}
			}
		}

		public int positionsCount => positions.Length;

		public int positionEdgesCount => positionEdges.Length;

		private int dynamiclyTriangulatedPolygonsCount => dynamicalyTriangulatedPolygons.Length;

		public Mesh generatedMesh => mesh;

		protected override void SetDirtyAttributes()
		{
			boundsDirty = true;
			mTrianglesDirty = true;
			mPositionsDirty = true;
			mUVDirty = true;
			mNormalsDirty = true;
			mColorDirty = true;
			formIsDirty = true;
		}

		private void OnTriangulationParamsChanged()
		{
			dynamicalyTriangulatedPolygons = Array.FindAll(polygons, (PM_Polygon p) => p.corners.Length >= _dynamicTriangulationThreshold);
			PM_Polygon[] array = dynamicalyTriangulatedPolygons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].triangulationDirty = true;
			}
			formIsDirty = true;
		}

		public PolygonalMesh(SerializedPolygonalMesh serializableData)
		{
			BuildFromSPM(serializableData);
		}

		public PolygonalMesh(Vector3[] posData, Vector2[] uvsData, Color[] colorsData, Polygon[] polygonsData)
		{
			SerializedPolygonalMesh procedural = SerializedPolygonalMesh.GetProcedural(posData, uvsData, colorsData, polygonsData);
			BuildFromSPM(procedural);
		}

		public PolygonalMesh(Vector3[] posData, Vector2[] uvsData, Polygon[] polygonsData)
		{
			SerializedPolygonalMesh procedural = SerializedPolygonalMesh.GetProcedural(posData, uvsData, null, polygonsData);
			BuildFromSPM(procedural);
			UnityEngine.Object.DestroyImmediate(procedural);
		}

		public PolygonalMesh(Vector3[] posData, Polygon[] polygonsData)
		{
			SerializedPolygonalMesh procedural = SerializedPolygonalMesh.GetProcedural(posData, null, null, polygonsData);
			BuildFromSPM(procedural);
			UnityEngine.Object.DestroyImmediate(procedural);
		}

		public void BuildFromSPM(SerializedPolygonalMesh spm)
		{
			if (!(mesh == null))
			{
				mesh.Clear();
			}
			modificationInfo = spm.modificationInfo;
			positions = new Position[spm.positions.Length];
			for (int i = 0; i < positions.Length; i++)
			{
				positions[i] = new Position(i);
			}
			uvs = new UV[spm.uvs.Length];
			for (int j = 0; j < spm.uvs.Length; j++)
			{
				uvs[j] = new UV(j, spm.uvs[j].uvValue);
			}
			colors = new VColor[spm.colors.Length];
			for (int k = 0; k < spm.colors.Length; k++)
			{
				colors[k] = new VColor(k, spm.colors[k].colorValue);
			}
			vertices = new Vertex[spm.vertices.Length];
			for (int l = 0; l < vertices.Length; l++)
			{
				SerializedPolygonalMesh.SPM_Vertex sPM_Vertex = spm.vertices[l];
				Vertex vertex = new Vertex();
				vertex.meshIndex = l;
				vertex.pos = positions[sPM_Vertex.posIdx];
				vertex.uv = uvs[sPM_Vertex.uvIdx];
				vertex.color = colors[sPM_Vertex.colorIdx];
				vertices[l] = vertex;
			}
			for (int m = 0; m < uvs.Length; m++)
			{
				SerializedPolygonalMesh.SPM_uv sPM_uv = spm.uvs[m];
				UV uV = uvs[m];
				uV.linkedVertices = new Vertex[sPM_uv.linkedVertices.Count];
				for (int n = 0; n < uV.linkedVertices.Length; n++)
				{
					uV.linkedVertices[n] = vertices[sPM_uv.linkedVertices[n]];
				}
			}
			for (int num = 0; num < colors.Length; num++)
			{
				SerializedPolygonalMesh.SPM_color sPM_color = spm.colors[num];
				VColor vColor = colors[num];
				vColor.linkedVertices = new Vertex[sPM_color.linkedVertices.Count];
				for (int num2 = 0; num2 < vColor.linkedVertices.Length; num2++)
				{
					vColor.linkedVertices[num2] = vertices[sPM_color.linkedVertices[num2]];
				}
			}
			int trisIndicesCounter = 0;
			polygons = new PM_Polygon[spm.polygons.Length];
			for (int num3 = 0; num3 < polygons.Length; num3++)
			{
				SerializedPolygonalMesh.SPM_Polygon sPM_Polygon = spm.polygons[num3];
				PM_Polygon pM_Polygon = new PM_Polygon(sPM_Polygon.corners.Length, ref trisIndicesCounter, spm.polygons[num3].smoothingGroup, spm.polygons[num3].materialId);
				for (int num4 = 0; num4 < pM_Polygon.corners.Length; num4++)
				{
					Vertex vertex2 = vertices[sPM_Polygon.corners[num4]];
					pM_Polygon.corners[num4] = vertex2;
				}
				pM_Polygon.SetNormalCross();
				polygons[num3] = pM_Polygon;
			}
			normals = new Normal[spm.normals.Length];
			for (int num5 = 0; num5 < normals.Length; num5++)
			{
				SerializedPolygonalMesh.SPM_Normal sPM_Normal = spm.normals[num5];
				Normal normal = new Normal(sPM_Normal.adjacentPolygons.Count);
				for (int num6 = 0; num6 < normal.adjacentPolygons.Length; num6++)
				{
					normal.adjacentPolygons[num6] = polygons[sPM_Normal.adjacentPolygons[num6]];
				}
				normals[num5] = normal;
			}
			for (int num7 = 0; num7 < vertices.Length; num7++)
			{
				SerializedPolygonalMesh.SPM_Vertex sPM_Vertex2 = spm.vertices[num7];
				Normal norm = normals[sPM_Vertex2.normIdx];
				vertices[num7].norm = norm;
			}
			for (int num8 = 0; num8 < normals.Length; num8++)
			{
				SerializedPolygonalMesh.SPM_Normal sPM_Normal2 = spm.normals[num8];
				Normal normal2 = normals[num8];
				normal2.linkedVertices = new Vertex[sPM_Normal2.linkedVertices.Count];
				for (int num9 = 0; num9 < normal2.linkedVertices.Length; num9++)
				{
					normal2.linkedVertices[num9] = vertices[sPM_Normal2.linkedVertices[num9]];
				}
			}
			mNormals = new Vector3[vertices.Length];
			mPositionsDirty = true;
			mColorDirty = true;
			mUVDirty = true;
			mTrianglesDirty = true;
			mPositions = new Vector3[vertices.Length];
			mColors = new Color[vertices.Length];
			for (int num10 = 0; num10 < spm.colors.Length; num10++)
			{
				SetColor(num10, spm.colors[num10].colorValue);
			}
			mUVs = new Vector2[vertices.Length];
			for (int num11 = 0; num11 < spm.uvs.Length; num11++)
			{
				SetUV(num11, spm.uvs[num11].uvValue);
			}
			mTriangles = new int[trisIndicesCounter];
			PM_Polygon[] array = polygons;
			for (int num12 = 0; num12 < array.Length; num12++)
			{
				array[num12].TriangulateConvex(mTriangles);
			}
			positionEdges = new PositionEdge[spm.positionEdges.Length];
			for (int num13 = 0; num13 < spm.positionEdges.Length; num13++)
			{
				SerializedPolygonalMesh.SPM_Edge sPM_Edge = spm.positionEdges[num13];
				Position a = positions[sPM_Edge.a];
				Position b = positions[sPM_Edge.b];
				positionEdges[num13] = new PositionEdge(a, b);
			}
			for (int num14 = 0; num14 < spm.positions.Length; num14++)
			{
				SerializedPolygonalMesh.SPM_Position sPM_Position = spm.positions[num14];
				Position position = positions[num14];
				position.adjacentPolygons = new PM_Polygon[sPM_Position.adjacentPolygons.Count];
				for (int num15 = 0; num15 < position.adjacentPolygons.Length; num15++)
				{
					position.adjacentPolygons[num15] = polygons[sPM_Position.adjacentPolygons[num15]];
				}
				position.linkedVertices = new Vertex[sPM_Position.linkedVertices.Count];
				for (int num16 = 0; num16 < position.linkedVertices.Length; num16++)
				{
					position.linkedVertices[num16] = vertices[sPM_Position.linkedVertices[num16]];
				}
				position.adjacentEdges = new PositionEdge[sPM_Position.adjacentEdges.Count];
				for (int num17 = 0; num17 < position.adjacentEdges.Length; num17++)
				{
					position.adjacentEdges[num17] = positionEdges[sPM_Position.adjacentEdges[num17]];
				}
				SetPosition(num14, sPM_Position.positionValue);
			}
			SetDirtyAttributes();
			OnTriangulationParamsChanged();
			Apply();
		}

		public void Apply()
		{
			base.PreDraw();
			if (formIsDirty)
			{
				PM_Polygon[] array = dynamicalyTriangulatedPolygons;
				foreach (PM_Polygon pM_Polygon in array)
				{
					if (pM_Polygon.triangulationDirty)
					{
						pM_Polygon.TriangulateNonConvex(mTriangles);
						mTrianglesDirty = true;
					}
				}
				if (lighingMode > LightingMode.Unlit)
				{
					array = polygons;
					foreach (PM_Polygon pM_Polygon2 in array)
					{
						if (pM_Polygon2.normalsDirty)
						{
							pM_Polygon2.RecalculateNormal(normalsRecalculationMode);
							Vertex[] corners = pM_Polygon2.corners;
							for (int j = 0; j < corners.Length; j++)
							{
								corners[j].norm.dirty = true;
							}
						}
					}
					Normal[] array2 = normals;
					foreach (Normal normal in array2)
					{
						if (normal.dirty)
						{
							normal.Recalculate();
							Vertex[] corners = normal.linkedVertices;
							foreach (Vertex vertex in corners)
							{
								mNormals[vertex.meshIndex] = normal;
							}
							mNormalsDirty = true;
						}
					}
				}
				FillMeshBuffers();
				if (positionEdgesWireframe != null)
				{
					positionEdgesWireframe.count = positionEdges.Length;
					for (int k = 0; k < positionEdges.Length; k++)
					{
						PositionEdge positionEdge = positionEdges[k];
						if (positionEdge.positionsDirty)
						{
							positionEdgesWireframe.SetPosition(k, positionEdge.a, positionEdge.b);
							positionEdge.positionsDirty = false;
						}
					}
				}
				formIsDirty = false;
				boundsDirty = true;
			}
			if (boundsDirty)
			{
				if (base.boundSize <= 0f)
				{
					mesh.RecalculateBounds();
					mBounds = mesh.bounds;
				}
				mesh.bounds = mBounds;
				boundsDirty = false;
			}
		}

		private void FillMeshBuffers()
		{
			if (mPositionsDirty)
			{
				mesh.vertices = mPositions;
				mPositionsDirty = false;
			}
			if (mUVDirty)
			{
				mesh.uv = mUVs;
				mUVDirty = false;
			}
			if (mColorDirty)
			{
				mesh.colors = mColors;
				mColorDirty = false;
			}
			if (_lightingMode == LightingMode.Unlit)
			{
				mesh.normals = null;
				mesh.tangents = null;
			}
			else if (_lightingMode == LightingMode.Lit)
			{
				if (mNormalsDirty)
				{
					mesh.normals = mNormals;
					mesh.tangents = null;
					mNormalsDirty = false;
				}
			}
			else if (mNormalsDirty)
			{
				mesh.normals = mNormals;
				mesh.RecalculateTangents();
				mNormalsDirty = false;
			}
			if (mTrianglesDirty)
			{
				mesh.triangles = mTriangles;
				mTrianglesDirty = false;
			}
		}

		public virtual Vector3 GetPosition(int idx)
		{
			return positions[idx].positionValue;
		}

		public virtual void SetPosition(int idx, Vector3 positionValue)
		{
			Position position = positions[idx];
			position.positionValue = positionValue;
			Vertex[] linkedVertices = position.linkedVertices;
			foreach (Vertex vertex in linkedVertices)
			{
				mPositions[vertex.meshIndex] = positionValue;
			}
			PM_Polygon[] adjacentPolygons = position.adjacentPolygons;
			foreach (PM_Polygon obj in adjacentPolygons)
			{
				obj.triangulationDirty = true;
				obj.normalsDirty = true;
			}
			PositionEdge[] adjacentEdges = position.adjacentEdges;
			for (int i = 0; i < adjacentEdges.Length; i++)
			{
				adjacentEdges[i].positionsDirty = true;
			}
			mPositionsDirty = true;
			formIsDirty = true;
		}

		public virtual void SetUV(int idx, Vector2 uvValue)
		{
			UV uV = uvs[idx];
			uV.uvValue = uvValue;
			for (int i = 0; i < uV.linkedVertices.Length; i++)
			{
				mUVs[uV.linkedVertices[i].meshIndex] = uvValue;
			}
			mUVDirty = true;
		}

		public virtual Vector2 GetUV(int idx)
		{
			return uvs[idx].uvValue;
		}

		public virtual void SetColor(int idx, Color colorValue)
		{
			VColor vColor = colors[idx];
			vColor.colorValue = colorValue;
			for (int i = 0; i < vColor.linkedVertices.Length; i++)
			{
				mColors[vColor.linkedVertices[i].meshIndex] = colorValue;
			}
			mColorDirty = true;
		}

		public virtual Color GetColor(int idx)
		{
			return colors[idx].colorValue;
		}

		protected override void PreDraw()
		{
			Apply();
		}

		protected override void OnAfterMaterialCreated()
		{
			base.OnAfterMaterialCreated();
			base.material.SetFloat("_Ambient", _ambient);
			base.material.SetVector("_TextureTransform", _textureTransform);
			base.material.SetFloat("_Culling", (!_doublesided) ? 1 : 0);
		}

		protected override string transparentShaderName()
		{
			return "Hidden/Linefy/DefaultPolygonalMeshTransparent";
		}

		protected override string opaqueShaderName()
		{
			return "Hidden/Linefy/DefaultPolygonalMesh";
		}

		public static PolygonalMesh BuildProcedural(Vector3[] posData, Vector2[] uvsData, Color[] colorsData, Polygon[] polygonsData)
		{
			SerializedPolygonalMesh procedural = SerializedPolygonalMesh.GetProcedural(posData, uvsData, colorsData, polygonsData);
			PolygonalMesh result = new PolygonalMesh(procedural);
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(procedural);
				return result;
			}
			UnityEngine.Object.DestroyImmediate(procedural);
			return result;
		}

		public override void GetStatistic(ref int linesCount, ref int totallinesCount, ref int dotsCount, ref int totalDotsCount, ref int polylinesCount, ref int totalPolylineVerticesCount)
		{
		}

		public override void Dispose()
		{
			base.Dispose();
		}

		public void SaveToSPM(SerializedPolygonalMesh spm)
		{
			Vector3[] array = new Vector3[positions.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = positions[i].positionValue;
			}
			Vector2[] array2 = new Vector2[uvs.Length];
			for (int j = 0; j < uvs.Length; j++)
			{
				array2[j] = uvs[j].uvValue;
			}
			Color[] array3 = new Color[colors.Length];
			for (int k = 0; k < array3.Length; k++)
			{
				array3[k] = colors[k].colorValue;
			}
			Polygon[] array4 = new Polygon[polygons.Length];
			for (int l = 0; l < polygons.Length; l++)
			{
				Polygon polygon = new Polygon(polygons[l].smoothingGroup, polygons[l].materialId, polygons[l].corners.Length);
				for (int m = 0; m < polygon.corners.Length; m++)
				{
					polygon.corners[m].position = polygons[l].corners[m].pos.idx;
					polygon.corners[m].uv = polygons[l].corners[m].uv.idx;
					polygon.corners[m].color = polygons[l].corners[m].color.idx;
				}
				array4[l] = polygon;
			}
			spm.BuildProcedural(array, array2, array3, array4);
		}

		public void LoadSerializationData(SerializationData_PolygonalMeshProperties inputData)
		{
			LoadSerializationData((SerializationData_LinefyDrawcall)inputData);
			ambient = inputData.ambient;
			lighingMode = inputData.lighingMode;
			dynamicTriangulationThreshold = inputData.dynamicTriangulationThreshold;
			normalsRecalculationMode = inputData.normalsRecalculationMode;
			textureTransform = inputData.textureTransform;
			doublesided = inputData.doublesided;
		}

		public void SaveSerializationData(ref SerializationData_PolygonalMeshProperties outputData)
		{
			SaveSerializationData(outputData);
			outputData.ambient = ambient;
			outputData.lighingMode = lighingMode;
			outputData.dynamicTriangulationThreshold = dynamicTriangulationThreshold;
			outputData.textureTransform = textureTransform;
			normalsRecalculationMode = normalsRecalculationMode;
		}
	}
}

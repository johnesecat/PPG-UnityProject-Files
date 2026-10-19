using System;
using Linefy.Serialization;
using UnityEngine;
using UnityEngine.Rendering;

namespace Linefy
{
	public abstract class LinefyDrawcall : Drawable
	{
		private int id_depthOffset = Shader.PropertyToID("_DepthOffset");

		private int id_viewOffset = Shader.PropertyToID("_ViewOffset");

		private int id_zTest = Shader.PropertyToID("_zTestCompare");

		private int id_fadeAlphaDistanceFrom = Shader.PropertyToID("_FadeAlphaDistanceFrom");

		private int id_fadeAlphaDistanceTo = Shader.PropertyToID("_FadeAlphaDistanceTo");

		private int id_colorMultiplier = Shader.PropertyToID("_Color");

		public string name;

		public int itemsModificationId = -1;

		public int propertiesModificationId = -1;

		protected const float defaultBoundsSize = 1000f;

		protected Mesh mesh;

		protected int _renderOrder;

		protected bool boundsDirty = true;

		protected Bounds mBounds = new Bounds(Vector3.zero, Vector3.one * 1000f);

		private float _boundsSize = 1000f;

		private bool _transparent;

		private Material _material;

		private Color _colorMultiplier = Color.white;

		protected Texture _texture;

		private float _viewOffset;

		private float _depthOffset;

		private float _fadeAlphaDistanceFrom = 100000f;

		private float _fadeAlphaDistanceTo = 100001f;

		private CompareFunction _zTest = CompareFunction.LessEqual;

		public int renderOrder
		{
			get
			{
				return _renderOrder;
			}
			set
			{
				if (value != _renderOrder)
				{
					_renderOrder = value;
					SetRenderQueue();
				}
			}
		}

		public Bounds bounds => mBounds;

		public float boundSize
		{
			get
			{
				return _boundsSize;
			}
			set
			{
				if (_boundsSize != value)
				{
					boundsDirty = true;
					_boundsSize = value;
					if (value > 0f)
					{
						mBounds = new Bounds(Vector3.zero, Vector3.one * _boundsSize);
					}
				}
			}
		}

		public bool transparent
		{
			get
			{
				return _transparent;
			}
			set
			{
				if (value != _transparent)
				{
					_transparent = value;
					ResetMaterial();
				}
			}
		}

		protected Material material
		{
			get
			{
				if (_disposed)
				{
					Debug.LogWarning("Using the object after dispose is not allowed.");
				}
				else if (_material == null)
				{
					Shader shader;
					if (transparent)
					{
						shader = Shader.Find(transparentShaderName());
						if (shader == null)
						{
							Debug.LogFormat("shader {0} not found", transparentShaderName());
						}
					}
					else
					{
						shader = Shader.Find(opaqueShaderName());
						if (shader == null)
						{
							Debug.LogFormat("shader {0} not found", opaqueShaderName());
						}
					}
					_material = new Material(shader);
					_material.hideFlags = HideFlags.HideAndDontSave;
					OnAfterMaterialCreated();
				}
				return _material;
			}
		}

		public Color colorMultiplier
		{
			get
			{
				return _colorMultiplier;
			}
			set
			{
				if (value != _colorMultiplier)
				{
					_colorMultiplier = value;
					material.SetColor(id_colorMultiplier, colorMultiplier);
				}
			}
		}

		public Texture texture
		{
			get
			{
				return _texture;
			}
			set
			{
				_texture = value;
				material.SetTexture("_MainTex", value);
			}
		}

		public float viewOffset
		{
			get
			{
				return _viewOffset;
			}
			set
			{
				if (value != _viewOffset)
				{
					material.SetFloat(id_viewOffset, value);
					_viewOffset = value;
				}
			}
		}

		public float depthOffset
		{
			get
			{
				return _depthOffset;
			}
			set
			{
				if (value != _depthOffset)
				{
					material.SetFloat(id_depthOffset, value);
					_depthOffset = value;
				}
			}
		}

		public float fadeAlphaDistanceFrom
		{
			get
			{
				return _fadeAlphaDistanceFrom;
			}
			set
			{
				if (value != _fadeAlphaDistanceFrom)
				{
					material.SetFloat("_FadeAlphaDistanceFrom", value);
					_fadeAlphaDistanceFrom = value;
				}
			}
		}

		public float fadeAlphaDistanceTo
		{
			get
			{
				return _fadeAlphaDistanceTo;
			}
			set
			{
				if (value != _fadeAlphaDistanceTo)
				{
					material.SetFloat("_FadeAlphaDistanceTo", value);
					_fadeAlphaDistanceTo = value;
				}
			}
		}

		public CompareFunction zTest
		{
			get
			{
				return _zTest;
			}
			set
			{
				if (_zTest != value)
				{
					material.SetInt(id_zTest, (int)value);
					_zTest = value;
				}
			}
		}

		protected void SetRenderQueue()
		{
			if (transparent)
			{
				material.renderQueue = 3500 + _renderOrder;
			}
			else
			{
				material.renderQueue = 2450 + _renderOrder;
			}
		}

		protected virtual string opaqueShaderName()
		{
			return "null";
		}

		protected virtual string transparentShaderName()
		{
			return "null";
		}

		protected void ResetMaterial()
		{
			if (_material != null)
			{
				UnityEngine.Object.DestroyImmediate(_material);
			}
			_material = null;
		}

		public override void DrawNow(Matrix4x4 matrix)
		{
			if (!_disposed && material != null)
			{
				PreDraw();
				material.SetPass(0);
				Graphics.DrawMeshNow(mesh, matrix);
			}
		}

		public sealed override void Draw(Matrix4x4 matrix, Camera cam, int layer)
		{
			if (!_disposed)
			{
				PreDraw();
				Graphics.DrawMesh(mesh, matrix, material, layer, cam);
			}
		}

		protected virtual void SetDirtyAttributes()
		{
			Debug.LogWarningFormat("not implemented SetDirtyAttributes() {0}", GetType());
		}

		protected virtual void PreDraw()
		{
			if (_disposed)
			{
				Debug.LogErrorFormat("{0} {1} is disposed. Yon have not to use this instance", GetType(), name);
			}
			if (mesh == null)
			{
				mesh = new Mesh();
				mesh.MarkDynamic();
				mesh.hideFlags = HideFlags.HideAndDontSave;
				mesh.indexFormat = IndexFormat.UInt32;
				SetDirtyAttributes();
			}
		}

		protected virtual void OnAfterMaterialCreated()
		{
			SetRenderQueue();
			material.SetFloat(id_depthOffset, depthOffset);
			material.SetFloat(id_viewOffset, viewOffset);
			material.SetInt(id_zTest, (int)zTest);
			material.SetFloat(id_fadeAlphaDistanceFrom, fadeAlphaDistanceFrom);
			material.SetFloat(id_fadeAlphaDistanceTo, fadeAlphaDistanceTo);
			material.SetColor(id_colorMultiplier, colorMultiplier);
			material.SetTexture("_MainTex", _texture);
		}

		public override void Dispose()
		{
			if (!_disposed)
			{
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(mesh);
					UnityEngine.Object.Destroy(material);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(mesh);
					UnityEngine.Object.DestroyImmediate(material);
				}
			}
		}

		[Obsolete("SetVisualPropertyBlock is Obsolete , use LoadSerializationData instead")]
		public virtual void SetVisualPropertyBlock(VisualPropertiesBlock block)
		{
			colorMultiplier = block.colorMuliplier;
			transparent = block.transparent;
			zTest = block.zTest;
			depthOffset = block.depthOffset;
			viewOffset = block.viewOffset;
			renderOrder = block.renderOrder;
		}

		public void LoadSerializationData(SerializationData_LinefyDrawcall inputData)
		{
			name = inputData.name;
			boundSize = inputData.boundsSize;
			renderOrder = inputData.renderOrder;
			colorMultiplier = inputData.colorMultiplier;
			texture = inputData.texture;
			viewOffset = inputData.viewOffset;
			depthOffset = inputData.depthOffset;
			fadeAlphaDistanceFrom = inputData.fadeAlphaDistance.from;
			fadeAlphaDistanceTo = inputData.fadeAlphaDistance.to;
			zTest = inputData.zTest;
			transparent = inputData.transparent;
		}

		public void SaveSerializationData(SerializationData_LinefyDrawcall outputData)
		{
			outputData.name = name;
			outputData.renderOrder = renderOrder;
			outputData.transparent = transparent;
			outputData.colorMultiplier = colorMultiplier;
			outputData.texture = texture;
			outputData.viewOffset = viewOffset;
			outputData.depthOffset = depthOffset;
			outputData.fadeAlphaDistance.from = fadeAlphaDistanceFrom;
			outputData.fadeAlphaDistance.to = fadeAlphaDistanceTo;
			outputData.zTest = zTest;
		}

		public override void GetStatistic(ref int linesCount, ref int totallinesCount, ref int dotsCount, ref int totalDotsCount, ref int polylinesCount, ref int totalPolylineVerticesCount)
		{
			Debug.LogFormat("not implemented LinefyEntity.GetStatistic(...) {0}", GetType());
		}
	}
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace LeTai.Asset.TranslucentImage
{
	[HelpURL("https://leloctai.com/asset/translucentimage/docs/articles/customize.html#translucent-image")]
	public class TranslucentImage : Image, IMeshModifier
	{
		public TranslucentImageSource source;

		[Tooltip("(De)Saturate them image, 1 is normal, 0 is black and white, below zero make the image negative")]
		[Range(-1f, 3f)]
		public float vibrancy = 1f;

		[Tooltip("Brighten/darken them image")]
		[Range(-1f, 1f)]
		public float brightness;

		[Tooltip("Flatten the color behind to help keep contrast on varying background")]
		[Range(0f, 1f)]
		public float flatten = 0.1f;

		private static readonly int _vibrancyPropId = Shader.PropertyToID("_Vibrancy");

		private static readonly int _brightnessPropId = Shader.PropertyToID("_Brightness");

		private static readonly int _flattenPropId = Shader.PropertyToID("_Flatten");

		private static readonly int _blurTexPropId = Shader.PropertyToID("_BlurTex");

		private static readonly int _cropRegionPropId = Shader.PropertyToID("_CropRegion");

		private Material materialForRenderingCached;

		private bool shouldRun;

		private bool sourceAcquiredOnStart;

		private float oldVibrancy;

		private float oldBrightness;

		private float oldFlatten;

		[Tooltip("Blend between the sprite and background blur")]
		[Range(0f, 1f)]
		[FormerlySerializedAs("spriteBlending")]
		public float m_spriteBlending = 0.65f;

		public float spriteBlending
		{
			get
			{
				return m_spriteBlending;
			}
			set
			{
				m_spriteBlending = value;
				SetVerticesDirty();
			}
		}

		protected override void Start()
		{
			base.Start();
			oldVibrancy = vibrancy;
			oldBrightness = brightness;
			oldFlatten = flatten;
			AutoAcquireSource();
			if ((bool)material)
			{
				if (Application.isPlaying && material.shader.name != "UI/TranslucentImage")
				{
					Debug.LogWarning("Translucent Image requires a material using the \"UI/TranslucentImage\" shader");
				}
				else if ((bool)source)
				{
					material.SetTexture(_blurTexPropId, source.BlurredScreen);
				}
			}
			if ((bool)base.canvas)
			{
				base.canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
			}
		}

		private bool IsInPrefabMode()
		{
			return false;
		}

		private void AutoAcquireSource()
		{
			if (!IsInPrefabMode() && !sourceAcquiredOnStart)
			{
				source = (source ? source : Object.FindObjectOfType<TranslucentImageSource>());
				sourceAcquiredOnStart = true;
			}
		}

		private bool Validate()
		{
			if (!IsActive() || !material)
			{
				return false;
			}
			if (!source)
			{
				if (Application.isPlaying && !IsInPrefabMode())
				{
					Debug.LogWarning("TranslucentImageSource is missing. Please add the TranslucentImageSource component to your main camera, then assign it to the Source field of the Translucent Image(s)");
				}
				return false;
			}
			if (!source.BlurredScreen)
			{
				return false;
			}
			return true;
		}

		private void LateUpdate()
		{
			if (shouldRun)
			{
				if (!materialForRenderingCached)
				{
					materialForRenderingCached = materialForRendering;
				}
				materialForRenderingCached.SetTexture(_blurTexPropId, source.BlurredScreen);
				materialForRenderingCached.SetVector(_cropRegionPropId, source.BlurRegionNormalizedScreenSpace.ToMinMaxVector());
			}
		}

		private void Update()
		{
			shouldRun = Validate();
			if (shouldRun && _vibrancyPropId != 0 && _brightnessPropId != 0 && _flattenPropId != 0)
			{
				materialForRenderingCached = materialForRendering;
				SyncMaterialProperty(_vibrancyPropId, ref vibrancy, ref oldVibrancy);
				SyncMaterialProperty(_brightnessPropId, ref brightness, ref oldBrightness);
				SyncMaterialProperty(_flattenPropId, ref flatten, ref oldFlatten);
			}
		}

		private void SyncMaterialProperty(int propId, ref float value, ref float oldValue)
		{
			float @float = materialForRendering.GetFloat(propId);
			if ((double)Mathf.Abs(@float - value) > 0.0001)
			{
				if ((double)Mathf.Abs(value - oldValue) > 0.0001)
				{
					if ((bool)materialForRenderingCached)
					{
						materialForRenderingCached.SetFloat(propId, value);
					}
					material.SetFloat(propId, value);
					SetMaterialDirty();
				}
				else
				{
					value = @float;
				}
			}
			oldValue = value;
		}

		public virtual void ModifyMesh(VertexHelper vh)
		{
			List<UIVertex> list = new List<UIVertex>();
			vh.GetUIVertexStream(list);
			for (int i = 0; i < list.Count; i++)
			{
				UIVertex value = list[i];
				value.uv1 = new Vector2(spriteBlending, 0f);
				list[i] = value;
			}
			vh.Clear();
			vh.AddUIVertexTriangleStream(list);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			SetVerticesDirty();
		}

		protected override void OnDisable()
		{
			SetVerticesDirty();
			base.OnDisable();
		}

		protected override void OnDidApplyAnimationProperties()
		{
			SetVerticesDirty();
			base.OnDidApplyAnimationProperties();
		}

		public virtual void ModifyMesh(Mesh mesh)
		{
			using (VertexHelper vertexHelper = new VertexHelper(mesh))
			{
				ModifyMesh(vertexHelper);
				vertexHelper.FillMesh(mesh);
			}
		}
	}
}

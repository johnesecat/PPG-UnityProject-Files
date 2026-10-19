using UnityEngine;

namespace LeTai.Asset.TranslucentImage
{
	public class ScalableBlur : IBlurAlgorithm
	{
		private Shader shader;

		private Material material;

		private ScalableBlurConfig config;

		private const int BLUR_PASS = 0;

		private const int CROP_BLUR_PASS = 1;

		private Material Material
		{
			get
			{
				if (material == null)
				{
					Material = new Material(Shader.Find("Hidden/EfficientBlur"));
				}
				return material;
			}
			set
			{
				material = value;
			}
		}

		public void Init(BlurConfig config)
		{
			this.config = (ScalableBlurConfig)config;
		}

		public void Blur(RenderTexture source, Rect sourceCropRegion, ref RenderTexture blurredTexture)
		{
			if (blurredTexture.IsCreated())
			{
				blurredTexture.DiscardContents();
			}
			float radius = ScaleWithResolution(config.Radius, (float)blurredTexture.width * sourceCropRegion.width, (float)blurredTexture.height * sourceCropRegion.height);
			int downsampleFactor = ((config.Iteration > 0) ? 1 : 0);
			RenderTexture target = CreateTempRenderTextureFrom(blurredTexture, downsampleFactor);
			FilterMode filterMode = source.filterMode;
			source.filterMode = FilterMode.Bilinear;
			ConfigMaterial(radius, sourceCropRegion.ToMinMaxVector());
			Graphics.Blit(source, target, Material, 1);
			for (int i = 2; i <= config.Iteration; i++)
			{
				BlurAtDepth(i, ref blurredTexture, ref target);
			}
			for (int num = config.Iteration - 1; num >= 1; num--)
			{
				BlurAtDepth(num, ref blurredTexture, ref target);
			}
			Graphics.Blit(target, blurredTexture, Material, 0);
			RenderTexture.ReleaseTemporary(target);
			source.filterMode = filterMode;
		}

		private float ScaleWithResolution(float baseRadius, float width, float height)
		{
			float value = Mathf.Min(width, height) / 1080f;
			value = Mathf.Clamp(value, 0.5f, 2f);
			return baseRadius * value;
		}

		protected void ConfigMaterial(float radius, Vector4 cropRegion)
		{
			Material.SetFloat(ShaderId.RADIUS, radius);
			Material.SetVector(ShaderId.CROP_REGION, cropRegion);
		}

		private RenderTexture CreateTempRenderTextureFrom(RenderTexture source, int downsampleFactor)
		{
			RenderTextureDescriptor descriptor = source.descriptor;
			descriptor.width = source.width >> downsampleFactor;
			descriptor.height = source.height >> downsampleFactor;
			RenderTexture temporary = RenderTexture.GetTemporary(descriptor);
			temporary.filterMode = FilterMode.Bilinear;
			return temporary;
		}

		protected virtual void BlurAtDepth(int depth, ref RenderTexture baseTexture, ref RenderTexture target)
		{
			RenderTexture renderTexture = CreateTempRenderTextureFrom(baseTexture, Mathf.Min(depth, config.MaxDepth));
			Graphics.Blit(target, renderTexture, Material, 0);
			RenderTexture.ReleaseTemporary(target);
			target = renderTexture;
		}
	}
}

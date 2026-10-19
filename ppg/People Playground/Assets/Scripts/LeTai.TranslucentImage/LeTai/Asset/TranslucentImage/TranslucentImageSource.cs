using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

namespace LeTai.Asset.TranslucentImage
{
	[ExecuteAlways]
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Image Effects/Tai Le Assets/Translucent Image Source")]
	[HelpURL("https://leloctai.com/asset/translucentimage/docs/articles/customize.html#translucent-image-source")]
	public class TranslucentImageSource : MonoBehaviour
	{
		public float maxUpdateRate = float.PositiveInfinity;

		public bool preview;

		private IBlurAlgorithm blurAlgorithm;

		[SerializeField]
		private BlurAlgorithmType blurAlgorithmSelection;

		[SerializeField]
		private BlurConfig blurConfig;

		[SerializeField]
		private int downsample;

		private int lastDownsample;

		[SerializeField]
		private Rect blurRegion = new Rect(0f, 0f, 1f, 1f);

		private Rect lastBlurRegion = new Rect(0f, 0f, 1f, 1f);

		private Camera camera;

		private Material previewMaterial;

		private RenderTexture blurredScreen;

		private TextureDimension lastEyeTexDim;

		private float lastUpdate;

		public BlurAlgorithmType BlurAlgorithmSelection
		{
			get
			{
				return blurAlgorithmSelection;
			}
			set
			{
				if (value != blurAlgorithmSelection)
				{
					blurAlgorithmSelection = value;
					InitializeBlurAlgorithm();
				}
			}
		}

		public BlurConfig BlurConfig
		{
			get
			{
				return blurConfig;
			}
			set
			{
				blurConfig = value;
				InitializeBlurAlgorithm();
			}
		}

		public RenderTexture BlurredScreen
		{
			get
			{
				return blurredScreen;
			}
			set
			{
				blurredScreen = value;
			}
		}

		internal Camera Cam
		{
			get
			{
				if (!camera)
				{
					return camera = GetComponent<Camera>();
				}
				return camera;
			}
		}

		public int Downsample
		{
			get
			{
				return downsample;
			}
			set
			{
				downsample = Mathf.Max(0, value);
			}
		}

		public Rect BlurRegion
		{
			get
			{
				return blurRegion;
			}
			set
			{
				Vector2 vector = new Vector2(1f / (float)Cam.pixelWidth, 1f / (float)Cam.pixelHeight);
				blurRegion.x = Mathf.Clamp(value.x, 0f, 1f - vector.x);
				blurRegion.y = Mathf.Clamp(value.y, 0f, 1f - vector.y);
				blurRegion.width = Mathf.Clamp(value.width, vector.x, 1f - blurRegion.x);
				blurRegion.height = Mathf.Clamp(value.height, vector.y, 1f - blurRegion.y);
			}
		}

		public Rect BlurRegionNormalizedScreenSpace
		{
			get
			{
				Rect rect = Cam.rect;
				return new Rect(rect.position + BlurRegion.position * rect.size, rect.size * BlurRegion.size);
			}
			set
			{
				Rect rect = Cam.rect;
				blurRegion.position = (value.position - rect.position) / rect.size;
				blurRegion.size = value.size / rect.size;
			}
		}

		private float MinUpdateCycle
		{
			get
			{
				if (!(maxUpdateRate > 0f))
				{
					return float.PositiveInfinity;
				}
				return 1f / maxUpdateRate;
			}
		}

		protected virtual void Start()
		{
			previewMaterial = new Material(Shader.Find("Hidden/FillCrop"));
			InitializeBlurAlgorithm();
			CreateNewBlurredScreen();
			lastDownsample = Downsample;
		}

		private void OnDestroy()
		{
			if ((bool)BlurredScreen)
			{
				BlurredScreen.Release();
			}
		}

		private void InitializeBlurAlgorithm()
		{
			if (BlurAlgorithmSelection == BlurAlgorithmType.ScalableBlur)
			{
				blurAlgorithm = new ScalableBlur();
				blurAlgorithm.Init(BlurConfig);
				return;
			}
			throw new ArgumentOutOfRangeException("BlurAlgorithmSelection");
		}

		protected virtual void CreateNewBlurredScreen()
		{
			if ((bool)BlurredScreen)
			{
				BlurredScreen.Release();
			}
			if (XRSettings.enabled)
			{
				BlurredScreen = new RenderTexture(XRSettings.eyeTextureDesc);
				BlurredScreen.width = Mathf.RoundToInt((float)BlurredScreen.width * BlurRegion.width) >> Downsample;
				BlurredScreen.height = Mathf.RoundToInt((float)BlurredScreen.height * BlurRegion.height) >> Downsample;
				BlurredScreen.depth = 0;
			}
			else
			{
				BlurredScreen = new RenderTexture(Mathf.RoundToInt((float)Cam.pixelWidth * BlurRegion.width) >> Downsample, Mathf.RoundToInt((float)Cam.pixelHeight * BlurRegion.height) >> Downsample, 0);
			}
			BlurredScreen.antiAliasing = 1;
			BlurredScreen.useMipMap = false;
			BlurredScreen.name = base.gameObject.name + " Translucent Image Source";
			BlurredScreen.filterMode = FilterMode.Bilinear;
			BlurredScreen.Create();
		}

		public void OnBeforeBlur()
		{
			if (BlurredScreen == null || !BlurredScreen.IsCreated() || Downsample != lastDownsample || !BlurRegion.Approximately(lastBlurRegion) || XRSettings.deviceEyeTextureDimension != lastEyeTexDim)
			{
				CreateNewBlurredScreen();
				lastDownsample = Downsample;
				lastBlurRegion = BlurRegion;
				lastEyeTexDim = XRSettings.deviceEyeTextureDimension;
			}
		}

		protected virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (blurAlgorithm != null && !(BlurConfig == null))
			{
				if (shouldUpdateBlur())
				{
					OnBeforeBlur();
					blurAlgorithm.Blur(source, BlurRegion, ref blurredScreen);
				}
				if (preview)
				{
					previewMaterial.SetVector(ShaderId.CROP_REGION, BlurRegion.ToMinMaxVector());
					Graphics.Blit(BlurredScreen, destination, previewMaterial);
					return;
				}
			}
			Graphics.Blit(source, destination);
		}

		public bool shouldUpdateBlur()
		{
			if (!base.enabled)
			{
				return false;
			}
			bool num = GetTrueCurrentTime() - lastUpdate >= MinUpdateCycle;
			if (num)
			{
				lastUpdate = GetTrueCurrentTime();
			}
			return num;
		}

		private static float GetTrueCurrentTime()
		{
			return Time.unscaledTime;
		}
	}
}

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class VideoSettingsInitBehaviour : MonoBehaviour
{
	public string BloomKey;

	public string TonemapKey;

	public string AAKey;

	public string VSyncKey;

	public string WindowModeKey;

	public string ResolutionKey;

	public string UIScaleKey;

	public string PhysicsIterationsKey;

	public UnityEvent OnLoadStart;

	private void Start()
	{
		Sync();
	}

	public void Sync()
	{
		OnLoadStart.Invoke();
		Preferences current = UserPreferenceManager.Current;
		Physics2D.positionIterations = current.PhysicsIterations;
		Physics2D.velocityIterations = current.PhysicsIterations;
		if (current.VSync)
		{
			QualitySettings.vSyncCount = (current.VSync ? 1 : 0);
		}
		else
		{
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = Mathf.Max(12, current.FramerateLimit);
		}
		PanelsBehaviour panelsBehaviour = Object.FindObjectOfType<PanelsBehaviour>();
		if ((bool)panelsBehaviour)
		{
			panelsBehaviour.Refresh();
		}
		Object.FindObjectOfType<CanvasScaler>().referenceResolution = new Vector2(2688f, 1080f);
		float num = Mathf.Clamp(current.RenderScale, 0.25f, 1f);
		Vector2Int vector2Int = new Vector2Int(Mathf.RoundToInt((float)Display.main.systemWidth * num), Mathf.RoundToInt((float)Display.main.systemHeight * num));
		switch (current.WindowMode)
		{
		case WindowMode.Windowed:
			Screen.fullScreen = false;
			Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.Windowed);
			break;
		case WindowMode.Borderless:
			Screen.fullScreen = true;
			Screen.SetResolution(vector2Int.x, vector2Int.y, FullScreenMode.FullScreenWindow);
			break;
		case WindowMode.Fullscreen:
			Screen.fullScreen = true;
			Screen.SetResolution(vector2Int.x, vector2Int.y, FullScreenMode.ExclusiveFullScreen);
			break;
		}
		if ((bool)Global.main)
		{
			for (int i = 0; i < Global.main.PhysicalObjectsInWorld.Count; i++)
			{
				PhysicalBehaviour physicalBehaviour = Global.main.PhysicalObjectsInWorld[i];
				if ((bool)physicalBehaviour && (bool)physicalBehaviour.rigidbody)
				{
					CollisionQuality collisionQuality = current.CollisionQuality;
					if (collisionQuality != 0 && collisionQuality == CollisionQuality.Continuous)
					{
						physicalBehaviour.rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
					}
					else
					{
						physicalBehaviour.rigidbody.collisionDetectionMode = (physicalBehaviour.ForceContinuous ? CollisionDetectionMode2D.Continuous : CollisionDetectionMode2D.None);
					}
				}
			}
		}
		PostProcessVolume[] array = Object.FindObjectsOfType<PostProcessVolume>();
		foreach (PostProcessVolume post in array)
		{
			SetPost(post);
		}
		PostProcessLayer postProcessLayer = Object.FindObjectOfType<PostProcessLayer>();
		if ((bool)postProcessLayer)
		{
			if (current.SMAA)
			{
				postProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
				postProcessLayer.subpixelMorphologicalAntialiasing.quality = SubpixelMorphologicalAntialiasing.Quality.High;
			}
			else
			{
				postProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.None;
			}
		}
	}

	private void SetPost(PostProcessVolume post)
	{
		if (!post)
		{
			return;
		}
		Bloom setting = post.profile.GetSetting<Bloom>();
		ColorGrading setting2 = post.profile.GetSetting<ColorGrading>();
		LightsEffect setting3 = post.profile.GetSetting<LightsEffect>();
		if ((bool)setting3)
		{
			LightRenderCameraBehaviour lightRenderCameraBehaviour = Object.FindObjectOfType<LightRenderCameraBehaviour>();
			if ((bool)lightRenderCameraBehaviour)
			{
				lightRenderCameraBehaviour.ResetRT();
				lightRenderCameraBehaviour.MyCamera.enabled = UserPreferenceManager.Current.Lighting && Utils.AreHdrRenderTexturesSupported();
				setting3.enabled.Override(lightRenderCameraBehaviour.MyCamera.enabled);
			}
		}
		if ((bool)setting)
		{
			switch (UserPreferenceManager.Current.BloomMode)
			{
			case BloomMode.Off:
				setting.active = false;
				break;
			case BloomMode.Fast:
				setting.active = true;
				setting.fastMode.Override(x: true);
				break;
			case BloomMode.Fancy:
				setting.active = true;
				setting.fastMode.Override(x: false);
				break;
			}
		}
		if ((bool)setting2)
		{
			setting2.postExposure.Override(1.385869f + Utils.MapRange(10f, 400f, -3f, 4f, UserPreferenceManager.Current.Brightness));
			switch (UserPreferenceManager.Current.TonemappingMode)
			{
			case TonemappingMode.Modern:
				setting2.tonemapper.Override(Tonemapper.ACES);
				break;
			case TonemappingMode.Legacy:
				setting2.tonemapper.Override(Tonemapper.Neutral);
				break;
			}
		}
	}
}

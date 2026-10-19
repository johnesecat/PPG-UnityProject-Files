using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class LightRenderCameraBehaviour : MonoBehaviour
{
	public Camera MyCamera;

	public Camera MainCamera;

	public PostProcessVolume PostProcessVolume;

	private Vector2Int lastResolution;

	private void Start()
	{
		ResetRT();
	}

	public void ResetRT()
	{
		if ((bool)MyCamera.targetTexture)
		{
			MyCamera.targetTexture.Release();
			Object.Destroy(MyCamera.targetTexture);
		}
		RenderTexture renderTexture = CreateNewRendertexture();
		MyCamera.targetTexture = renderTexture;
		PostProcessVolume.profile.GetSetting<LightsEffect>().LightTexture.Override(renderTexture);
	}

	private RenderTexture CreateNewRendertexture()
	{
		if (!Utils.AreHdrRenderTexturesSupported())
		{
			Debug.LogWarning("System does not support HDR RenderTextures");
			return null;
		}
		RenderTexture renderTexture = new RenderTexture(MainCamera.pixelWidth / 2, MainCamera.pixelHeight / 2, 16, DefaultFormat.HDR);
		lastResolution = new Vector2Int(MainCamera.pixelWidth, MainCamera.pixelHeight);
		renderTexture.Create();
		Debug.Log($"New light overlay texture created: {renderTexture.IsCreated()}, {renderTexture.width}x{renderTexture.height}");
		return renderTexture;
	}

	private void OnPreRender()
	{
		SyncTransform();
		SyncOrthographicSize();
	}

	private void Update()
	{
		if (lastResolution.x != MainCamera.pixelWidth || lastResolution.y != MainCamera.pixelHeight)
		{
			UserPreferenceManager.Current.Resolution = new Vector2Int(Screen.width, Screen.height);
			ResetRT();
		}
	}

	private void SyncOrthographicSize()
	{
		MyCamera.orthographicSize = MainCamera.orthographicSize;
	}

	private void SyncTransform()
	{
		Transform transform = MainCamera.transform;
		MyCamera.transform.SetPositionAndRotation(transform.position, transform.rotation);
	}
}

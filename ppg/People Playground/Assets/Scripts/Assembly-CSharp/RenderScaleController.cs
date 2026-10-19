using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class RenderScaleController : MonoBehaviour
{
	private RenderTexture target;

	private void OnDestroy()
	{
		if ((bool)target)
		{
			Object.Destroy(target);
		}
	}

	private void Start()
	{
		ScalableBufferManager.ResizeBuffers(0.1f, 0.1f);
	}

	public void SetRenderScale(float scale)
	{
		if ((bool)target)
		{
			Object.Destroy(target);
		}
		Resolution currentResolution = Screen.currentResolution;
		scale = Mathf.Clamp01(scale);
		int width = Mathf.FloorToInt((float)currentResolution.width * scale);
		int height = Mathf.FloorToInt((float)currentResolution.height * scale);
		target = new RenderTexture(width, height, 16, DefaultFormat.HDR);
	}

	private void Update()
	{
		if (ScalableBufferManager.widthScaleFactor != 0.1f)
		{
			ScalableBufferManager.ResizeBuffers(0.1f, 0.1f);
		}
	}
}

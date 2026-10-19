using UnityEngine;

[ExecuteAlways]
public class IgnoreCameraZoom : MonoBehaviour
{
	public Camera CameraToConsider;

	public float SizeMultiplier = 1f;

	private void Start()
	{
		if (!CameraToConsider)
		{
			CameraToConsider = Global.main.camera;
		}
	}

	private void OnWillRenderObject()
	{
		base.transform.localScale = CameraToConsider.orthographicSize * SizeMultiplier * Vector3.one;
	}
}

using System;
using UnityEngine;

[Obsolete]
public class BathroomMirrorController : MonoBehaviour
{
	public RenderTexture Target;

	private void Start()
	{
		Camera.onPostRender = _OnPostRender;
	}

	private void _OnPostRender(Camera cam)
	{
		if (!(cam != Global.main.camera) && (bool)Target && (bool)cam.activeTexture)
		{
			RenderTexture activeTexture = cam.activeTexture;
			_ = (float)Target.width / (float)activeTexture.width;
			Graphics.Blit(activeTexture, Target);
		}
	}
}

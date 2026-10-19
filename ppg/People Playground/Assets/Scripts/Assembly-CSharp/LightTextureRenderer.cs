using UnityEngine;

[ExecuteInEditMode]
public class LightTextureRenderer : MonoBehaviour
{
	public Material MultiplyShader;

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, MultiplyShader);
	}
}

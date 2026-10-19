using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public sealed class LightsEffectRenderer : PostProcessEffectRenderer<LightsEffect>
{
	private Shader shader;

	public override void Init()
	{
		shader = Shader.Find("Hidden/Custom/LightPost");
	}

	public override void Render(PostProcessRenderContext context)
	{
		if (base.settings.LightTexture != null && !(base.settings.LightTexture.value == null))
		{
			PropertySheet propertySheet = context.propertySheets.Get(shader);
			propertySheet.properties.SetFloat(ShaderProperties.Get("_Intensity"), base.settings.Intensity);
			propertySheet.properties.SetFloat(ShaderProperties.Get("_MinimumBrightness"), base.settings.MinBrightness);
			propertySheet.properties.SetFloat(ShaderProperties.Get("_MaximumBrightness"), base.settings.MaxBrightness);
			propertySheet.properties.SetTexture(ShaderProperties.Get("_LightsRenderTexture"), base.settings.LightTexture);
			context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
		}
	}
}

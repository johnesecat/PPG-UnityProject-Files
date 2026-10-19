using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public sealed class HeatEffectRenderer : PostProcessEffectRenderer<HeatEffect>
{
	private Shader shader;

	public override void Init()
	{
		shader = Shader.Find("Hidden/Custom/HeatEffect");
	}

	public override void Render(PostProcessRenderContext context)
	{
		PropertySheet propertySheet = context.propertySheets.Get(shader);
		propertySheet.properties.SetFloat(ShaderProperties.Get("_Intensity"), base.settings.Intensity);
		propertySheet.properties.SetFloat(ShaderProperties.Get("_Frequency"), base.settings.Frequency);
		propertySheet.properties.SetFloat(ShaderProperties.Get("_Speed"), base.settings.Speed);
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
	}
}

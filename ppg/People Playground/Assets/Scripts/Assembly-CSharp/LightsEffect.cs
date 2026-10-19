using System;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(LightsEffectRenderer), PostProcessEvent.BeforeStack, "Custom/Lights", false)]
public class LightsEffect : PostProcessEffectSettings
{
	public TextureParameter LightTexture = new TextureParameter();

	public FloatParameter Intensity = new FloatParameter
	{
		value = 1f
	};

	public FloatParameter MinBrightness = new FloatParameter
	{
		value = 0.05f
	};

	public FloatParameter MaxBrightness = new FloatParameter
	{
		value = 100f
	};
}

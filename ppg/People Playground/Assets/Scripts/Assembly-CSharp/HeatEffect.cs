using System;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(HeatEffectRenderer), PostProcessEvent.AfterStack, "Custom/Heat", true)]
public class HeatEffect : PostProcessEffectSettings
{
	public FloatParameter Intensity = new FloatParameter
	{
		value = 0f
	};

	public FloatParameter Frequency = new FloatParameter
	{
		value = 5f
	};

	public FloatParameter Speed = new FloatParameter
	{
		value = -1f
	};
}

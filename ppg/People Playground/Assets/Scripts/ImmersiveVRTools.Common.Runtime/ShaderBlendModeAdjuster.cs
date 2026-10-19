using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ShaderBlendModeAdjuster : MonoBehaviour
{
	[SerializeField]
	private List<Material> _materials;

	[SerializeField]
	private string _srcBlendShaderPropertyName = "_SrcBlend";

	[SerializeField]
	private string _dstBlendShaderPropertyName = "_DstBlend";

	public void SetMaterialsTo(BlendMode srcBlend, BlendMode dstBlend)
	{
		foreach (Material material in _materials)
		{
			material.SetInt(_srcBlendShaderPropertyName, (int)srcBlend);
			material.SetInt(_dstBlendShaderPropertyName, (int)dstBlend);
		}
	}

	public void SetMaterialsToOneZero()
	{
		SetMaterialsTo(BlendMode.One, BlendMode.Zero);
	}

	public void SetMaterialsToAlphaBlending()
	{
		SetMaterialsTo(BlendMode.SrcAlpha, BlendMode.OneMinusSrcAlpha);
	}
}

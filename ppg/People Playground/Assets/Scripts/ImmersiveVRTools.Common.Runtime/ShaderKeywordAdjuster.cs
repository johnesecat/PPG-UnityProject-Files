using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShaderKeywordAdjuster : MonoBehaviour
{
	[SerializeField]
	private List<string> _shaderKeywords;

	[SerializeField]
	private List<Material> _materials;

	[ContextMenu("ToggleAllKeywords")]
	public void ToggleAllKeywords()
	{
		Material material = _materials[0];
		foreach (string shaderKeyword in _shaderKeywords)
		{
			bool flag = material.IsKeywordEnabled(shaderKeyword);
			foreach (Material material2 in _materials)
			{
				if (flag)
				{
					material2.DisableKeyword(shaderKeyword);
				}
				else
				{
					material2.EnableKeyword(shaderKeyword);
				}
			}
		}
	}

	public List<bool> ResolveKeywordEnabledState()
	{
		Material firstMaterial = _materials[0];
		return _shaderKeywords.Select((string keywordName) => firstMaterial.IsKeywordEnabled(keywordName)).ToList();
	}
}

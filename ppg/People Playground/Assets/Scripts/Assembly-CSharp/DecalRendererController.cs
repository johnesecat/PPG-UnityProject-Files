using System;
using UnityEngine;

[Obsolete]
public class DecalRendererController : MonoBehaviour, Messages.IDecal
{
	private DecalRenderer decalRenderer;

	private void Awake()
	{
		decalRenderer = GetComponentInChildren<DecalRenderer>();
	}

	public void Decal(DecalInstruction decalInstruction)
	{
		if ((bool)decalRenderer)
		{
			decalRenderer.Decal(decalInstruction);
		}
	}
}

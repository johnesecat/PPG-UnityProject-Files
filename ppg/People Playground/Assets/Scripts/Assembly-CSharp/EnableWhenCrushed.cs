using System;
using UnityEngine;

public class EnableWhenCrushed : MonoBehaviour
{
	public PhysicalBehaviour CrushTarget;

	public Behaviour Component;

	public Renderer RendererBecauseUnityIsBad;

	private void Awake()
	{
		if ((bool)CrushTarget)
		{
			CrushTarget.OnDisintegration += PhysicalBehaviour_OnDisintegration;
			Component.enabled = false;
			if ((bool)RendererBecauseUnityIsBad)
			{
				RendererBecauseUnityIsBad.enabled = false;
			}
		}
	}

	private void PhysicalBehaviour_OnDisintegration(object sender, EventArgs e)
	{
		Component.enabled = true;
		if ((bool)RendererBecauseUnityIsBad)
		{
			RendererBecauseUnityIsBad.enabled = true;
		}
	}
}

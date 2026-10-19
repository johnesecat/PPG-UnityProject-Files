using System;
using UnityEngine;

[Obsolete]
public class FireExtinguisherBehaviour : MonoBehaviour
{
	public ParticleSystem particleSystem;

	private void UseContinuous()
	{
		ContinuousActivationBehaviour.AssertState();
	}
}

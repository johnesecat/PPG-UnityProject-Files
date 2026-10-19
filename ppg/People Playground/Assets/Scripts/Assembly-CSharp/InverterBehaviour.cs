using System;
using UnityEngine;

[Obsolete]
public class InverterBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	public bool Activated;

	private void Use()
	{
		Activated = !Activated;
	}

	private void FixedUpdate()
	{
		PhysicalBehaviour.EnergyWireResistance = ((!Activated) ? 1 : (-1));
	}
}

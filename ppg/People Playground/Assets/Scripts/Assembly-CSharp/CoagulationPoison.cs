using System;

[Obsolete]
public class CoagulationPoison : PoisonSpreadBehaviour
{
	public override void Start()
	{
		Limb.CirculationBehaviour.HealBleeding();
	}
}

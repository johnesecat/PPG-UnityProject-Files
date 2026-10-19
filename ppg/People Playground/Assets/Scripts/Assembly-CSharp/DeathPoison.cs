using System;

[Obsolete]
public class DeathPoison : PoisonSpreadBehaviour
{
	public override float SpreadSpeed => 10f;

	public override void Start()
	{
		Limb.CirculationBehaviour.IsPump = false;
	}
}

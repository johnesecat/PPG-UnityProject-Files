using System;

[Obsolete]
public class FreezePoison : PoisonSpreadBehaviour
{
	public override void Start()
	{
		Limb.Frozen = true;
	}
}

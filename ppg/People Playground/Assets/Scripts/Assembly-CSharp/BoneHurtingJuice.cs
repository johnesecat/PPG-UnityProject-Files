using System;

[Obsolete]
public class BoneHurtingJuice : PoisonSpreadBehaviour
{
	public override void Start()
	{
		Limb.BreakBone();
	}
}

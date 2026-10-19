using System;

[Obsolete]
public class KnockoutPoison : PoisonSpreadBehaviour
{
	public override float Lifespan => 5f;

	public override void Start()
	{
		Limb.Numbness = 1f;
		if (Limb.RoughClassification == LimbBehaviour.BodyPart.Head)
		{
			Limb.Person.Consciousness = 0f;
		}
	}
}

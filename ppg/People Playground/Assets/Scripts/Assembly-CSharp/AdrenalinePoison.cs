using System;

[Obsolete]
public class AdrenalinePoison : PoisonSpreadBehaviour
{
	public override float SpreadSpeed => 20f;

	public override void Start()
	{
		if (Limb.RoughClassification == LimbBehaviour.BodyPart.Head)
		{
			Limb.Person.AdrenalineLevel = 20f;
			Limb.Person.Consciousness = 1f;
			Limb.Person.PainLevel = 0f;
			Limb.Person.ShockLevel = 0f;
		}
	}
}

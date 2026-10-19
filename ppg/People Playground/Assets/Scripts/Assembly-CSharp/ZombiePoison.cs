using System;

[Obsolete]
public class ZombiePoison : PoisonSpreadBehaviour
{
	public bool FillBlood = true;

	public override float SpreadSpeed => 500f;

	public override void Start()
	{
		Limb.IsZombie = true;
		Limb.Health = Limb.InitialHealth;
		Limb.Numbness = 0f;
		if (FillBlood)
		{
			Limb.CirculationBehaviour.AddLiquid(Limb.GetOriginalBloodType(), 5f);
			Limb.CirculationBehaviour.BloodFlow = 5f;
		}
		Limb.CirculationBehaviour.HealBleeding();
		Limb.CirculationBehaviour.BleedingRate = 0f;
		Limb.CirculationBehaviour.IsPump = Limb.CirculationBehaviour.WasInitiallyPumping;
		if (Limb.RoughClassification == LimbBehaviour.BodyPart.Head)
		{
			Limb.Person.Consciousness = 1f;
			Limb.Person.ShockLevel = 0f;
			Limb.Person.PainLevel = 0f;
			Limb.Person.OxygenLevel = 1f;
			Limb.Person.AdrenalineLevel = 1f;
		}
	}
}

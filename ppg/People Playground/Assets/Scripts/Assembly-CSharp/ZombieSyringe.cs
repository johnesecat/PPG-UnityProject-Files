using UnityEngine;

public class ZombieSyringe : SyringeBehaviour
{
	public class ZombiePoisonLiquid : Liquid
	{
		public const string ID = "REANIMATION AGENT";

		public override string GetDisplayName()
		{
			return "Reanimation agent";
		}

		public ZombiePoisonLiquid()
		{
			Color = new Color(0.8835177f, 1f, 151f / 212f);
		}

		public override void OnEnterContainer(BloodContainer container)
		{
		}

		public override void OnEnterLimb(LimbBehaviour limb)
		{
			if (!(limb.SpeciesIdentity == "Android"))
			{
				limb.IsZombie = true;
				limb.Health = limb.InitialHealth;
				limb.Numbness = 0f;
				CirculationBehaviour circulationBehaviour = limb.CirculationBehaviour;
				float amount = circulationBehaviour.GetAmount(limb.GetOriginalBloodType());
				if (amount < 0.1f)
				{
					circulationBehaviour.AddLiquid(limb.GetOriginalBloodType(), 1f - amount);
				}
				circulationBehaviour.BloodFlow = 1f;
				circulationBehaviour.HealBleeding();
				circulationBehaviour.BleedingRate = 0f;
				circulationBehaviour.IsPump = circulationBehaviour.WasInitiallyPumping;
				if (limb.RoughClassification == LimbBehaviour.BodyPart.Head)
				{
					limb.Person.Braindead = false;
					limb.Person.BrainDamaged = false;
					limb.Person.Consciousness = 1f;
					limb.Person.ShockLevel = 0f;
					limb.Person.PainLevel = 0f;
					limb.Person.OxygenLevel = 1f;
					limb.Person.AdrenalineLevel = 1f;
				}
			}
		}

		public override void OnExitContainer(BloodContainer container)
		{
		}
	}

	public override string GetLiquidID()
	{
		return "REANIMATION AGENT";
	}
}
